namespace Textile.Core.Entities.Models.Requests.SaleVouchers
{
    public class LrRequest
    {
        public int Id { get; set; }
        public required string LrNumber { get; set; }
        public required DateTime? LrDate { get; set; }

    }
}
