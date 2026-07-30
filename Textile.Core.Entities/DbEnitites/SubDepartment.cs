using System.ComponentModel.DataAnnotations;

namespace Textile.Core.Entities.DbEnitites
{
    public class SubDepartment : DatabaseEntity<int>
    {
        public int Id { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public Department Department { get; set; }

        public ICollection<Supplier> Suppliers { get; set; }
    }
}



