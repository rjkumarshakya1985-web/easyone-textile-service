using Textile.Core.Entities.Enums;

namespace Textile.Core.Entities.Models.Requests.Tally
{
    public class TallyNameRequest
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string TallyName { get; set; }
        public TallyType Type { get; set; }
    }
}
