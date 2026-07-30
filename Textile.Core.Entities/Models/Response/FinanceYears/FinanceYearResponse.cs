namespace Textile.Core.Entities.Models.Response.FinanceYears
{
    public class FinanceYearResponse
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsClosed { get; set; }
    }
}
