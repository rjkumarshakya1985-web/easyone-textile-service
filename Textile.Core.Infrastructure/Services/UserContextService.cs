using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Textile.Core.Entities.Enums;
using Textile.Core.Interfaces.Services;

namespace Textile.Core.Infrastructure.Services
{


    public class UserContextService : IUserContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContextService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid GetUserId()
        {
            var claim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)
                        ?? _httpContextAccessor.HttpContext?.User?.FindFirst(JwtRegisteredClaimNames.Sub);

            if (claim == null)
                throw new Exception("UserId not found in token");

            return Guid.Parse(claim.Value);
        }

        public string GetUserName()
        {
            var claim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name);
            return claim?.Value ?? throw new Exception("UserName not found in token");
        }

        public RoleEnum GetUserRole()
        {
            var claim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role);
            if (claim == null)
                throw new Exception("Role not found");

            if (!Enum.TryParse<RoleEnum>(claim.Value, out var role))
                throw new Exception("Invalid role");

            return role;
        }
    }

}
