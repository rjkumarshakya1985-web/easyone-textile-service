namespace Textile.Core.Entities.Models.Requests.Billing.DeliveryChallan
{
    public class DeliveryChalanReturnRequest
    {
        public int DeliveryChallanId { get; set; }
        public int FinanceYearId { get; set; }
        public Guid CustomerId { get; set; }
      
        public List<DeliveryChallanReturnItemRequest> DeliveryChallanReturnItems { get; set; }
    }

    
    public class DeliveryChallanReturnItemRequest
    {
        public int DeliveryChallanItemId { get; set; }
        public Guid StockId { get; set; }
        public int ReturnQty { get; set; }
        public decimal SaleRate { get; set; }
      
     
    }

 
}
