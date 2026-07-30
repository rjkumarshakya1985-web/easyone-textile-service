using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Enums;
using Textile.Core.Interfaces.Services;

namespace Textile.Core.Infrastructure.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _config;

        public JwtService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateJwtToken(User user, ClientType clientType)
        {

            //var jwtSettings = Configuration.GetSection("JwtSettings");

            //// We use ONE key + issuer (shared)
            //var key = Encoding.UTF8.GetBytes(jwtSettings["Web:Key"]);
            //var issuer = jwtSettings["Web:Issuer"];

            //  Pick JWT config based on client type
            var jwtSection = _config.GetSection($"JwtSettings:{clientType}");
            var jwtKey = _config.GetSection($"JwtSettings:Web");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey["Key"]!));

            var creds = new SigningCredentials(
                key, SecurityAlgorithms.HmacSha256
            );

            var claims = new[]
            {
              new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
              new Claim(ClaimTypes.Name, user.UserName),
              new Claim(ClaimTypes.Role, user.Role.Name),
              new Claim("client_type", clientType.ToString()),
              new Claim("user_status", user.IsActive.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtSection["Issuer"],
                audience: jwtSection["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    Convert.ToInt32(jwtSection["TokenValidityInMinutes"])
                ),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }
    }

}
