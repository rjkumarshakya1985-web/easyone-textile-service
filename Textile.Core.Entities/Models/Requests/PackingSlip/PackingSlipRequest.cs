using Textile.Core.Entities.Models.Requests.PackingSlip;

namespace Textile.Core.Entities.Models.Requests.PackingSlips
{
    public class PackingSlipRequest
    {
        public int? Id { get; set; }
        public int? FinanceYearId { get; set; }

        public Guid? SalesPersonId { get; set; }
        public int? VisitorId { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? UserId { get; set; }

        public decimal DiscountPercent { get; set; }
        public string? Remarks { get; set; }

        public required List<PackingSlipItemRequest> Items { get; set; }
    }
}