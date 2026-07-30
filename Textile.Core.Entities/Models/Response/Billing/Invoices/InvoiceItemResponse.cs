namespace Textile.Core.Entities.Models.Response.Billing.Invoices
{
    public class InvoiceItemResponse
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public Guid StockId { get; set; }
        public required string ProductCategory { get; set; }
        public required string ProductName { get; set; }

        public int Qty { get; set; }
        public decimal SaleRate { get; set; }
      
        public decimal Amount { get; set; }
    }
}
