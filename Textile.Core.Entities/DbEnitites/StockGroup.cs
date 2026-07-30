namespace Textile.Core.Entities.DbEnitites
{
    public class StockGroup : BaseAuditDbEntity<int>
    {
        public string Name { get; set; }
        public string? TallyLedgerName { get; set; }
        public int GstValue { get; set; }
        public string? Description { get; set; }
        public bool IsGstRule { get; set; }
        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }
        public ICollection<SupplierProduct> SupplierProducts { get; set; }
        public ICollection<SupplierHsnCode> SupplierHsnCodes { get; set; }
        public ICollection<GstRule> GstRules { get; set; }
    }
}
