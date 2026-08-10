using System.ComponentModel.DataAnnotations;

namespace Textile.Core.Entities.DbEnitites
{
    public class StickerPrintSetting : DatabaseEntity<int>
    {
        public bool ShowSupplierCode { get; set; }
        public bool ShowCompanyShortName { get; set; }
        public bool ShowWholeSaleRate { get; set; }
        public bool ShowProductName { get; set; }
        public bool ShowPrintDate { get; set; }
        public bool ShowRetailRate { get; set; }
        public bool ShowBarcode { get; set; }
        public bool ShowBarcodeText { get; set; }

        [MaxLength(30)]
        public string CompanyShortName { get; set; } = "";

        public bool ApplyWholeSaleRateFormula { get; set; }

        [MaxLength(20)]
        public string? WholeSaleRatePrefix { get; set; }

        [MaxLength(20)]
        public string? WholeSaleRatePostfix { get; set; }

        public decimal WholeSaleRateAddAmount { get; set; }

        public ICollection<StickerPrintFieldSetting> FieldSettings { get; set; } = new List<StickerPrintFieldSetting>();
    }
}
