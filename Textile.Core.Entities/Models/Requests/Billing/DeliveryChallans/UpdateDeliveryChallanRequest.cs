namespace Textile.Core.Entities.Models.Requests.Billing.DeliveryChallans
{
    public class UpdateDeliveryChallanRequest
    {
        public int Id { get; set; }
        public decimal DiscountPercent { get; set; }
        public required List<UpdateDeliverChallanItemRequest> Items { get; set; }
    }

    public class UpdateDeliverChallanItemRequest
    {
        public int? Id { get; set; }
        public Guid StockId { get; set; }

        public decimal SaleRate { get; set; }
        public int Qty { get; set; }

        public decimal GstPercent { get; set; }
        public decimal? DiscountPercent { get; set; }
    }
}
