using Textile.Core.Entities.Data;
using Textile.Core.Entities.Models.Requests;
using Textile.Core.Entities.Models.Requests.Suppliers;
using Textile.Core.Entities.Models.Response.Masters;
using Textile.Core.Entities.Models.Response.Suppliers;

namespace Textile.Core.Interfaces.Services
{
    public interface ISupplierStockGroupService
    {
        Task<bool> AssignSupplierStockGroup(AddSupplierStockGroupRequest addSupplierStockGroupRequest);
        Task<TableResult<SupplierStockGroupResponse>> GetSupplierStockGroupMappings(TableDataRequest tableDataRequest);
        Task<bool> SupplierStockGroupDelete(SupplierStockGroupDeleteRequest supplierStockGroup);

        Task<IEnumerable<StockGroupResponse>> GetSupplierOrphanStockGroups(Guid supplierId);
        Task<IEnumerable<StockGroupResponse>> SupplierStockGroups(Guid supplierId);
    }
}
