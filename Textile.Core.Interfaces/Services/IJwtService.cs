using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Enums;

namespace Textile.Core.Interfaces.Services
{
    public interface IJwtService
    {
        string GenerateJwtToken(User user,ClientType clientType);
        string GenerateRefreshToken();
    }
}
