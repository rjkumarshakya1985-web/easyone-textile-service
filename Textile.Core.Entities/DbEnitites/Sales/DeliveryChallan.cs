namespace Textile.Core.Entities.DbEnitites.Sales
{
    public class DeliveryChallan : BaseAuditDbEntity<int>
    {

        public Guid UserId { get; set; }
        public string DeliveryChallanNumber { get; set; } = null!;
        public DateTime Date { get; set; }
        public int FinanceYearId { get; set; }
        public int? VisitorId { get; set; }
        public Guid? CustomerId { get; set; }
      
        public int TotalQuantity { get; set; }

        public int TotalReturnQty { get; set; }
        public int TotalEffectiveQty { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalGst { get; set; }
        public decimal TotalAmount { get; set; }

        public int Status { get; set; }

        public bool IsDeleted { get; set; }

        public decimal? DiscountPercent { get; set; }
        public FinanceYear FinanceYear { get; set; }
        public Visitor? Visitor { get; set; }

        public Customer? Customer { get; set; }
        public User User { get; set; }

        public ICollection<DeliveryChallanItem> DeliveryChallanItems { get; set; }

    }
}
