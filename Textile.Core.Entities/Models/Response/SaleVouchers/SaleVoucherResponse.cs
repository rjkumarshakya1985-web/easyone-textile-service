

using Textile.Core.Entities.Models.Response.Suppliers;

namespace Textile.Core.Entities.Models.Response.SaleVouchers
{
    public class SaleVoucherResponse
    {
        public int Id { get; set; }
        public Guid SupplierId { get; set; }

        public string SupplierName { get; set; }
        public int TransportId { get; set; }
        public string TransportName { get; set; }
        public DateTime Date { get; set; }
        public int NumberOfParcel { get; set; }
        public string SupplierBillNumber { get; set; }

        public decimal AdditionalCharges { get; set; }
        public int Status { get; set; }
        public string? Remarks { get; set; }
        public List<SaleVoucherDetailResponse> Details { get; set; }

        public SupplierTableResponse SupplierObj { get; set; }
    }

    public class SaleVoucherDetailResponse
    {
        public Guid Id { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public Guid  ProductId { get; set; }
        public string ProductName { get; set; }
        
        public int Quantity { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal WholeSalePrice { get; set; }

        public decimal RetailPrice { get; set; }

        public decimal MrpPrice { get; set; }

        public bool IsSupplierDiscount { get; set; }
    }
        
}
