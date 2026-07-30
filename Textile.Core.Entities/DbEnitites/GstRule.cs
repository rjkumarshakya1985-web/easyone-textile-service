namespace Textile.Core.Entities.DbEnitites
{
    public class GstRule : BaseAuditDbEntity<int>
    {
        public int StockGroupId { get; set; }
        public decimal GstValue { get; set; }

        public int ApplyOrder { get; set; }
        public decimal StartRange { get; set; }
        public decimal? EndRange { get; set; }
        public bool IsDeleted { get; set; }

        public StockGroup StockGroup { get; set; }
    }
}
