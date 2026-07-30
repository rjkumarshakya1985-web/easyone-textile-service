namespace Textile.Core.Entities.DbEnitites
{
    public class Visitor : BaseAuditDbEntity<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? CityId { get; set; }
        public string? Mobile { get; set; }
        public int CustomerType { get; set; }
        public Guid? CustomerId { get; set; }
        public DateTimeOffset VisitDate { get; set; }
        public string? Remarks { get; set; }
        public City City { get; set; }
        public DateTime? ModifiedOn { get; set; }    
        public Guid? ModifiedBy { get; set; }
        public string? ModifiedByUserName { get; set; }
      
        public Customer Customer { get; set; }
    }
}
