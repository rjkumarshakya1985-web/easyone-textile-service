using Microsoft.EntityFrameworkCore;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Enums;
using Textile.Core.Entities.Models.Requests.StickerPrint;
using Textile.Core.Entities.Models.Response.StickerPrint;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Interfaces.Services;

namespace Textile.Core.Managers.Services
{
    public class StickerPrintSettingService : IStickerPrintSettingService
    {
        private readonly TextileDbContext _context;

        public StickerPrintSettingService(TextileDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<StickerPrintSettingResponse> GetAsync()
        {
            return Map(await GetOrCreateAsync());
        }

        public async Task<StickerPrintSettingResponse> GetForPrintAsync()
        {
            var setting = await _context.StickerPrintSettings
                .Include(x => x.FieldSettings)
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .FirstOrDefaultAsync();

            return setting == null ? CreateDefaultResponse() : Map(setting);
        }

        public async Task<bool> SaveAsync(StickerPrintSettingRequest request, Guid currentUserId)
        {
            var currentUser = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == currentUserId);

            if (currentUser == null ||
                currentUser.RoleId != (int)RoleEnum.SuperAdmin ||
                !currentUser.IsDeveloper)
            {
                throw new UnauthorizedAccessException("Only developer admin can update sticker settings.");
            }

            var setting = await GetOrCreateAsync();

            setting.ShowSupplierCode = request.ShowSupplierCode;
            setting.ShowCompanyShortName = request.ShowCompanyShortName;
            setting.ShowWholeSaleRate = request.ShowWholeSaleRate;
            setting.ShowProductName = request.ShowProductName;
            setting.ShowPrintDate = request.ShowPrintDate;
            setting.ShowRetailRate = request.ShowRetailRate;
            setting.ShowBarcode = request.ShowBarcode;
            setting.ShowBarcodeText = request.ShowBarcodeText;
            setting.CompanyShortName = NormalizeShortName(request.CompanyShortName);
            setting.ApplyWholeSaleRateFormula = request.ApplyWholeSaleRateFormula;
            setting.WholeSaleRatePrefix = NormalizeOptional(request.WholeSaleRatePrefix);
            setting.WholeSaleRatePostfix = NormalizeOptional(request.WholeSaleRatePostfix);
            setting.WholeSaleRateAddAmount = request.WholeSaleRateAddAmount;
            SyncFieldSettings(setting, request.FieldSettings);

            await _context.SaveChangesAsync();
            return true;
        }

        public string FormatWholeSaleRate(decimal wholeSaleRate, StickerPrintSettingResponse setting)
        {
            var value = setting.ApplyWholeSaleRateFormula
                ? wholeSaleRate + setting.WholeSaleRateAddAmount
                : wholeSaleRate;

            return $"{setting.WholeSaleRatePrefix}{value:0.##}{setting.WholeSaleRatePostfix}";
        }

        private async Task<StickerPrintSetting> GetOrCreateAsync()
        {
            var setting = await _context.StickerPrintSettings
                .Include(x => x.FieldSettings)
                .OrderBy(x => x.Id)
                .FirstOrDefaultAsync();

            if (setting != null)
            {
                EnsureFieldSettings(setting);
                return setting;
            }

            setting = CreateDefaultEntity();
            _context.StickerPrintSettings.Add(setting);
            await _context.SaveChangesAsync();
            return setting;
        }

        private static StickerPrintSetting CreateDefaultEntity()
        {
            return new StickerPrintSetting
            {
                ShowSupplierCode = true,
                ShowCompanyShortName = true,
                ShowWholeSaleRate = true,
                ShowProductName = true,
                ShowPrintDate = true,
                ShowRetailRate = true,
                ShowBarcode = true,
                ShowBarcodeText = true,
                CompanyShortName = "SSBD",
                ApplyWholeSaleRateFormula = true,
                WholeSaleRatePrefix = "5",
                WholeSaleRatePostfix = null,
                WholeSaleRateAddAmount = 500,
                FieldSettings = CreateDefaultFieldSettings()
            };
        }

        private static StickerPrintSettingResponse CreateDefaultResponse()
        {
            return Map(CreateDefaultEntity());
        }

        private static StickerPrintSettingResponse Map(StickerPrintSetting setting)
        {
            return new StickerPrintSettingResponse
            {
                ShowSupplierCode = setting.ShowSupplierCode,
                ShowCompanyShortName = setting.ShowCompanyShortName,
                ShowWholeSaleRate = setting.ShowWholeSaleRate,
                ShowProductName = setting.ShowProductName,
                ShowPrintDate = setting.ShowPrintDate,
                ShowRetailRate = setting.ShowRetailRate,
                ShowBarcode = setting.ShowBarcode,
                ShowBarcodeText = setting.ShowBarcodeText,
                CompanyShortName = setting.CompanyShortName,
                ApplyWholeSaleRateFormula = setting.ApplyWholeSaleRateFormula,
                WholeSaleRatePrefix = setting.WholeSaleRatePrefix,
                WholeSaleRatePostfix = setting.WholeSaleRatePostfix,
                WholeSaleRateAddAmount = setting.WholeSaleRateAddAmount,
                FieldSettings = setting.FieldSettings
                    .OrderBy(x => x.SortOrder)
                    .Select(x => new StickerPrintFieldSettingResponse
                    {
                        FieldKey = x.FieldKey,
                        Label = x.Label,
                        IsVisible = x.IsVisible,
                        X = x.X,
                        Y = x.Y,
                        Width = x.Width,
                        Height = x.Height,
                        FontSize = x.FontSize,
                        FontWeight = x.FontWeight,
                        TextAlign = x.TextAlign,
                        SortOrder = x.SortOrder
                    })
                    .ToList()
            };
        }

