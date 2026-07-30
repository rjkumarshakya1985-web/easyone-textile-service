namespace Textile.Core.Entities.Models.Requests.Departments
{
    public class DepartmentRequest
    {
        public int? DepartmentId { get; set; }
        public int ? SubDepartmentId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
