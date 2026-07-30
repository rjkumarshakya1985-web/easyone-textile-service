using Textile.Core.Entities.Models.Requests.Masters;
using Textile.Core.Entities.Models.Response.StockGroups;

namespace Textile.Core.Interfaces.Services
{
    public interface IStockGroupService
    {
        Task<IEnumerable<StockGroupDto>> GetAllAsync();

        Task<StockGroupDto?> GetByIdAsync(int id);

        Task<bool> CreateAsync(StockGroupRequest request,Guid currentUserId,string currentUserName);

        Task<bool> UpdateAsync(StockGroupRequest request, Guid currentUserId, string currentUserName);
        Task<bool> DeleteAsync(int id);

        Task<bool> ToggleActiveAsync(int id);
    }


}
