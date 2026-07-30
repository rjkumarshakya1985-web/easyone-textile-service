using Textile.Core.Entities.Data;
using Textile.Core.Entities.Models.Requests;
using Textile.Core.Entities.Models.Response.Users;

namespace Textile.Core.Interfaces.Services
{
    public interface IUserService
    {
        Task<TableResult<UserResponse>> GetTableData(TableDataRequest DataRequest);

        Task<UserResponse?> GetByIdAsync(Guid id);
        Task<bool> ToggleActiveAsync(Guid id);
    }
}
