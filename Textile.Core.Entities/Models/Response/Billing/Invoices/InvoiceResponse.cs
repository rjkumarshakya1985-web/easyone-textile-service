
using Textile.Core.Entities.Enums;

namespace Textile.Core.Entities.Models.Response.Billing.Invoices
{
    public class InvoiceResponse
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public Guid CustomerId { get; set; }

        public required string InvoiceNumber { get; set; }
        public required string CustomerName { get; set; }
        public string? GstIn { get; set; }

        public int TotalQuantity { get; set; }
        public decimal TotalAmount { get; set; }

        public InvoiceStatusEnum Status { get; set; }

        public List<InvoiceItemResponse> Items { get; set; } = new();
    }
}
