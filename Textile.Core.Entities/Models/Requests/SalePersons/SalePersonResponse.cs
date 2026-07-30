namespace Textile.Core.Entities.Models.Requests
{
    public class SalePersonResponse
    {
        public Guid? Id { get; set; }
        public required string Name { get; set; }
        public required string PhoneNumber { get; set; }
        public string? Email { get; set; }

        public int StateId { get; set; }    
        public required string StateName { get; set; }
        public int CityId { get; set; }

        public required string CityName { get; set; }
        public required string Address { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
