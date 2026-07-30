namespace Textile.Core.Entities.DbEnitites.Sales
{
    public class DeliveryChallanReturn : BaseAuditDbEntity_Created<int>
    {
        public int Id { get; set; }
        public required string ReturnNumber { get; set; }

        public DateTime ReturnDate { get; set; }
        public int DeliveryChallanId { get; set; }

        public int FinanceYearId { get; set; }
        public Guid CustomerId { get; set; }

        public int? VisitorId { get; set; }

       public bool IsDeleted { get; set; }

        
    }
}
