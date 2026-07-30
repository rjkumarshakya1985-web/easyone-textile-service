namespace Textile.Core.Entities.Models.Requests.SaleVouchers
{
    public class SaleVoucherRequest
    {
        public int? Id { get; set; }
        public Guid? SupplierId { get; set; }
        public int TransportId { get; set; }
        public DateTime Date { get; set; }
        public int NumberOfParcel { get; set; }
        public string SupplierBillNumber { get; set; }
  
        public int Status { get; set; }
        public string? Remarks { get; set; }
      
        public decimal AdditionalCharges { get; set; }
        public List<SaleVoucherDetailRequest> SaleVoucherDetails { get; set; }

    }

    public class SaleVoucherDetailRequest
    {
        public Guid? Id { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
       
        public bool IsSupplierDiscount { get; set; }
    }
}
