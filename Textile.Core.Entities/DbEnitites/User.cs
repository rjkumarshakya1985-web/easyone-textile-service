using System.ComponentModel.DataAnnotations;

namespace Textile.Core.Entities.DbEnitites
{
    public class User : BaseAuditDbEntity<Guid>
    {
        [Required]
        public int RoleId { get; set; }

        [Required]
        [MaxLength(100)]
        public string UserName { get; set; }

        [Required]
        [MaxLength(255)]
        public string Password { get; set; }

        [MaxLength(150)]
        public string? Email { get; set; }

        [MaxLength(50)]
        public string? Phone { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeveloper { get; set; }

        // Navigation Property
        public Role Role { get; set; }

        public UserDetail UserDetail { get; set; }

        public ICollection<RefreshToken> RefreshTokens { get; set; }

        public ICollection<Supplier> Suppliers { get; set; }
    }

}
