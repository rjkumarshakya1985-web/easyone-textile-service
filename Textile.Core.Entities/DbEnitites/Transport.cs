namespace Textile.Core.Entities.DbEnitites
{
    public class Transport : DatabaseEntity<int>
    {
        public string Name { get; set; }
        public int CityId { get; set; }

        public string? GstIn { get; set; }
        public int RegistrationType { get; set; } // Regular,Compostition,Unregistered

        public int TransportType { get; set; }  // Purchase,Sales,Both

        public string? Address { get; set; }
        public string? Pincode { get; set; }
        public string? Mobile { get; set; }
        public string? Email { get; set; }
        public string? Remarks { get; set; }

        public City City { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public ICollection<SupplierTransport> SupplierTransports { get; set; }
        public ICollection<SaleVoucher> SaleVouchers { get; set; }
    }
}
