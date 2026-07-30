namespace Textile.Core.Entities.Models.Response.Departments
{
    public class DepartmentResponse
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; }
    }
}
