using Microsoft.EntityFrameworkCore;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Models.Requests.StickerPrint;
using Textile.Core.Entities.Models.Response.StickerPrint;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Interfaces.Services;

namespace Textile.Core.Managers.Services
{
    public class SupplierStickerSettingService : ISupplierStickerSettingService
    {
        private readonly TextileDbContext _context;

        public SupplierStickerSettingService(TextileDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<SupplierStickerSizeSettingResponse> GetAsync(Guid supplierId)
        {
            var setting = await _context.SupplierStickerSettings
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.SupplierId == supplierId);

            if (setting == null)
            {
                return new SupplierStickerSizeSettingResponse
                {
                    HasCustomSize = false
                };
            }

            return new SupplierStickerSizeSettingResponse
            {
                StickerWidthMm = setting.StickerWidthMm,
                StickerHeightMm = setting.StickerHeightMm,
                HasCustomSize = true
            };
        }

        public async Task<bool> SaveAsync(Guid supplierId, SupplierStickerSizeSettingRequest request)
        {
            if (request.StickerWidthMm <= 0 || request.StickerHeightMm <= 0)
            {
                throw new ArgumentException("Sticker width and height must be greater than zero.");
            }

            var setting = await _context.SupplierStickerSettings
                .FirstOrDefaultAsync(x => x.SupplierId == supplierId);

            if (setting == null)
            {
                setting = new SupplierStickerSetting
                {
                    SupplierId = supplierId
                };
                _context.SupplierStickerSettings.Add(setting);
            }

            setting.StickerWidthMm = request.StickerWidthMm;
            setting.StickerHeightMm = request.StickerHeightMm;
            setting.UpdatedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task ApplySizeAsync(Guid supplierId, StickerPrintSettingResponse stickerSetting)
        {
            var size = await GetAsync(supplierId);
            stickerSetting.StickerWidthMm = size.StickerWidthMm;
            stickerSetting.StickerHeightMm = size.StickerHeightMm;
            stickerSetting.HasCustomSize = size.HasCustomSize;
        }
    }
}
