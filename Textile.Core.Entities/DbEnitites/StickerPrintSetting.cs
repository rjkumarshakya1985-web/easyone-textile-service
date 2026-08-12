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

        public bool ApplyWholeSaleRateCode { get; set; }

        public int WholeSaleRateCodeDigitCount { get; set; }

        [MaxLength(10)]
        public string WholeSaleRateCode0 { get; set; } = "A";

        [MaxLength(10)]
        public string WholeSaleRateCode1 { get; set; } = "B";

        [MaxLength(10)]
        public string WholeSaleRateCode2 { get; set; } = "C";

        [MaxLength(10)]
        public string WholeSaleRateCode3 { get; set; } = "D";

        [MaxLength(10)]
        public string WholeSaleRateCode4 { get; set; } = "E";

        [MaxLength(10)]
        public string WholeSaleRateCode5 { get; set; } = "F";

        [MaxLength(10)]
        public string WholeSaleRateCode6 { get; set; } = "G";

        [MaxLength(10)]
        public string WholeSaleRateCode7 { get; set; } = "H";

        [MaxLength(10)]
        public string WholeSaleRateCode8 { get; set; } = "I";

        [MaxLength(10)]
        public string WholeSaleRateCode9 { get; set; } = "J";

        public ICollection<StickerPrintFieldSetting> FieldSettings { get; set; } = new List<StickerPrintFieldSetting>();
    }
}
