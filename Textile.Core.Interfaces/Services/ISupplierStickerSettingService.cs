using Textile.Core.Entities.Models.Requests.StickerPrint;
using Textile.Core.Entities.Models.Response.StickerPrint;

namespace Textile.Core.Interfaces.Services
{
    public interface ISupplierStickerSettingService
    {
        Task<SupplierStickerSizeSettingResponse> GetAsync(Guid supplierId);
        Task<bool> SaveAsync(Guid supplierId, SupplierStickerSizeSettingRequest request);
        Task ApplySizeAsync(Guid supplierId, StickerPrintSettingResponse stickerSetting);
    }
}
