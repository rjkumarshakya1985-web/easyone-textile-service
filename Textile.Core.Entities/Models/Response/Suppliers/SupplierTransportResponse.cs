namespace Textile.Core.Entities.Models.Response.Suppliers
{
    public class SupplierTransportResponse
    {
        public Guid Id { get; set; }
        public Guid SupplierId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        
        public IEnumerable<TransportResponse> TransportResponses { get; set; }
    }
}
