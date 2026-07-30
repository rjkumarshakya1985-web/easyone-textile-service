namespace Textile.Core.Entities.Models.Requests.Suppliers
{
    public class AddSupplierStockGroupRequest
    {
        public Guid SupplierId { get; set; }
        public int StockGroupId { get; set; }
    }
}
