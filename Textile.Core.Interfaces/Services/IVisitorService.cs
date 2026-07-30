using Textile.Core.Entities.Data;
using Textile.Core.Entities.Models.Requests;
using Textile.Core.Entities.Models.Response.Visitors;

namespace Textile.Core.Interfaces.Services
{
    public interface IVisitorService
    {
        Task<VisitorResponse> GetVisitoryById(int id);
        Task<VisitorResponse> GetVisitoryByMobile(string mobile);
        Task<TableResult<VisitorResponse>> GetTableData(TableDataRequest req);
    }
}
