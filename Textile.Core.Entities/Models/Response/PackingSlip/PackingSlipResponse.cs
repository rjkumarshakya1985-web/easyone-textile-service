using Textile.Core.Entities.Enums;
using Textile.Core.Entities.Models.Response.Customers;
using Textile.Core.Entities.Models.Response.Visitors;

namespace Textile.Core.Entities.Models.Response.PackingSlip
{
    public class PackingSlipResponse
    {
        public int Id { get; set; }

        public Guid? SalesPersonId { get; set; }

        public required string SlipNumber { get; set; }

        public DateTime Date { get; set; }

        public VisitorResponse? Visitor { get; set; }

        public CustomerResponse ? CustomerResponse { get; set; }

        public int FinanceYearId { get; set; }

        public Guid UserId { get; set; }

        public int TotalQuantity { get; set; }

        public decimal TotalTaxableAmount { get; set; }

        public decimal TotalAmount { get; set; }
        public string Remarks { get; set; }

        public PackingSlipStatusEnum Status { get; set; }

        
        public List<PackingSlipItemResponse> Items { get; set; } = new();
    }
}
