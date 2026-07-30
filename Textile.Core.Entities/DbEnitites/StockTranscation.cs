namespace Textile.Core.Entities.DbEnitites
{
    
    public class StockTransaction : DatabaseEntity<int>
    {
        public Guid ProductId { get; set; }
        public int VoucherId { get; set; }

        public int VoucherType { get; set; }

        public string TransactionType { get; set; }
        public decimal Quantity { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime CreatedAt { get; set; }

        public SupplierProduct Product { get; set; }
    }
}
