namespace Textile.Core.Entities.DbEnitites
{
    public class BaseAuditDbEntity<TEntityId> : BaseAuditDbEntity_Created<TEntityId>
    {
        public Guid? ModifiedBy { get; set; }
        public string? ModifiedByUserName { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
