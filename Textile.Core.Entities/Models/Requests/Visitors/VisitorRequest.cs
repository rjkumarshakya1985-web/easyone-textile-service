namespace Textile.Core.Entities.Models.Requests.Visitors
{
    public class VisitorRequest
    {
        public int? Id { get; set; }
        public Guid? CustomerId { get; set; }
        public string Name { get; set; }

        public string? Mobile { get; set; }

        public int CustomerType { get; set; }
        public DateTimeOffset VisitDate { get; set; }
        public string? Remarks { get; set; }
        public int? CityId { get; set; }
       
    }
}
