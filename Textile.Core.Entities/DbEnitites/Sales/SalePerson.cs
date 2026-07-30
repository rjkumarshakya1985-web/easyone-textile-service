namespace Textile.Core.Entities.DbEnitites.Sales
{
    public class SalePerson : BaseAuditDbEntity<Guid>
    {
        public required string Name { get;set; }
        public required string PhoneNumber { get; set; }
        public string? Email { get; set; }
        public int CityId { get; set; }
        public required string Address { get;set; }
        public bool IsActive {get; set; }
        public bool IsDeleted { get; set; }
        public City City { get; set; }

    }
}
