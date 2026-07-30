namespace Textile.Core.Entities.Models.Requests.Suppliers
{
    public class SupplierTransportDeleteRequest
    {
        public Guid SupplierId { get; set; }
        public int TransportId { get; set; }
    }
}
