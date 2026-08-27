using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Enums;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Infrastructure.Helpers;
using Textile.Core.Interfaces.Data;
using Textile.Core.Managers.Commands.SaleVouchers;
using Textile.Core.Managers.Common.Exceptions;

namespace Textile.Core.Managers.Handlers.Commands.SaleVouchers
{

    public class UpdateSaleVoucherCommandHandler : IRequestHandler<UpdateSaleVoucherCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly TextileDbContext _context;

        public UpdateSaleVoucherCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, TextileDbContext context)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<int> Handle(UpdateSaleVoucherCommand command, CancellationToken cancellationToken)
        {
            var request = command.SaleVoucherRequest;

            var saleVoucherRepo = _unitOfWork.Repository<SaleVoucher, int>();
            var saleVoucherDetailRepo = _unitOfWork.Repository<SaleVoucherDetail, Guid>();
            var saleVoucherStatusRepo = _unitOfWork.Repository<SaleVoucherStatus, Guid>();
            var supplierRepo = _unitOfWork.Repository<Supplier, Guid>();
            var transportRepo = _unitOfWork.Repository<Transport, int>();
            var productRepo = _unitOfWork.Repository<SupplierProduct, Guid>();

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // ---------------------------
                // Validate Supplier
                // ---------------------------
                var supplier = await supplierRepo.GetSingleAsync(x => x.Id == request.SupplierId)
                               ?? throw new Exception("Supplier not found");

                // ---------------------------
                // Validate Transport
                // ---------------------------
                var transport = await transportRepo.GetSingleAsync(x => x.Id == request.TransportId)
                                  ?? throw new Exception("Transport not found");

                // ---------------------------
                // Fetch existing SaleVoucher
                // ---------------------------
                var saleVoucher = await saleVoucherRepo.GetByIdAsync(request.Id.Value)
                                    ?? throw new Exception("SaleVoucher not found");

                var hasStatusChanged = saleVoucher.Status != request.Status;

                if (hasStatusChanged)
                {
                    if (!CanChangeFromStatus(saleVoucher.Status))
                    {
                        throw new InvalidOperationException("Status cannot be changed after Opened, TallySynced or Cancelled.");
                    }

                    if (!IsAllowedUpdateStatus(request.Status))
                    {
                        throw new InvalidOperationException("Status can be changed only to In Transit, Transport, Packed at Location, Opened or Cancelled.");
                    }
                }

                // ---------------------------
                // Update fields
                // ---------------------------

                var selectedDate = request.Date.Date;

                // 2. Current system time
                var now = DateTime.Now;

                // 3. Combine date + current time
                var finalDateTime = new DateTime(
                    selectedDate.Year,
                    selectedDate.Month,
                    selectedDate.Day,
                    now.Hour,
                    now.Minute,
                    now.Second
                );


                saleVoucher.TransportId = request.TransportId;
              
                saleVoucher.NumberOfParcel = request.NumberOfParcel;
                saleVoucher.SupplierBillNumber = request.SupplierBillNumber;
                saleVoucher.Status = request.Status;
                saleVoucher.Remarks = request.Remarks;
                saleVoucher.Date = finalDateTime;
                saleVoucher.ModifiedBy = command.CurrentUserId;
                saleVoucher.AdditionalCharges =request.AdditionalCharges;
                saleVoucher.ModifiedByUserName = command.CurrentUserName;
                saleVoucher.ModifiedOn = DateTime.UtcNow;

                await saleVoucherRepo.UpdateAsync(saleVoucher);

                if (hasStatusChanged)
                {
                    await saleVoucherStatusRepo.AddAsync(new SaleVoucherStatus
                    {
                        SaleVoucherId = saleVoucher.Id,
                        Status = request.Status,
                        Date = DateTime.UtcNow,
                        CreatedBy = command.CurrentUserId,
                        CreatedByUserName = command.CurrentUserName,
                        CreatedOn = DateTime.UtcNow
                    });
                }

                // ---------------------------
                // Update SaleVoucherDetails
                // ---------------------------

                // 1. Delete existing details
                var existingDetails = await saleVoucherDetailRepo.GetAllAsync(x => x.SaleVoucherId == saleVoucher.Id);
                if (existingDetails.Any())
                {
                    await saleVoucherDetailRepo.DeleteAllAsync(existingDetails);
                }

                // 2. Add new/updated details
                var details = request.SaleVoucherDetails?.ToList();
                if (details != null && details.Count > 0)
                {
                    var productIds = details.Select(d => d.ProductId).ToList();
                    var products = await _context.SupplierProductViews.Where(x => productIds.Contains(x.Id)).ToListAsync();

                    var saleVoucherDetails = details.Select(detail =>
                    {
                        var product = products.FirstOrDefault(p => p.Id == detail.ProductId)
                                      ?? throw new Exception($"Product {detail.ProductId} not found");

                        return new SaleVoucherDetail
                        {
                            SaleVoucherId = saleVoucher.Id,
                            ProductId = product.Id,
                            PurchaseRate = product.PurchaseRate,
                            Quantity = detail.Quantity,
                            Discount = product.Discount,  /// gst column
                            WholeSalesMargin = supplier.WholeSalesMargin,
                            RetailMargin = supplier.RetailMargin,
                            MrpMargin = supplier.MrpMargin,
                            ManualWholeSaleRate = product.ManualWholeSaleRate,
                            WholeSaleRate = product.WholeSaleRate,
                            IsSupplierDiscount = detail.IsSupplierDiscount,
                            SupplierDiscount = detail.IsSupplierDiscount ? supplier.BillDiscount ?? 0 : 0,
                        };

                    }).ToList();

                    await saleVoucherDetailRepo.AddAsync(saleVoucherDetails);
                }

                await _unitOfWork.CommitTranscationAsync();
                return saleVoucher.Id;
            }
            catch (DbUpdateException ex) when (DbExceptionHelper.IsDuplicateKey(ex))
            {
                await _unitOfWork.RollbackTranscationAsync();

                throw new DuplicateEntityException(
                    $"Supplier bill number '{request.SupplierBillNumber}' already exists.");
            }
            catch
            {
                await _unitOfWork.RollbackTranscationAsync();
                throw;
            }
        }

        private static bool CanChangeFromStatus(int status)
        {
            return status != (int)ParcelStatusEnum.Opened &&
                status != (int)ParcelStatusEnum.TallySynced &&
                status != (int)ParcelStatusEnum.Cancelled;
        }

        private static bool IsAllowedUpdateStatus(int status)
        {
            return status == (int)ParcelStatusEnum.InTransit ||
                status == (int)ParcelStatusEnum.Transport ||
                status == (int)ParcelStatusEnum.PackedAtLocation ||
                status == (int)ParcelStatusEnum.Opened ||
                status == (int)ParcelStatusEnum.Cancelled;
        }
    }

}
