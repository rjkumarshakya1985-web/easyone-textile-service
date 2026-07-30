namespace Textile.Core.Entities.Models.Response.DeliveryChallan
{
    public class DeliveryChallanListResponse
    {
        public int Id { get; set; }
        public string? ChallanNumber { get; set; }
        public DateTime Date { get; set; }
        public string? CustomerName { get; set; }
        public int Quantity { get; set; }
        public int ReturnQuantity { get; set; }
        public int BalanceQuantity { get; set; }
        public decimal TotalAmount { get; set; }
        public int Status { get; set; }
    }
}
