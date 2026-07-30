using Textile.Core.Entities.Enums;

namespace Textile.Core.Entities.Models.Response
{
    public class TransportResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CityId { get; set; }
        public string City { get; set; }

        public int StateId { get; set; }
        public string State { get; set; }
        public string? GstIn { get; set; }
        public int RegistrationType { get; set; }  // Regular,Compostition,Unregistered
        public TransportTypeEnum TransportType { get; set; }   // Purchase,Sales,Both
        public string? Address { get; set; }
        public string? PinCode { get; set; }
        public string? Mobile { get; set; }
        public string? Email { get; set; }
        public string? Remarks { get; set; }
    }
}
