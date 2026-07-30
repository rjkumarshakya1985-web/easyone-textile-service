using Textile.Core.Entities.Data;
using Textile.Core.Entities.Models.Requests;
using Textile.Core.Entities.Models.Response.DeliveryChallan;

namespace Textile.Core.Interfaces.Services.Sales
{
    public interface IDeliveryChallanService
    {
        Task<TableResult<DeliveryChallanListResponse>> GetTableData(TableDataRequest tableDataRequest,int financialYear);
    }
}
