namespace Textile.Core.Entities.DbEnitites
{
    public class State : DatabaseEntity<int>
    {
        public string Name { get; set; }

        public string Code { get; set; }
        public ICollection<City> Cities { get; set; }
    }
}
