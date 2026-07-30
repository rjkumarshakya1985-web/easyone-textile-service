namespace Textile.Core.Entities.DbEnitites
{
    public class SupplierHsnCode : DatabaseEntity<Guid>
    {
        public Guid SupplierId { get; set; }
        public Guid HsnCodeId { get; set; }
        public int StockGroupId { get; set; }
        public bool IsActive { get; set; }
        public Supplier Supplier { get; set; }
        public ProductHsnCode HsnCode { get; set; }

        public StockGroup StockGroup { get; set; }
    }
}
