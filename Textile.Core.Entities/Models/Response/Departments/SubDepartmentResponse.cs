namespace Textile.Core.Entities.Models.Response.Departments
{
    public class SubDepartmentResponse
    {
        public int Id { get; set; }

        public int DepartmentId { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }
    }
}
