namespace Textile.Core.Entities.Models.Response.StickerPrint
{
    public class SupplierStickerSizeSettingResponse
    {
        public decimal? StickerWidthMm { get; set; }
        public decimal? StickerHeightMm { get; set; }
        public bool HasCustomSize { get; set; }
    }
}
