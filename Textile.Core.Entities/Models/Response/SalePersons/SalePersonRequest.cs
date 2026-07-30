

namespace Textile.Core.Entities.Models.Response.SalePersons
{
    public class SalePersonRequest
    {
        public Guid? Id { get; set; }
        public required string Name { get; set; }
        public required string PhoneNumber { get; set; }
        public string? Email { get; set; }
        public int CityId { get; set; }
        public required string Address { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
