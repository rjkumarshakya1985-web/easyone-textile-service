
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Enums;
using Textile.Core.Entities.Models.Requests;
using Textile.Core.Entities.Models.Response.Parcels;
using Textile.Core.Entities.Views;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Interfaces.Data;
using Textile.Core.Interfaces.Services;

namespace Textile.Core.Managers.Services
{
    public class ParcelService(IUnitOfWork unitOfWork,TextileDbContext context) : IParcelService
    {

        private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        private readonly TextileDbContext _context = context ?? throw new ArgumentNullException(nameof(context));

    
        public async Task<bool> ChangeSaleVouchersStatus(ParcelScanRequest parcelScanRequest, Guid createdBy, string currentUser)
        {
            if (parcelScanRequest == null || parcelScanRequest.SaleVoucherId == null || !parcelScanRequest.SaleVoucherId.Any())
                return false;

            var repository = _unitOfWork.Repository<SaleVoucher, int>();
            var newStatus = (int)parcelScanRequest.StatusEnum;

            // Fetch all parcels in one DB call
            var saleVouchers = await repository
                .GetAllAsync(x => parcelScanRequest.SaleVoucherId.Contains(x.Id));

            if (!saleVouchers.Any())
                return false;

            foreach (var voucher in saleVouchers)
            {
                if(newStatus == (int)ParcelStatusEnum.Opened || newStatus == (int)ParcelStatusEnum.PackedAtLocation || newStatus == (int)ParcelStatusEnum.TallySynced)
                {
                    voucher.IsExported = false;
                }
                voucher.Status = newStatus;
              
            }

            var SaleVoucherStatuses = saleVouchers.Select(voucher => new SaleVoucherStatus
            {
                SaleVoucherId = voucher.Id,
                Status = newStatus,
                CreatedBy = createdBy,
                CreatedOn = DateTime.UtcNow,
                CreatedByUserName = currentUser,
                Date = DateTime.UtcNow
            }).ToList();


            var saleVoucherStatusRepository = _unitOfWork.Repository<SaleVoucherStatus, Guid>();

            await saleVoucherStatusRepository.AddAsync(SaleVoucherStatuses);


            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<ParcelResponse> GetParcelScanInfoAsync(int parcelId, ParcelStatusEnum requestedStatus)
        {
            var repository = _unitOfWork.Repository<SaleVoucher, int>();

            var saleVoucher = await _context.SaleVouchers
                              .Where(x => !x.IsDeleted && x.Id == parcelId)
                              .Include(x => x.Supplier)
                              .Include(x => x.Transport)
                              .Include(x => x.SaleVoucherDetails)
                              .ThenInclude(d => d.Product)
                              .SingleOrDefaultAsync();

            if (saleVoucher == null)
            {
                return new ParcelResponse
                {
                    IsAvailable = false,
                    Message = "Parcel not found."
                };
            }

            // Construct ParcelView once
            var parcelView = new ParcelView
            {
                SaleVoucherId = saleVoucher.Id,
                SupplierName = saleVoucher.Supplier.Name,
                TransporterName = saleVoucher.Transport.Name,
                SupplierBillNumber = saleVoucher.SupplierBillNumber,
                Status = ((ParcelStatusEnum)saleVoucher.Status).ToString(),
                Quantity = saleVoucher.SaleVoucherDetails.Sum(x=>x.Quantity),
                Products = string.Join(", ",
                           saleVoucher.SaleVoucherDetails.Where(d => d.Product != null).Select(d => d.Product.Name))
            };

            // Case: Opening parcel
            if (requestedStatus == ParcelStatusEnum.PackedAtLocation &&
                (saleVoucher.Status == (int)ParcelStatusEnum.InTransit || saleVoucher.Status == (int)ParcelStatusEnum.Transport || saleVoucher.Status == (int)ParcelStatusEnum.PackedAtLocation))
            {
                return new ParcelResponse
                {
                    SaleVoucher = parcelView,
                    IsAvailable = true,
                    Message = "Parcel is available for opening."
                };
            }

            // Case: Parcel status matches requested status
            if (saleVoucher.Status<(int)ParcelStatusEnum.Opened && saleVoucher.Status<=(int)requestedStatus)
            {
                return new ParcelResponse
                {
                    SaleVoucher = parcelView,
                    IsAvailable = true,
                    Message = requestedStatus switch
                    {
                        ParcelStatusEnum.InTransit => "Parcel is available for warehouse scanning.",
                        ParcelStatusEnum.Transport => "Parcel is available for packed at location scanning.",
                        ParcelStatusEnum.PackedAtLocation => "Parcel is available for open scanning.",
                        _ => "Parcel is available."
                    }
                };
            }

            // Parcel exists but status is not valid
            return new ParcelResponse
            {
                IsAvailable = false,
                Message = $"Parcel status is {parcelView.Status}."
            };
        }

        public async Task<bool> MoveSaleVoucherProductsToStockAsync(
         ParcelScanRequest parcelScanRequest,Guid createdBy,string currentUser)
        {
            if (parcelScanRequest == null ||
                parcelScanRequest.SaleVoucherId == null ||
                !parcelScanRequest.SaleVoucherId.Any())
                return false;

            try
            {
                // 1️⃣ Create DataTable for TVP
                var table = new DataTable();
                table.Columns.Add("Id", typeof(int));

                foreach (var id in parcelScanRequest.SaleVoucherId)
                {
                    table.Rows.Add(id);
                }

                // 2️⃣ Create SQL Parameter
                var parameter = new SqlParameter("@VoucherIds", table)
                {
                    TypeName = "dbo.VoucherIdTableType",
                    SqlDbType = SqlDbType.Structured
                };

                // 3️⃣ Execute Stored Procedure
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC MoveSaleVoucherProductsToStock @VoucherIds",
                    parameter);

            
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
