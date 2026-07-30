namespace Textile.Core.Entities.Models.Response.BillingPrint
{
    public class DeliveryChallanPrintResponse
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string SlipNo { get; set; }

        public string CashierName { get; set; }  

        public int TotalQuantity { get; set; }

        public decimal TotalTaxableAmount { get; set; }

        public decimal TotalAmount { get; set; }

        public CustomerPrintResponse Customer { get; set; }

        public CompanyDetailResponse CompanyDetail { get; set; }

        public List<PackingSlipPrintItemResponse> Items { get; set; } = new List<PackingSlipPrintItemResponse>();
    }
}
