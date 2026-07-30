using Textile.Core.Entities.Data;
using Textile.Core.Entities.Enums;
using Textile.Core.Entities.Models.Requests;
using Textile.Core.Entities.Models.Response.PackingSlip;

namespace Textile.Core.Interfaces.Services.Sales
{
    public interface IPackingSlipService
    {

        Task<TableResult<PackingSlipListResponse>> GetTableData(TableDataRequest tableDataRequest);
        Task<bool> DeleteAsync(int Id, Guid currentUserId, string currentUserName);
        Task<PackingSlipResponse?> GetByIdAsync(int id);

        Task<PackingSlipResponse?> GetByPackingSlipNumberAsync(string number);

        Task<List<PackingSlipResponse>> GetPendingPackingSlipForBilling(Guid currentUserId, RoleEnum role, int? financeYearId);
        Task<PackingSlipResponse?> GetPackingSlipNumberForBillingAsync(string number, int financeYearId);

        Task<BillPackingSlipsResponse?> GetPackingSlipsNumberForBillingByVisitorIdAsync(int visitorId, int financeYearId);

    }
}
