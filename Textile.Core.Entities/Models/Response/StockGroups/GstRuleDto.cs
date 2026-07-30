namespace Textile.Core.Entities.Models.Response.StockGroups
{
    public class GstRuleDto
    {
        public int Id { get; set; }
        public int StockGroupId { get; set; }
        public string? StockGroupName {get;set; }
        public decimal GstValue { get; set; }
        public int ApplyOrder { get; set; }
        public decimal StartRange { get; set; }
        public decimal? EndRange { get; set; }
        public bool IsDeleted { get; set; }
    }
}
