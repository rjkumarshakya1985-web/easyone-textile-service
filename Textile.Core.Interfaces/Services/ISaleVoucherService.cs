using Textile.Core.Entities.Data;
using Textile.Core.Entities.Models.Requests;
using Textile.Core.Entities.Models.Requests.SaleVouchers;
using Textile.Core.Entities.Models.Response.SaleVouchers;

namespace Textile.Core.Interfaces.Services
{
    public interface ISaleVoucherService
    {
        Task<TableResult<SaleVoucherTableResponse>> GetTableData(TableDataRequest DataRequest, Guid? supplierId = null);
        Task<TableResult<SaleVoucherMobileResponse>> GetMobileTableData(TableDataRequest DataRequest, Guid? supplierId = null);
        Task<List<SaleVoucherMobileProductResponse>> GetMobileProductsAsync(int saleVoucherId, Guid? supplierId = null);
        Task<IEnumerable<SaleVoucherDto>> GetAllAsync();

        Task<SaleVoucherDto?> GetByIdAsync(int id);
        Task<bool> DeleteAsync(int id,Guid userId,string userName);

        Task<SaleVoucherDto> IsExport(int id);

        Task<bool> SaveLR(LrRequest request, Guid userId, string userName);
        Task<IEnumerable<SaleVoucherDto>> GetAllExportAsync();
    }
}
