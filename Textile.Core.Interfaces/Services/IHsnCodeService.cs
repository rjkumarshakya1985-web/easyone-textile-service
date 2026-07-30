using Textile.Core.Entities.Data;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Models.Requests;

namespace Textile.Core.Interfaces.Services
{
    public interface IHsnCodeService
    {
        Task<TableResult<ProductHsnCode>> GetTableData(TableDataRequest DataRequest);
        Task<IEnumerable<ProductHsnCode>> GetAllAsync();

        Task<ProductHsnCode?> GetByIdAsync(Guid id);

        Task<bool> CreateAsync(HsnCodeRequest request, Guid currentUserId, string currentUserName);

        Task<bool> UpdateAsync(HsnCodeRequest request, Guid currentUserId, string currentUserName);
        Task<bool> DeleteAsync(Guid id);
    }
}
