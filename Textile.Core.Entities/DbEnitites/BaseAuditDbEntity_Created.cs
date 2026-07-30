using System.ComponentModel.DataAnnotations;

namespace Textile.Core.Entities.DbEnitites
{
    public class BaseAuditDbEntity_Created<TEntityId> : DatabaseEntity<TEntityId>
    {
        public Guid CreatedBy { get; set; }

        [MaxLength(255)]
        [Required(AllowEmptyStrings =false)]
        public string CreatedByUserName { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
