namespace Textile.Core.Entities.Models.Requests.Masters
{
    public class StockGroupRequest
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public int GstValue { get; set; }
        public bool IsGstRule { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }

}
