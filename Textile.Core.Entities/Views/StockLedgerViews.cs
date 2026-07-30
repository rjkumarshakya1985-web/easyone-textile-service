namespace Textile.Core.Entities.Views
{
    public class StockLedgerViews
    {
        public required string StockGroupName { get;set; }

        public required string ProductName { get; set; }
        public DateTime Date { get;set; }

        public int? BillNo { get; set; }
        public required string Description { get; set; }
        public decimal? In { get; set; }
        public decimal? Out { get; set; }
        public decimal Balance { get; set; }
    }
}
