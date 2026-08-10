using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Textile.Core.Entities;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Models.Response.Suppliers.Print;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Interfaces.Data;
using Textile.Core.Interfaces.Services;

namespace Textile.Core.Managers.Services
{
    public class PrintService : IPrintService
    {
        private readonly TextileDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IStickerPrintSettingService _stickerPrintSettingService;

        public PrintService(
            IUnitOfWork unitOfWork,
            TextileDbContext context,
            IStickerPrintSettingService stickerPrintSettingService)
        {

            this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _stickerPrintSettingService = stickerPrintSettingService ?? throw new ArgumentNullException(nameof(stickerPrintSettingService));
        }
        public async Task<StickerPrint> GetStickerByProduct(Guid id, bool isSaleVoucher = false)
        {
            var stickerSetting = await _stickerPrintSettingService.GetForPrintAsync();

            if (isSaleVoucher)
            {
                var saleVoucherDetailRepo = _unitOfWork.Repository<SaleVoucherDetail, Guid>();
                var saleVoucherRepo = _unitOfWork.Repository<SaleVoucher, int>();

                var detail = await saleVoucherDetailRepo.GetByIdAsync(
                    id,
                    x => x.Product);

                if (detail == null)
                    throw new Exception("Sale voucher detail not found");

                var saleVoucher = await saleVoucherRepo.GetByIdAsync(
                    detail.SaleVoucherId,
                    x => x.Transport,
                    x => x.Supplier.City);

                return new StickerPrint
                {
                    Barcode = detail.Product.Barcode,
                    WholeSaleRate = _stickerPrintSettingService.FormatWholeSaleRate(detail.WholeSaleRate, stickerSetting),
                    RetailRate = detail.RetailPrice
                        .GenerateRandomPrefixedSuffixedNumber(),
                    MrpRate = detail.MrpRate.ToString("0.00"),
                    PurchaseRate = detail.PurchaseRate,
                    SupplierCode = saleVoucher.Supplier.City.Name.Substring(0, 1)
                                    + saleVoucher.Supplier.Code,
                    Name = saleVoucher.Supplier.Name,
                    ProductName = detail.Product.Name,
                    PrintDateString = DateTime.UtcNow.ToString("ddMMyyyy"),
                    StickerSetting = stickerSetting
                };
            }
            else
            {

                var supplierProduct = await _context.SupplierProductViews
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.Id == id);

                if (supplierProduct == null)
                    throw new Exception("Supplier product not found");

                var supplierRepo = _unitOfWork.Repository<Supplier, Guid>();
                var supplier = await supplierRepo.GetByIdAsync(
                    supplierProduct.SupplierId,
                    x => x.City.State);

                return new StickerPrint
                {
                    Barcode = supplierProduct.Barcode,
                    WholeSaleRate = _stickerPrintSettingService.FormatWholeSaleRate(supplierProduct.WholeSaleRate, stickerSetting),
                    RetailRate = supplierProduct.RetailPrice?
                        .GenerateRandomPrefixedSuffixedNumber(),
                    MrpRate = supplierProduct.MrpRate?.ToString("0.00") ?? "0.00",
                    PurchaseRate = supplierProduct.PurchaseRate,
                    SupplierCode = supplier.City.Name.Substring(0, 1)
                                    + supplier.Code,
                    Name = supplier.Name,
                    ProductName = supplierProduct.Name,
                    PrintDateString = DateTime.UtcNow.ToString("ddMMyyyy"),
                    StickerSetting = stickerSetting
                };


            }
        }

        public async Task<StickerPrint> GetStickerBySaleVoucherDetail(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
