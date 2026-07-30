using Textile.Core.Entities.Models.Response.Agents;

namespace Textile.Core.Entities.Models.Response.Suppliers
{
    public class SupplierDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public string UserName { get; set; }
        public int DepartmentId { get; set; }
        public int SubDepartmentId { get; set; }
        public string Code { get; set; }
        public string? Name { get; set; }
        public string? Alias { get; set; }

        public string? GstIn { get; set; }
        public string? PAN { get; set; }

        public int? RegType { get; set; }
        public string? Address { get; set; }

        public int ? StateId { get; set; }
        public string StateCode { get; set; }
        public int? CityId { get; set; }

        public string? Mobile { get; set; }
        public string? Email { get; set; }
        public string? ContactPerson { get; set; }

        public string? BankName { get; set; }
        public string? Branch { get; set; }
        public string? AccountNumber { get; set; }
        public string? IFSC { get; set; }
        public string? UPID { get; set; }

        public int? CreditDays { get; set; }
        public decimal? CreditLimit { get; set; }

        public DateTime? GstRegistrationDate { get; set; }

        public string? MSMENumber { get; set; }
        public string? ECCNumber { get; set; }
        public string? Remarks { get; set; }

        public int DiscountType { get; set; }
        public int? TransactionType { get; set; }

        public decimal WholeSalesMargin { get; set; }
        public decimal RetailMargin { get; set; }

        public decimal MrpMargin { get; set; }

        public decimal? BillDiscount { get; set; }
        public decimal? PaymentDiscount { get; set; }
        public decimal? AnnualIncentive { get; set; }
        public string? PinCode { get; set; }

        public AgentTableResponse? AgentObj { get; set; }
        public List<int> TransportIds { get; set; } = new();

    }
}
