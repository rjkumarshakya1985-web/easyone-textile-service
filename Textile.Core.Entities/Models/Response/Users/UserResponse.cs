using Textile.Core.Entities.Enums;

namespace Textile.Core.Entities.Models.Response.Users
{
    public class UserResponse
    {
        public Guid Id { get; set; }
        public RoleEnum Role { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeveloper { get; set; }
        public UserDetailResponse? UserDetail { get; set; }
    }

    public class UserDetailResponse
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public int? DepartmentId { get; set; }
    }
}
