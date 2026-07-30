namespace Textile.Core.Entities.DbEnitites
{
    public class City : DatabaseEntity<int>
    {
        public string Name { get; set; }
        public int StateId { get; set; }
        public State State { get; set; }       

        public ICollection<Transport> Transports { get; set; }

        public ICollection<Supplier> Suppliers { get; set; }

        public ICollection<Customer> Customers { get; set; }
    }
}
