namespace Textile.Core.Entities.DbEnitites
{
    public class Permission : DatabaseEntity<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<RolePermission> RolePermissions { get; set; }
    }
}
