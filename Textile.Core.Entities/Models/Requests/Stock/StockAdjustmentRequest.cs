namespace Textile.Core.Entities.Models.Requests.Stock
{
    public class StockAdjustmentRequest
    {
        public Guid StockId { get; set; }
        public decimal SystemQty { get; set; }
        public decimal AdjustmentQty { get; set; }
        public decimal NewQty { get; set; }
        public int AdjustmentType { get; set; }
        public string? Reason { get; set; }
    }
}
