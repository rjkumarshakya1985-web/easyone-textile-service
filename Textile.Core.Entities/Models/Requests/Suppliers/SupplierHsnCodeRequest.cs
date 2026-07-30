namespace Textile.Core.Entities.Models.Requests.Suppliers
{
    public class SupplierHsnCodeRequest
    {
        public Guid SupplierId { get; set; }
        public Guid HsnCodeId { get; set; }
        public int StockGroupId { get; set; }

    }
}
