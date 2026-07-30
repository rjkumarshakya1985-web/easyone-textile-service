namespace Textile.Core.Entities.Models.Response.Departments
{
    public class SubDepartmentsResponse
    {
        public int Id { get; set; }
        public int SubDepartmentId { get; set; }

        public string DepartmentName { get; set; }
        public string SubDepartmentName { get; set; }
    }
}
