namespace Textile.Core.Entities.Models.Response.SaleVouchers
{
    public class SaleVoucherDto
    {
        public int Id { get; set; }
        public string? LrNumber { get; set; }
        public string? SupplierBillNumber { get; set; }
        public string? SupplierName { get; set; }
        public DateTime Date { get; set; }
        public bool IsExported { get; set; }
        public string? Department { get; set; }
        public string? ParcelStatus { get; set; }

    }
}
