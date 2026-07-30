namespace Textile.Core.Entities.DbEnitites
{
    public class SaleVoucher : BaseAuditDbEntity<int>
    {
        public Guid SupplierId { get; set; }
        public int  TransportId { get; set; }
        public DateTime Date { get; set; }  
        public int NumberOfParcel { get; set; }
        public string  SupplierBillNumber { get; set; }
        public int Status { get; set; }
        public string? Remarks { get;set; }

        public bool IsDeleted { get; set; }

        public decimal Discount { get; set; }

        public bool IsExported { get; set; }

        public decimal AdditionalCharges { get; set; }

        public string? LrNumber { get; set; }
        public DateTime? LrDate { get; set; }
        public Supplier Supplier { get; set; }

        public Transport Transport { get; set; }

        public ICollection<SaleVoucherStatus> SaleVoucherStatuses { get; set; }

        public ICollection<SaleVoucherDetail> SaleVoucherDetails { get; set; }
    }
}
