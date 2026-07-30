namespace Textile.Core.Entities.DbEnitites.Sales
{
    public class VoucherNumberSeries : DatabaseEntity<int>
    {
        public required int VoucherType { get; set; }
        public required int FinanceYearId { get; set; }
        public required int CurrentNumber { get; set; }
        public string Prefix { get; set; } = "";
        public int NumberLength { get; set; } = 4;

        public  FinanceYear FinanceYear { get; set; }
    }
}
