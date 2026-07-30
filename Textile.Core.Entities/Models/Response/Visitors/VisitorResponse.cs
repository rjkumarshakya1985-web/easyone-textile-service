using Textile.Core.Entities.Models.Response.Customers;

namespace Textile.Core.Entities.Models.Response.Visitors
{
    public class VisitorResponse
    {
        public int Id { get; set; }
        public Guid? CustomerId { get; set; }

        public CustomerResponse CustomerResponse { get; set; }
        public string Name { get; set; }

        public string? Mobile { get; set; }

        public int? CustomerType { get; set; }
        public DateTimeOffset VisitDate { get; set; }
        public string? Remarks { get; set; }

        public int? StateId { get; set; }
        public int? CityId { get; set; }
        public Guid? CreatedBy { get; set; }
        public string? CreatedByUserName { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid? ModifiedBy { get; set; }
        public string? ModifiedByUserName { get; set; }
        public DateTime? ModifiedOn { get; set; }


    }
}
