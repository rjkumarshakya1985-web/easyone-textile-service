namespace Textile.Core.Entities.Models.Requests
{
    public class TransportRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CityId { get; set; }

        public int StateId { get; set; }
        public string? GstIn { get; set; }
        public int RegistrationType { get; set; }
        public int TransportType { get; set; }
        public string? Address { get; set; }
        public string? Pincode { get; set; }
        public string? Mobile { get; set; }
        public string? Email { get; set; }
        public string? Remarks { get; set; }

    }
}
