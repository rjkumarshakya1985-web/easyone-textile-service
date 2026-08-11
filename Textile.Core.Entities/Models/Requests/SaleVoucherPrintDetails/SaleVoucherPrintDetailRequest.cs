namespace Textile.Core.Entities.Models.Requests.SaleVoucherPrintDetails
{
    public class SaleVoucherPrintDetailRequest
    {
        public int Id { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string Address1 { get; set; } = string.Empty;
        public string? Address2 { get; set; }
        public string? Description { get; set; }
        public string? GstIn { get; set; }
    }
}
