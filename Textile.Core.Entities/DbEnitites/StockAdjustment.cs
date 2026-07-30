namespace Textile.Core.Entities.DbEnitites
{
    public class StockAdjustment : BaseAuditDbEntity_Created<Guid>
    {
        public Guid StockId { get; set; }
        public decimal SystemQty { get; set; }
        public decimal AdjustmentQty { get; set; }
        public decimal NewQty { get; set; }
        public int AdjustmentType { get; set; }
        public string? Reason { get; set; }
        public bool IsDeleted { get; set; }
    }
}
