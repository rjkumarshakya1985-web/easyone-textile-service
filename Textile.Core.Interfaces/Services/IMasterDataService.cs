using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Dto;

namespace Textile.Core.Interfaces.Services
{
    public interface IMasterDataService
    {
        public Task<IEnumerable<Gst>> GetGsts();
        public Task<List<LookupDto<int>>> GetTransportLookUp(int? transportType = null);

        public Task<List<LookupDto<Guid>>> GetHsnCodeLookUp();
    }
}
