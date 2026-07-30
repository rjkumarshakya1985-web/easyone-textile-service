using Textile.Core.Entities.Models.Requests.Billing.Invoices;
using Textile.Core.Entities.Models.Response.Billing.DeliveryChallans;

namespace Textile.Core.Interfaces.Services.Sales
{
    public interface IDeliveryChallanToInvoiceService
    {
        Task<DeliverChallanToInvoiceResponse> GetDeliveryChallanForInvoiceByNumber(string number,int finYearId);
       
    }
}
