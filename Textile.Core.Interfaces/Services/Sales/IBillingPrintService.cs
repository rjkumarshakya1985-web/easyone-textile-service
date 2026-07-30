using Textile.Core.Entities.Models.Response.BillingPrint;

namespace Textile.Core.Interfaces.Services.Sales
{
    public interface IBillingPrintService
    {
        public Task<PackingSlipPrintResponse?> GetPackingSlipPrint(int id);

        public Task<DeliveryChallanPrintResponse?> GetDeliveryChallanPrint(int id);
    }
}
