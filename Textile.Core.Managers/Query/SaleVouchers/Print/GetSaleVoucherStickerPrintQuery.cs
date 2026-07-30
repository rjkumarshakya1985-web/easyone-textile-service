using MediatR;
using Textile.Core.Entities.Models.Response.Suppliers.Print;

namespace Textile.Core.Managers.Query.SaleVouchers.Print
{
    public class GetSaleVoucherStickerPrintQuery : IRequest<SaleVoucherPrintResponse>
    {
        public int SaleVoucherId { get; set; }
        public GetSaleVoucherStickerPrintQuery(int id)
        {
            SaleVoucherId = id;
        }
    }
}
