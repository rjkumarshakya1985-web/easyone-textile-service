using MediatR;
using Textile.Core.Entities.Models.Requests.SaleVouchers;

namespace Textile.Core.Managers.Commands.SaleVouchers
{
   
    public class UpdateSaleVoucherCommand : IRequest<int>
    {
        public SaleVoucherRequest SaleVoucherRequest { get; set; }
        public Guid CurrentUserId { get; set; }
        public string CurrentUserName { get; set; }
        public UpdateSaleVoucherCommand(SaleVoucherRequest saleVoucherRequest, Guid userId, string username)
        {
            SaleVoucherRequest = saleVoucherRequest;
            CurrentUserId = userId;
            CurrentUserName = username;
        }
    }
}
