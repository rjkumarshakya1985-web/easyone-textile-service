namespace Textile.Core.Entities.DbEnitites.Sales
{
    public class InvoiceDeliveryChallanMap : DatabaseEntity<Guid>
    {
        public int DeliveryChallanId { get; set; }
        public DeliveryChallan  DeliveryChallan { get; set; }
        public int InvoiceId { get; set; }
        public Invoice Invoice { get; set; }
    }
}
