namespace Textile.Core.Entities.Models.Response.Suppliers
{
    public class SupplierProductRateHistoryDTO
    {
        public DateTime Date { get; set; }
        public Guid SupplierProductId { get; set; }
        public decimal PurchaseRate { get; set; }
        public decimal WholesaleRate { get; set; }
        public decimal RetailRate { get; set; }         
    }
    
}
