using AutoMapper;
using MediatR;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Models.Response.Suppliers.Print;
using Textile.Core.Interfaces.Data;
using Textile.Core.Managers.Query.SaleVouchers.Print;

namespace Textile.Core.Managers.Handlers.Query.SaleVouchers.Print
{


    //public class GetStickerPrintQueryHandler
    //: IRequestHandler<GetStickerPrintQuery, IEnumerable<StickerPrint>>
    //{
    //    private readonly IUnitOfWork _unitOfWork;
    //    private readonly IMapper _mapper;

    //    public GetStickerPrintQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    //    {
    //        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    //        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    //    }

    //    public async Task<IEnumerable<StickerPrint>> Handle(
    //        GetStickerPrintQuery request,
    //        CancellationToken cancellationToken)
    //    {

    //        // 7️⃣ Sticker prints (Quantity-based expansion)
    //        var productStickerRecords = saleVoucherDetails
    //            .SelectMany(d =>
    //                Enumerable.Range(0, d.Quantity)
    //                    .Select(_ => new StickerPrint
    //                    {
    //                        Barcode = d.Product.Barcode,
    //                        WholeSaleRate = "5" + (d.WholeSaleRate + 500),
    //                        RetailRate = d.RetailPrice.GenerateRandomPrefixedSuffixedNumber(),
    //                        MrpRate = d.MrpRate.ToString(".00"),
    //                        PurchaseRate = d.PurchaseRate,
    //                        SupplierCode = saleVoucher.Supplier.City.Name.Substring(0, 1) + saleVoucher.Supplier.Code,
    //                        Name = saleVoucher.Supplier.Name,
    //                        ProductName = d.Product.Name,
    //                        PrintDateString = DateTime.UtcNow.ToString("ddMMyyyy")
    //                    }))
    //            .ToList();
    //    }
    //}

}
