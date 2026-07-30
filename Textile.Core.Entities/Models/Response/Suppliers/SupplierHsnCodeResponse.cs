namespace Textile.Core.Entities.Models.Response.Suppliers
{

    public class SupplierHsnCodeResponse
    {
        public Guid Id { get; set; }
        public Guid SupplierId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public int StockGroupId { get; set; }
        public string StockGroupName { get; set; }

        public IEnumerable<HsnCodeResponse> HsnCodeResponses { get; set; }

       
    }
}
