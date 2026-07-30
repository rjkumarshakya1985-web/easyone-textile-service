namespace Textile.Core.Entities.Models.Requests.Suppliers
{
    public class SupplierStockGroupDeleteRequest
    {
        public Guid SupplierId { get; set; }
        public int StockGroupId { get; set; }
    }
}
