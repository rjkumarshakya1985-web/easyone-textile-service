using Textile.Core.Entities.Models.Response.Masters;

namespace Textile.Core.Entities.Models.Response.Suppliers
{
    public class SupplierStockGroupResponse
    {

        public Guid Id { get; set; }
        public Guid SupplierId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public IEnumerable<StockGroupResponse> StockGroupResponses { get; set; }
    }
}
