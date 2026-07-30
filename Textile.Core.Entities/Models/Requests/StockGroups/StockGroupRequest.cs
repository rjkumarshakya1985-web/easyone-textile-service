namespace Textile.Core.Entities.Models.Requests.StockGroups
{
    public class StockGroupRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int GstValue { get; set; }
        public string Description { get; set; }
        public bool isGstRule { get; set; }
    }
}