        private static List<StickerPrintFieldSetting> CreateDefaultFieldSettings()
        {
            return new List<StickerPrintFieldSetting>
            {
                CreateField("supplierCode", "Supplier Code", true, 10, 8, 82, 24, 20, "800", "left", 1),
                CreateField("companyShortName", "Company Short Name", true, 113, 8, 74, 22, 20, "800", "center", 2),
                CreateField("wholeSaleRate", "Wholesale Rate", true, 162, 8, 128, 24, 20, "800", "right", 3),
                CreateField("productName", "Product Name", true, 42, 32, 216, 24, 18, "800", "center", 4),
                CreateField("printDate", "Print Date", true, 51, 59, 80, 18, 14, "400", "left", 5),
                CreateField("retailRate", "Retail Rate", true, 195, 59, 62, 18, 14, "400", "right", 6),
                CreateField("barcode", "Barcode", true, 51, 78, 188, 34, 14, "400", "center", 7),
                CreateField("barcodeText", "Barcode Text", true, 121, 113, 58, 14, 12, "400", "center", 8)
            };
        }

        private static StickerPrintFieldSetting CreateField(
            string fieldKey,
            string label,
            bool isVisible,
            decimal x,
            decimal y,
            decimal width,
            decimal height,
            int fontSize,
            string fontWeight,
            string textAlign,
            int sortOrder)
        {
            return new StickerPrintFieldSetting
            {
                FieldKey = fieldKey,
                Label = label,
                IsVisible = isVisible,
                X = x,
                Y = y,
                Width = width,
                Height = height,
                FontSize = fontSize,
                FontWeight = fontWeight,
                TextAlign = textAlign,
                SortOrder = sortOrder
            };
        }

        private static void EnsureFieldSettings(StickerPrintSetting setting)
        {
            var existingKeys = setting.FieldSettings.Select(x => x.FieldKey).ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (var field in CreateDefaultFieldSettings().Where(x => !existingKeys.Contains(x.FieldKey)))
            {
                setting.FieldSettings.Add(field);
            }
        }

        private static void SyncFieldSettings(StickerPrintSetting setting, List<StickerPrintFieldSettingRequest> requestFields)
        {
            var allowedDefaults = CreateDefaultFieldSettings();
            var allowedKeys = allowedDefaults.Select(x => x.FieldKey).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var requestedByKey = requestFields
                .Where(x => allowedKeys.Contains(x.FieldKey))
                .GroupBy(x => x.FieldKey, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(x => x.Key, x => x.First(), StringComparer.OrdinalIgnoreCase);

            EnsureFieldSettings(setting);

            foreach (var field in setting.FieldSettings)
            {
                var fallback = allowedDefaults.First(x => x.FieldKey.Equals(field.FieldKey, StringComparison.OrdinalIgnoreCase));
                var request = requestedByKey.GetValueOrDefault(field.FieldKey);

                field.Label = NormalizeOptional(request?.Label) ?? fallback.Label;
                field.IsVisible = request?.IsVisible ?? field.IsVisible;
                field.X = Clamp(request?.X ?? field.X, 0, 300);
                field.Y = Clamp(request?.Y ?? field.Y, 0, 134);
                field.Width = Clamp(request?.Width ?? field.Width, 10, 300);
                field.Height = Clamp(request?.Height ?? field.Height, 10, 134);
                field.FontSize = Math.Clamp(request?.FontSize ?? field.FontSize, 8, 32);
                field.FontWeight = NormalizeOptional(request?.FontWeight) ?? fallback.FontWeight;
                field.TextAlign = NormalizeTextAlign(request?.TextAlign) ?? fallback.TextAlign;
                field.SortOrder = request?.SortOrder ?? fallback.SortOrder;
            }
        }

        private static decimal Clamp(decimal value, decimal min, decimal max)
        {
            return Math.Min(Math.Max(value, min), max);
        }

        private static string? NormalizeTextAlign(string? value)
        {
            var normalized = NormalizeOptional(value)?.ToLowerInvariant();
            return normalized is "left" or "center" or "right" ? normalized : null;
        }

        private static string NormalizeShortName(string? value)
        {
            return value?.Trim() ?? "";
        }

        private static string? NormalizeOptional(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }
    }
}
