using Textile.Core.Entities.Enums;

namespace Textile.Core.Entities.Models.Response.SaleVouchers
{
    public class SaleVoucherTableResponse
    {
        public int Id { get; set; }

        public string? SupplierName { get; set; }
        public DateTime Date { get; set; }
        public string? TranportName { get; set; }
        public int NumberOfParcel { get; set; }
        public string? BillNumber { get; set; }

        public string? DepartmentName { get; set; }  

        public string? LrNumber { get; set; }
        public DateTime? LrDate { get; set; }

        public bool IsExported { get; set; }
        public ParcelStatusEnum ParcelStatus { get; set; }
        public DateTime StatusDate { get; set; }

        public required string ProductDetails { get; set; }
    }

    public class SaleVoucherMobileResponse
    {
        public int Id { get; set; }
        public string? SupplierName { get; set; }
        public DateTime Date { get; set; }
        public string? SupplierInvoice { get; set; }
        public string? CompanyName { get; set; }
        public string? Floor { get; set; }
        public ParcelStatusEnum ParcelStatus { get; set; }
        public DateTime StatusDate { get; set; }
        public int TotalQuantity { get; set; }
        public List<SaleVoucherMobileProductResponse> Products { get; set; } = new();
    }

    public class SaleVoucherMobileProductResponse
    {
        public string? CategoryName { get; set; }
        public string? ProductName { get; set; }
        public string? Description { get; set; }
        public string? Barcode { get; set; }
        public int Quantity { get; set; }
    }
}
