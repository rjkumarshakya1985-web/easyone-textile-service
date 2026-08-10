using Textile.Core.Entities.Models.Requests.StickerPrint;
using Textile.Core.Entities.Models.Response.StickerPrint;

namespace Textile.Core.Interfaces.Services
{
    public interface IStickerPrintSettingService
    {
        Task<StickerPrintSettingResponse> GetAsync();
        Task<StickerPrintSettingResponse> GetForPrintAsync();
        Task<bool> SaveAsync(StickerPrintSettingRequest request, Guid currentUserId);
        string FormatWholeSaleRate(decimal wholeSaleRate, StickerPrintSettingResponse setting);
    }
}
