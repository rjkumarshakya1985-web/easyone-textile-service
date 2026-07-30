

namespace Textile.Core.Entities.DbEnitites.Sales
{
    public class PackingSlip : BaseAuditDbEntity<int>
    {
        public required string  SlipNumber { get; set; }
        public required DateTime Date { get; set; }
        public required int FinanceYearId   { get; set; }
        public  int? VisitorId { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? UserId { get; set; }
        public Guid? SalesPersonId { get; set; }
        public int TotalQuantity { get; set; }

        public decimal? DiscountPercent { get; set; }
        public  decimal TotalDiscount { get; set; }
        public decimal TotalGst { get; set; }
        public decimal TotalAmount { get; set; }
        public int Status { get; set; }
        public bool IsDeleted { get; set; }
        public string? Remarks { get; set; }

        public  FinanceYear FinanceYear { get; set; }
        public Visitor? Visitor { get; set; }
        public  User User { get; set; }

        public Customer ? Customer { get; set; }
        public SalePerson? SalesPerson { get; set; }

        public ICollection<PackingSlipItem> Items { get; set; } = new List<PackingSlipItem>();

    }
}
