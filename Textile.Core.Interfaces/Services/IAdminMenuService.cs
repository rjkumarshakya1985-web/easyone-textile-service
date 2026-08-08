using Textile.Core.Entities.Models.Requests.AdminMenu;
using Textile.Core.Entities.Models.Response.AdminMenu;

namespace Textile.Core.Interfaces.Services
{
    public interface IAdminMenuService
    {
        Task<List<AdminMenuSettingResponse>> GetAsync();
        Task<bool> SaveAsync(AdminMenuSettingRequest request, Guid currentUserId);
    }
}
