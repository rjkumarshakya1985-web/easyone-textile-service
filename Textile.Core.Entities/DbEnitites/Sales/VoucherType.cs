namespace Textile.Core.Entities.DbEnitites.Sales
{
    public class VoucherType : DatabaseEntity<int>
    {
        public string Name { get; set; } = "";
        public string Prefix { get; set; } = "";
        public int NumberLength { get; set; } = 5;
    }
}
