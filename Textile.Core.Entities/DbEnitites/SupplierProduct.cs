namespace Textile.Core.Entities.DbEnitites
{
    public class SupplierProduct : BaseAuditDbEntity<Guid>
    {
        public Guid SupplierId { get; set; }
        public int StockGroupId { get; set; }

        public string Name { get; set; }
        public string? TallyLedgerName { get; set; }

        public string Alias { get; set; }

        public string PrintName { get; set; }

        public string HsnCode { get; set; }
        public string Barcode { get; set; }

        // Yes, No
        public bool GstApplicable { get; set; }

        // Goods, Services
        public int GSTNature { get; set; }

        // 1 = Taxable, 2 = Exempt, 3 = NilRated

        public int GSTTaxability { get; set; }
        public decimal PurchaseRate { get; set; }


        /// <summary>
        /// Gst Column
        /// </summary>
        public int Discount { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public decimal? ManualWholeSaleRate { get; set; }

        public Supplier? Supplier { get; set; }
        public StockGroup? StockGroup { get; set; }
     
        public ICollection<SaleVoucherDetail> SaleVoucherDetails { get; set; } 

        public ICollection<SupplierProductPriceHistory> SupplierProductPriceHistories { get; set; }
    }
}
