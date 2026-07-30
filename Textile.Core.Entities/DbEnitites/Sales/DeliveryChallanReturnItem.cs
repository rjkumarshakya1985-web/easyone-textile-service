namespace Textile.Core.Entities.DbEnitites.Sales
{
    public class DeliveryChallanReturnItem : DatabaseEntity<int>
    {
        public int DeliveryChallanReturnId { get; set; }
        public int DeliveryChallanItemId { get; set; }
        public Guid? SalesPersonId { get; set; }
        public Guid StockId { get; set; }
       
        public int ReturnQty { get; set; }
    }
}
