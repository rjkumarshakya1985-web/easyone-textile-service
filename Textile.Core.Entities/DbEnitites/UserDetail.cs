namespace Textile.Core.Entities.DbEnitites
{
    public class UserDetail : DatabaseEntity<Guid>
    {
        public Guid UserId { get; set; }
        public User User { get; set; }
        public int ? DepartmentId { get; set; }
        public Department Department { get; set; }
    }
}
