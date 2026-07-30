namespace Textile.Core.Entities.Models.Requests.Agents
{
    public class AgentRequest
    {
        public Guid? Id { get; set; }
        public string? Code { get; set; }
        public string Name { get; set; }

        public string? ContactPersonName { get; set; }

        public string? ContactPersonMobile { get; set; }
        public string? GSTIN { get; set; }
        public string? PAN { get; set; }
        public int? CityId { get; set; }
        public string? Email { get; set; }

        public string? Pincode { get; set; }
        public string? TallyLedgerName { get; set; }
        public string? Area { get; set; }

        public string? Address { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
