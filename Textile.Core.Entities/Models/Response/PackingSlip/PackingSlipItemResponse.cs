namespace Textile.Core.Entities.Models.Response.PackingSlip
{
    public class PackingSlipItemResponse
    {
        public int Id { get; set; } 
        public Guid StockId { get; set; }  
        public required string StockGroup { get; set; }
        public required string ProductName { get; set; }
        public required string BarCode { get; set; }
        public required int Qty { get; set; }       
        public decimal SaleRate { get; set; }
        public decimal GstPercent { get; set; }
        public decimal? DiscountPercent { get; set; }

        public decimal TaxableAmount { get; set; }
        public decimal DiscountAmount { get;set; }

        public decimal NetAmount { get; set; }
        public decimal GstAmount => NetAmount * GstPercent / 100;

        public decimal TotalAmount { get; set; }
       
        public int AvailableQty { get; set; }

    }
}
