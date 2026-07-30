namespace Textile.Core.Entities.DbEnitites
{
    public class SaleVoucherPrintDetail : DatabaseEntity<int>
    {
        public string CompanyName { get;set; }
        public string Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? Description { get; set; }
        public string? GstIn { get; set; }
    }
}
