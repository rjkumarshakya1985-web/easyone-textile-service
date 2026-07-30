using Textile.Core.Entities.Enums;
using Textile.Core.Entities.Models.Response.Customers;

namespace Textile.Core.Entities.Models.Response.Billing.DeliveryChallans
{
    public class DeliverChallanToInvoiceResponse
    {
        public int DeliveryChallanId { get; set; }
        public required string DeiliverChallanNo { get;set; }
        public int Quantity { get; set; }
        public int ReturnQty { get; set; }
        public int AvailableInvoiceQty { get; set; }
        public decimal TotalAmount { get; set; }

        public DeliveryChallanStatusEnum DeliveryChallanStatusEnum { get;set; }
        public CustomerResponse Customer { get; set; }
    }
}
