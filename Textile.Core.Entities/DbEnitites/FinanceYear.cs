namespace Textile.Core.Entities.DbEnitites
{
    public class FinanceYear : BaseAuditDbEntity<int>
    {
        public required string Name { get; set; }
        public required DateTime  StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsClosed { get; set; }
    }
}
