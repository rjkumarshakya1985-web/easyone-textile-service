namespace Textile.Core.Entities.Models.Requests.Billing.Invoices
{
    public class DeliveryChallanToInvoiceRequest
    {
        public int FinYearId { get; set; }
        public decimal BillDiscount { get;set; }
        public required List<int> DeliveryChallanIds { get; set; }
    }
}
