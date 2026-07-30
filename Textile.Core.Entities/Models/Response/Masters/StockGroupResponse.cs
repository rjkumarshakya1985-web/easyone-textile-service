using Textile.Core.Entities.Models.Response.StockGroups;

namespace Textile.Core.Entities.Models.Response.Masters
{
    public class StockGroupResponse
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public int GstValue { get; set; }
        public string Description { get; set; }

        public bool IsGstRule { get; set; }
        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public List<GstRuleDto> GstRuleDtos { get; set; }
    }

    
}
