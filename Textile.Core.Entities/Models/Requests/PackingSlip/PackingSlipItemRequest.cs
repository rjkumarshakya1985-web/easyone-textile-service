namespace Textile.Core.Entities.Models.Requests.PackingSlip
{
    public class PackingSlipItemRequest
    {
        public int? Id { get; set; }
        public Guid StockId { get; set; }

        public decimal SaleRate { get; set; }
        public int Qty { get; set; }

        public decimal GstPercent { get; set; }
        public decimal? DiscountPercent { get; set; }
    }
}
