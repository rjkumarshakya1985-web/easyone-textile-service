using MediatR;
using Textile.Core.Entities.Models.Response.SaleVouchers;

namespace Textile.Core.Managers.Query.SaleVouchers
{
    public class GetSaleVoucherDetailByIdQuery :IRequest<SaleVoucherResponse>
    {
        public int SaleVoucherId { get; set; }
        public GetSaleVoucherDetailByIdQuery(int saleVoucherId)
        {
            SaleVoucherId = saleVoucherId;
        }
    }
}
