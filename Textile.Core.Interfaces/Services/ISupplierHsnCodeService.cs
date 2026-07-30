using Textile.Core.Entities.Data;
using Textile.Core.Entities.Models.Requests;
using Textile.Core.Entities.Models.Requests.Suppliers;
using Textile.Core.Entities.Models.Response;
using Textile.Core.Entities.Models.Response.Suppliers;

namespace Textile.Core.Interfaces.Services
{
    public interface ISupplierHsnCodeService
    {
        Task<bool> AssignSupplierHsnCode(SupplierHsnCodeRequest addSupplierHsnCodeRequest);
        Task<TableResult<SupplierHsnCodeResponse>> GetSupplierHsnCodeMappings(TableDataRequest tableDataRequest);
        Task<bool> SupplierHsnCodeDelete(SupplierHsnCodeRequest supplierStockGroup);

        Task<IEnumerable<HsnCodeResponse>> GetSupplierOrphanHsnCodes(Guid supplierId, int stockGroupId, string search);
       
        Task<IEnumerable<HsnCodeResponse>> GetSupplierStockGroupHsnCodes(Guid supplierId, int stockGroupId);
    }
}
