using Textile.Core.Entities.Models.Requests.StockGroups;
using Textile.Core.Entities.Models.Response.StockGroups;

namespace Textile.Core.Interfaces.Services
{
    public interface IGstRuleService
    {
        Task<IEnumerable<GstRuleDto>> GetAllAsync();

        Task<GstRuleDto?> GetByIdAsync(int id);

        Task<bool> CreateAsync(GstRuleRequest request, Guid currentUserId, string currentUserName);

        Task<bool> UpdateAsync(GstRuleRequest request, Guid currentUserId, string currentUserName);
        Task<bool> DeleteAsync(int id);

    }
}
