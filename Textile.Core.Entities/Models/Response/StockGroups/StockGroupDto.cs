namespace Textile.Core.Entities.Models.Response.StockGroups
{
    public class StockGroupDto
    {
            public  int Id { get; set; }
            public string Name { get; set; }
            public int GstValue { get; set; }
            public string? Description { get; set; }
            public bool isGstRule { get; set; }
            public bool IsActive { get; set; }
            public bool IsDeleted { get; set; }
    }
}
