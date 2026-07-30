using Textile.Core.Entities.Models.Response.Billing.DeliveryChallans;

namespace Textile.Core.Interfaces.Services.Sales
{
    public interface IDeliveryChallanReturnService
    {
        public Task<DeliveryChallanReturnDetailResponse?> GetDeliveryChallanForReturn(string number, int finYearId);
        public Task<DeliveryChallanReturnDetailResponse?> GetDeliveryChallan(string number, int finYearId);
    }
}
