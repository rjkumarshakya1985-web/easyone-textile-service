using AutoMapper;
using MediatR;
using Textile.Core.Entities;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Models.Response.Suppliers.Print;
using Textile.Core.Interfaces.Data;
using Textile.Core.Managers.Query.SaleVouchers.Print;


namespace Textile.Core.Managers.Handlers.Query.SaleVouchers.Print
{
    public class GetSaleVoucherStickerPrintQueryHandler
     : IRequestHandler<GetSaleVoucherStickerPrintQuery, SaleVoucherPrintResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetSaleVoucherStickerPrintQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<SaleVoucherPrintResponse> Handle(
            GetSaleVoucherStickerPrintQuery request,
            CancellationToken cancellationToken)
        {
            var saleVoucherRepo = _unitOfWork.Repository<SaleVoucher, int>();
            var saleVoucherDetailRepo = _unitOfWork.Repository<SaleVoucherDetail, Guid>();
            var saleVoucherPrintRepo = _unitOfWork.Repository<SaleVoucherPrintDetail, int>();

            // 1️⃣ SaleVoucher with relations
            var saleVoucher = await saleVoucherRepo.GetByIdAsync(
                request.SaleVoucherId,
                x => x.Transport,
                x => x.Supplier.SubDepartment.Department,
                x => x.Supplier.City.State);

            if (saleVoucher is null)
                throw new KeyNotFoundException($"SaleVoucher not found. Id: {request.SaleVoucherId}");

            // 2️⃣ Details with Product (materialize once)
            var saleVoucherDetails = (await saleVoucherDetailRepo.GetAllAsync(
                    x => x.SaleVoucherId == saleVoucher.Id,
                    x => x.Product))
                .ToList();

            if (saleVoucherDetails.Count == 0)
                throw new KeyNotFoundException("SaleVoucher item not found for print.");

            // 3️⃣ BillingDetailPrints


            // 4️⃣ Print master data (fetch once, not full list)
            var billingInformation = (await saleVoucherPrintRepo.GetAllAsync())
                .FirstOrDefault()
                ?? throw new KeyNotFoundException("SaleVoucher print detail not found.");

            // 5️⃣ SupplierPrint
            var supplierPrint = new SupplierPrint
            {
                Name = saleVoucher.Supplier.Name,
                GstIn = saleVoucher.Supplier.GstIn,
                SupplierCode = saleVoucher.Supplier.City.Name.Substring(0, 1) + saleVoucher.Supplier.Code,
               Department= saleVoucher.Supplier.SubDepartment.Department.Name,
               SubDepartment=saleVoucher.Supplier.SubDepartment.Name
            };

            // 6️⃣ SaleVoucherPrint
            var saleVoucherPrint = new SaleVoucherPrint
            {
                Id = saleVoucher.Id,
                CompanyName = billingInformation.CompanyName,
                Address = string.Join(", ",
                    billingInformation.Address1,
                    billingInformation.Address2),
                InVoiceNo = saleVoucher.SupplierBillNumber,
                TransportName = saleVoucher.Transport?.Name,
                SupplierBillNumber = saleVoucher.SupplierBillNumber,
                Date = saleVoucher.Date,
                GstIn = billingInformation.GstIn,
                Discount = saleVoucher.Discount


            };

            var billingDetailPrints = saleVoucherDetails.Select(d =>
            {
                var lineAmount = d.PurchaseRate * d.Quantity;
                var discountAmount = d.SupplierDiscount == 0 ? 0 : (lineAmount * d.SupplierDiscount) / 100;
                var taxableamt = lineAmount - discountAmount;
                decimal cgst = 0;
                decimal sgst = 0;
                decimal igst = 0;
                decimal payableamt = 0;
                if (saleVoucher.Supplier.City.State.Code == "09")
                {
                    // Intra-state  discount is gst column
                    cgst = Math.Round(taxableamt * (d.Discount / 2) / 100, 2); 
                    sgst = Math.Round(taxableamt * (d.Discount / 2) / 100, 2);
                }
                else
                {
                    // Inter-state discount is gst column
                    igst = Math.Round(taxableamt * d.Discount / 100, 2);
                }
                payableamt = taxableamt + cgst + sgst + igst;
                return new BillingDetailPrint
                {
                    ProductName = d.Product.PrintName,
                    HsnCode = d.Product.HsnCode,
                    Qty = d.Quantity,
                    PurchasePrice = d.PurchaseRate,
                    Gst = d.Discount,
                    Total = lineAmount,
                    DiscountAmount = lineAmount - discountAmount,
                    CGST = cgst,
                    SGST = sgst,
                    IGST = igst,
                    PayableAmount = payableamt,
                    SupplierDiscount = d.SupplierDiscount
                };
            }).ToList();

            // 7️⃣ Sticker prints (Quantity-based expansion)
            var productStickerRecords = saleVoucherDetails
                .SelectMany(d =>
                    Enumerable.Range(0, d.Quantity)
                        .Select(_ => new StickerPrint
                        {
                            Barcode = d.Product.Barcode,
                            WholeSaleRate = "5" + (d.WholeSaleRate + 500),
                            RetailRate = d.RetailPrice.GenerateRandomPrefixedSuffixedNumber(),
                            MrpRate = d.MrpRate.ToString(".00"),
                            PurchaseRate = d.PurchaseRate,
                            SupplierCode = saleVoucher.Supplier.City.Name.Substring(0, 1) + saleVoucher.Supplier.Code,
                            Name = saleVoucher.Supplier.Name,
                            ProductName = d.Product.PrintName,
                            PrintDateString = DateTime.Now.ToString("ddMMyyyy")
                        }))
                .ToList();

            // 8️⃣ Response
            return new SaleVoucherPrintResponse
            {
                SaleVoucherPrint = saleVoucherPrint,
                SupplierPrint = supplierPrint,
                BillingDetailPrints = billingDetailPrints,
                StickerPrints = productStickerRecords
            };
        }
    }

}
