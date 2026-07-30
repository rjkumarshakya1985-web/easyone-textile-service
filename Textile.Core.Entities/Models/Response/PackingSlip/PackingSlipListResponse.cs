namespace Textile.Core.Entities.Models.Response.PackingSlip
{
    public class PackingSlipListResponse
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public required string VisitorName { get; set; }
        public required string SlipNumber { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalAmount { get; set; }
        public required int Status { get; set; }
    }
}
