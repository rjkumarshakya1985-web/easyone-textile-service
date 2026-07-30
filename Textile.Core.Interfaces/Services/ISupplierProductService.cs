using Textile.Core.Entities.Data;
using Textile.Core.Entities.Dto;
using Textile.Core.Entities.Enums;
using Textile.Core.Entities.Models.Requests;
using Textile.Core.Entities.Models.Requests.Suppliers;
using Textile.Core.Entities.Models.Response.Suppliers;

namespace Textile.Core.Interfaces.Services
{
    public interface ISupplierProductService
    {
        Task<TableResult<SupplierProductDto>> GetTableData(TableDataRequest DataRequest,Guid? supplierId=null);
        Task<IEnumerable<SupplierProductDto>> GetAllAsync();

        Task<SupplierProductDto?> GetByIdAsync(Guid id);

        Task<SupplierProductDto?> GetProductViewByIdAsync(Guid id);


        Task<bool> CreateAsync(SupplierProductRequest request, Guid currentUserId, string currentUserName);

        Task<bool> UpdateAsync(SupplierProductRequest request, Guid currentUserId, string currentUserName, RoleEnum role);
        Task<bool> DeleteAsync(Guid id);

        Task<bool> ToggleActiveAsync(Guid id);

        Task<string> FetchNextBarcodeNumber();

        Task<IEnumerable<SupplierProductPriceHistoryDto>>  GetProductPriceHistory(Guid productId);

        Task<bool> UpdateProductPriceHistoryAsync(List<int> salevoucherIds);
        Task<bool> DeleteProductPriceHistoryAsync(int historyId);
    }
}
