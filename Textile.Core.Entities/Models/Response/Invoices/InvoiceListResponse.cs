

namespace Textile.Core.Entities.Models.Response.Invoices
{
    public class InvoiceListResponse
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime Date { get; set; }
        public string CustomerName { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        public int Status { get; set; }
    }
}
