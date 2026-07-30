namespace Textile.Core.Entities.Models.Response.Stocks
{
    public class StockTableResponse
    {
        public Guid Id { get; set; }
        public required string SupplierName { get; set; }
        public Guid ProductId { get; set; }

        public required string Barcode { get; set; }

        public required string StockGroup { get; set; }
        public required string ProductName { get; set; }
        public decimal OpeningQty { get; set; }
        public decimal InwardQty { get; set; }
        public decimal OutwardQty { get; set; }
        public decimal ReservedQty { get; set; }
        public decimal DamagedQty { get; set; }
        public decimal TotalQty { get;  set; }
        public decimal AvailableQty { get;  set; }
        public decimal? PurchaseRate { get; set; }
        public decimal? Discount { get; set; }
        public decimal? WholeSaleMargin { get; set; }
        public decimal? RetailMargin { get; set; }
        public decimal? MrpMargin { get; set; }
        public decimal? WholeSaleRate { get; set; }
        public decimal? RetailRate { get; set; }
        public decimal? MrpRate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
