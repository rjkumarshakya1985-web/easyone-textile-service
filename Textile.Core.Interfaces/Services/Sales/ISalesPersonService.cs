using Textile.Core.Entities.Data;
using Textile.Core.Entities.Models.Requests;
using Textile.Core.Entities.Models.Response.SalePersons;


namespace Textile.Core.Interfaces.Services.Sales
{
    public interface ISalesPersonService
    {
        Task<TableResult<SalePersonResponse>> GetTableData(TableDataRequest req);
        Task<SalePersonResponse?> GetByIdAsync(Guid id);
        Task<bool> SaveAsync(SalePersonRequest request, Guid currentUserId, string currentUserName);

        Task<bool> DeleteAsync(Guid Id, Guid currentUserId, string currentUserName);

        Task<List<SalePersonResponse>> GetActiveSalesPerson();
    }
}
