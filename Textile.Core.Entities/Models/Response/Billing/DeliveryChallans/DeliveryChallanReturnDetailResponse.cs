

using Textile.Core.Entities.Enums;

namespace Textile.Core.Entities.Models.Response.Billing.DeliveryChallans
{
    public class DeliveryChallanReturnDetailResponse
    {
        public int DeliveryChallanId { get; set; }
        public DateTime DeliveryChallanDate { get; set; }
        public Guid CustomerId { get; set; }
        public required string CustomerName { get; set; }

        public int CustomerType { get; set; }

        public int TotalQuantity { get; set; }
        public decimal TotalAmount { get; set; }

        public decimal? DiscountPercent { get; set; }

        public decimal TotalTaxableAmount { get; set; }

        public DeliveryChallanStatusEnum Status { get; set; }

        public List<DeliveryChallanReturnDetailItem> Items { get; set; } = new();

    }

    public class DeliveryChallanReturnDetailItem
    {
        public int DeliveryChallanItemId { get; set; }
        public Guid StockId { get; set; }

        public required string Barcode { get; set; }
        public required string ProductCategory { get; set; }
        public required string ProductName { get; set; }

        public decimal SaleRate { get; set; }
        public int Qty { get; set; }
        public int Returned { get; set; }

        public int Balance { get; set; }

        public int ReturnQty { get; set; }

        public decimal TaxableAmount { get; set; }

        public decimal DiscountPercent { get; set; }
        public decimal DiscountAmount { get; set; }


        public decimal NetAmount { get; set; }

        public decimal GstPercent { get; set; }

        public decimal Amount { get; set; }

        public int StockQty { get; set; }
    }
}
