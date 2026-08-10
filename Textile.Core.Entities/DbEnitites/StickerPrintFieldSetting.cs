using System.ComponentModel.DataAnnotations;

namespace Textile.Core.Entities.DbEnitites
{
    public class StickerPrintFieldSetting : DatabaseEntity<int>
    {
        public int StickerPrintSettingId { get; set; }

        [MaxLength(50)]
        public string FieldKey { get; set; } = "";

        [MaxLength(80)]
        public string Label { get; set; } = "";

        public bool IsVisible { get; set; }
        public decimal X { get; set; }
        public decimal Y { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public int FontSize { get; set; }

        [MaxLength(20)]
        public string FontWeight { get; set; } = "700";

        [MaxLength(20)]
        public string TextAlign { get; set; } = "left";

        public int SortOrder { get; set; }

        public StickerPrintSetting StickerPrintSetting { get; set; }
    }
}
