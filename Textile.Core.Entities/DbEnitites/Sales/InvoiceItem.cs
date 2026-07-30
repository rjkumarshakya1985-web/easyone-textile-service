namespace Textile.Core.Entities.DbEnitites.Sales
{
    public class InvoiceItem : DatabaseEntity<int>
    {
        public int InvoiceId { get; set; }
        public int? DeliveryChallanItemId { get; set; }
        public int? PackingSlipItemId { get; set; }
        public Guid? SalesPersonId { get; set; }
        public Guid StockId { get; set; }

        public decimal SaleRate { get; set; }
        public int Qty { get; set; }
     
        public decimal DiscountPercent { get; set; }
        public decimal GstPercent { get; set; }

        public bool IsDeleted { get; set; }
        public decimal TaxableAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal GstAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public Stock Stock { get; set; }

    }
}
