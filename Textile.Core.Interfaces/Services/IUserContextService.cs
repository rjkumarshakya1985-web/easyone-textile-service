using Textile.Core.Entities.Enums;

namespace Textile.Core.Interfaces.Services
{
    public interface IUserContextService
    {
        Guid GetUserId();
        string GetUserName();
        RoleEnum GetUserRole();
    }
}
