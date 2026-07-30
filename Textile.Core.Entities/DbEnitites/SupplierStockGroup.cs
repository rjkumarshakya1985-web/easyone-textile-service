namespace Textile.Core.Entities.DbEnitites
{

    public class SupplierStockGroup : DatabaseEntity<Guid>
    {
        public Guid SupplierId { get; set; }
        public int StockGroupId { get; set; }

        public bool IsActive { get; set; }

        public Supplier Supplier { get; set; }
        public StockGroup StockGroup { get; set; }
    }
}
