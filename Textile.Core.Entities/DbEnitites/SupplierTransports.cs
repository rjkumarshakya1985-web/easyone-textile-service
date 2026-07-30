namespace Textile.Core.Entities.DbEnitites
{
    public class SupplierTransport : DatabaseEntity<Guid>
    {
        
        public Guid SupplierId { get; set; }
        public int TransportId { get; set; }

        public bool IsActive { get; set; }

        // Navigation Properties
        public  Supplier Supplier { get; set; }
        public  Transport Transport { get; set; }
    }
}
