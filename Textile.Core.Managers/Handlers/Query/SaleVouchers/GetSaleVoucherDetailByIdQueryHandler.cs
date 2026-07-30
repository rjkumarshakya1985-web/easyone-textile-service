using MediatR;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Models.Response.SaleVouchers;
using Textile.Core.Entities.Models.Response.Suppliers;
using Textile.Core.Interfaces.Data;
using Textile.Core.Managers.Query;
using Textile.Core.Managers.Query.SaleVouchers;

namespace Textile.Core.Managers.Handlers.Query.SaleVouchers
{
    public class GetSaleVoucherDetailByIdQueryHandler : IRequestHandler<GetSaleVoucherDetailByIdQuery, SaleVoucherResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetSaleVoucherDetailByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<SaleVoucherResponse> Handle(
        GetSaleVoucherDetailByIdQuery query,
        CancellationToken cancellationToken)
        {
            var saleVoucherRepository = _unitOfWork.Repository<SaleVoucher, int>();
            var saleVoucherDetailRepository = _unitOfWork.Repository<SaleVoucherDetail, Guid>();

            var saleVoucher = await saleVoucherRepository.GetByIdAsync(
                query.SaleVoucherId,x=>x.Transport,x=>x.Supplier);

            if (saleVoucher == null)
            {
                throw new KeyNotFoundException(
                    $"SaleVoucher with Id {query.SaleVoucherId} was not found.");
            }

            var saleVoucherDetails = await saleVoucherDetailRepository.GetAllAsync(
                x => x.SaleVoucherId == saleVoucher.Id,
                x => x.Product.StockGroup
            );

            var response = new SaleVoucherResponse
            {
                Id = saleVoucher.Id,
                SupplierId = saleVoucher.SupplierId,
                SupplierName = saleVoucher.Supplier.Name,
                TransportName = saleVoucher.Transport.Name,
                TransportId = saleVoucher.TransportId,
                Date = saleVoucher.Date,
                NumberOfParcel = saleVoucher.NumberOfParcel,
                SupplierBillNumber = saleVoucher.SupplierBillNumber,
                AdditionalCharges = saleVoucher.AdditionalCharges,
                Status = saleVoucher.Status,
                Remarks = saleVoucher.Remarks,
                SupplierObj =  new SupplierTableResponse
                {
                    Id = saleVoucher.Supplier.Id,
                    Name = saleVoucher.Supplier.Name,
                    Address = saleVoucher.Supplier.Address
                },
                Details = saleVoucherDetails.Select(d => new SaleVoucherDetailResponse
                {
                    Id = d.Id,
                    ProductId = d.Product.Id,
                    ProductName = d.Product.Name,
                    CategoryId = d.Product.StockGroup.Id,
                    CategoryName = d.Product.StockGroup.Name,
                    Quantity = d.Quantity,
                    PurchasePrice = d.PurchaseRate,
                    WholeSalePrice = d.WholeSaleRate,
                    RetailPrice = d.RetailPrice,
                    MrpPrice = d.MrpRate,
                    IsSupplierDiscount = d.IsSupplierDiscount
                }).ToList()
            };

            return response;
        }


    }
}
