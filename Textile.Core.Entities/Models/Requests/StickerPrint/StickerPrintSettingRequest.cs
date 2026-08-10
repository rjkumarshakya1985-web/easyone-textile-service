namespace Textile.Core.Entities.Models.Requests.StickerPrint
{
    public class StickerPrintSettingRequest
    {
        public bool ShowSupplierCode { get; set; }
        public bool ShowCompanyShortName { get; set; }
        public bool ShowWholeSaleRate { get; set; }
        public bool ShowProductName { get; set; }
        public bool ShowPrintDate { get; set; }
        public bool ShowRetailRate { get; set; }
        public bool ShowBarcode { get; set; }
        public bool ShowBarcodeText { get; set; }
        public string CompanyShortName { get; set; } = "SSBD";
        public bool ApplyWholeSaleRateFormula { get; set; }
        public string? WholeSaleRatePrefix { get; set; }
        public string? WholeSaleRatePostfix { get; set; }
        public decimal WholeSaleRateAddAmount { get; set; }
        public List<StickerPrintFieldSettingRequest> FieldSettings { get; set; } = new();
    }

    public class StickerPrintFieldSettingRequest
    {
        public string FieldKey { get; set; } = "";
        public string Label { get; set; } = "";
        public bool IsVisible { get; set; }
        public decimal X { get; set; }
        public decimal Y { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public int FontSize { get; set; }
        public string FontWeight { get; set; } = "700";
        public string TextAlign { get; set; } = "left";
        public int SortOrder { get; set; }
    }
}
