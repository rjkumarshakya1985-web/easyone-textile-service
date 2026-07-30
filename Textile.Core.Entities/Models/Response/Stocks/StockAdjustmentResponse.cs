namespace Textile.Core.Entities.Models.Response.Stocks
{
    public class StockAdjustmentResponse
    {
        public StockTableResponse Stock { get; set; }
        public List<StockAdjustmentDetailsResponse> Adjustments { get; set; }
    }

    public class StockAdjustmentDetailsResponse
    {
        public Guid Id { get; set; }
        public Guid StockId { get; set; }
        public decimal SystemQty { get; set; }
        public decimal AdjustmentQty { get; set; }
        public decimal NewQty { get; set; }
        public int AdjustmentType { get; set; }
        public string? Reason { get; set; }
        public DateTime CreatedOn { get; set; }
        public required string CreatedByUserName { get; set; }
    }
}