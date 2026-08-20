namespace Textile.Core.Entities.DbEnitites
{
    public class SupplierStickerSetting : DatabaseEntity<int>
    {
        public Guid SupplierId { get; set; }
        public decimal StickerWidthMm { get; set; }
        public decimal StickerHeightMm { get; set; }
        public DateTime UpdatedOn { get; set; }
        public Supplier Supplier { get; set; }
    }
}
