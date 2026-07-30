namespace Textile.Core.Entities.Dto
{
    public class SupplierProductPriceHistoryDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal  PurchaseRate { get; set; }
        public decimal  WholesaleRate { get; set; }
        public decimal  RetailRate { get; set; }
    }
}
