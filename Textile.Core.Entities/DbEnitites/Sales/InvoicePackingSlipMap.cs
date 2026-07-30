namespace Textile.Core.Entities.DbEnitites.Sales
{
    public class InvoicePackingSlipMap : DatabaseEntity<int>
    {
        public int InvoiceId { get; set; }
        public Invoice Invoice { get; set; }
        public int PackingSlipId { get; set; }
        public PackingSlip PackingSlip { get; set; }
    }
}
