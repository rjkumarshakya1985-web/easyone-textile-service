using System.ComponentModel.DataAnnotations;

namespace Textile.Core.Entities.DbEnitites
{
    public class AdminMenuSetting : DatabaseEntity<int>
    {
        [Required]
        [MaxLength(120)]
        public string MenuKey { get; set; }

        [Required]
        [MaxLength(150)]
        public string Label { get; set; }

        public bool IsEnabled { get; set; }
    }
}
