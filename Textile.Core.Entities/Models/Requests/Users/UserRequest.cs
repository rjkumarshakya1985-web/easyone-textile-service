namespace Textile.Core.Entities.Models.Requests.Users
{
    public class UserRequest
    {
        public Guid ? Id { get; set; }
        public int RoleId { get; set; }

        public int? DepartmentId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeveloper { get; set; }
        public Guid? CreatedBy { get; set; }
        public string? CreatedByUserName { get; set; }
    }
}
