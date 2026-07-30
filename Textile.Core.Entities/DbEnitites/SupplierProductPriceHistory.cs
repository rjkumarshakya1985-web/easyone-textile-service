namespace Textile.Core.Entities.DbEnitites
{
    public class SupplierProductPriceHistory : DatabaseEntity<int>
    {
        public DateTime Date { get; set; }
        public Guid SupplierProductId { get; set; }
        public decimal PurchaseRate { get; set; }
        public decimal WholesaleRate { get; set; }
        public decimal RetailRate { get; set; }
        public bool IsDeleted { get; set; }
        public SupplierProduct SupplierProduct { get; set; }
    }
}
