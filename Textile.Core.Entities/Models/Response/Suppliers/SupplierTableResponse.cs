namespace Textile.Core.Entities.Models.Response.Suppliers
{
    public class SupplierTableResponse
    {
        public Guid Id { get; set; }

        public Guid? AgentId { get; set; }
        public string? AgentName { get; set; }
        public string Code { get; set; }
        public string? Name { get; set; }    

        public string UserName { get; set; }

        public string Password { get; set; }
        public string? Mobile { get; set; }
        public string? GstIn { get; set; }
        public string? PAN { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        public string? Address { get; set; }

        public bool IsActive { get; set; }
    }
}
