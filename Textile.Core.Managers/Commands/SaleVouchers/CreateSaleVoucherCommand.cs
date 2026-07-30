using MediatR;
using Textile.Core.Entities.Models.Requests.SaleVouchers;

namespace Textile.Core.Managers.Commands.SaleVouchers
{
    public class CreateSaleVoucherCommand : IRequest<int>
    {
        public SaleVoucherRequest SaleVoucherRequest { get; set; }
        public Guid CurrentUserId { get;set; }
        public string CurrentUserName { get; set; }
        public CreateSaleVoucherCommand(SaleVoucherRequest saleVoucherRequest,Guid userId,string username)
        {
            SaleVoucherRequest = saleVoucherRequest;
            CurrentUserId = userId;
            CurrentUserName = username;
        }
    }
}
