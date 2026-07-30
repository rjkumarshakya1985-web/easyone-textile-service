

namespace Textile.Core.Entities.Dto
{
    public class LookupDto<TId>
    {
        public TId Id { get; set; } = default!;
        public string Name { get; set; } = string.Empty;
    }
}
