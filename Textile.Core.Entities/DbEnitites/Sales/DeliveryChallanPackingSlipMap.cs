namespace Textile.Core.Entities.DbEnitites.Sales
{
    public class DeliveryChallanPackingSlipMap : DatabaseEntity<int>
    {
        public int DeliveryChallanId { get; set; }
        public DeliveryChallan DeliveryChallan { get; set; }
        public int PackingSlipId { get; set; }
        public PackingSlip PackingSlip { get; set; }
    }
}
