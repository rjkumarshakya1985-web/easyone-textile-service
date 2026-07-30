namespace Textile.Core.Entities.DbEnitites.Sales
{
    public class Invoice : BaseAuditDbEntity<int>
    {
        
        public Guid UserId { get; set; }
        public required string InvoiceNumber { get; set; }
        public DateTime Date { get; set; }
        public int FinanceYearId { get; set; }

        public int? VisitorId { get; set; }
        public Guid CustomerId { get; set; }
      
        public int TotalQuantity { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalGst { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal BillDiscount { get; set; }

        public decimal GrandTotal { get; set; }
        public int Status { get; set; }
        public bool IsDeleted { get; set; }
        public Customer Customer { get; set; }

        public List<InvoiceItem> InvoiceItems { get; set; } 

    }
}
