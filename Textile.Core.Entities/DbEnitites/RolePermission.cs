namespace Textile.Core.Entities.DbEnitites
{
    public class RolePermission : DatabaseEntity<int>
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public int PermissionId { get; set; }

        public Role Role { get; set; }
        public Permission Permission { get; set; }
    }
}
