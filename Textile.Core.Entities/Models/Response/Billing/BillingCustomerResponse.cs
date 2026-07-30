namespace Textile.Core.Entities.Models.Response.Billing
{
    public class BillingCustomerResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? GstIn { get; set; }

        public decimal? Discount { get; set; }
        public  string? Mobile { get; set; }

        public int? CustomerType { get; set; }
    }
}
