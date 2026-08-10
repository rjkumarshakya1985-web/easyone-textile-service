using Textile.Core.Entities.Models.Response.StickerPrint;

namespace Textile.Core.Entities.Models.Response.Suppliers.Print
{
    public class SaleVoucherPrintResponse
    {
        public SaleVoucherPrint SaleVoucherPrint { get; set; }
        public SupplierPrint SupplierPrint { get; set; }
        public List<BillingDetailPrint> BillingDetailPrints { get; set; }
        public List<StickerPrint> StickerPrints { get; set; }
        public StickerPrintSettingResponse StickerSetting { get; set; }
    }

    public class SaleVoucherPrint
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string InVoiceNo { get; set; }
        public string? TransportName { get; set; }
        public string SupplierBillNumber { get; set; }
        public decimal Discount { get; set; }
        public DateTime Date { get; set; }
        public string GstIn { get; set; }
    }

    public class SupplierPrint
    {
        public string? Name { get; set; }
        public string? GstIn { get; set; }
        public string SupplierCode { get; set; }
        public string? Department { get; set; }
        public string? SubDepartment { get; set; }
    }

    public class BillingDetailPrint
    {
        public string ProductName { get; set; }
        public string HsnCode { get; set; }
        public int Qty { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal Gst { get; set; }
        public decimal Total { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal CGST { get; set; }
        public decimal SGST { get; set; }
        public decimal IGST { get; set; }
        public decimal PayableAmount { get; set; }
        public decimal SupplierDiscount { get; set; }
    }

    public class StickerPrint
    {
        public string Barcode { get; set; }
        public string RetailRate { get; set; }
        public decimal PurchaseRate { get; set; }
        public string WholeSaleRate { get; set; }
        public string MrpRate { get; set; }
        public string SupplierCode { get; set; }
        public string? Name { get; set; }
        public string ProductName { get; set; }
        public string PrintDateString { get; set; }
        public StickerPrintSettingResponse StickerSetting { get; set; }
    }
}
