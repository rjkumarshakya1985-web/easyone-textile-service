namespace Textile.Core.Entities.Models.Requests.StockGroups
{
    public class GstRuleRequest
    {
        public int? Id { get; set; }
        public int StockGroupId { get; set; }
        public decimal GstValue { get; set; }
        public decimal StartRange { get; set; }
        public decimal EndRange { get; set; }
        public bool IsDeleted { get; set; }

    }
}
