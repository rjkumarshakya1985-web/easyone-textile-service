namespace Textile.Core.Entities.Models.Response.Agents
{
    public class AgentTableResponse
    {
        public Guid Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }

        public string? ContactPersonName { get; set; }

        public string? ContactPersonMobile { get; set; }
        public string? GSTIN { get; set; }
        public string? PAN { get; set; }

        public string? City { get; set; }
        public string? State { get; set; }

        public string? Pincode { get; set; }
        public string? TallyLedgerName { get; set; }
        public string? Area { get; set; }

        public string? Address { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
