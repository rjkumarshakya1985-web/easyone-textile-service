using Textile.Core.Entities.Models.Requests.Tally;

namespace Textile.Core.Interfaces.Services.Tally
{
    public interface ITallyNameService
    {
       Task<bool> UpdateBulkTallyNames(List <TallyNameRequest> items);
    }
}
