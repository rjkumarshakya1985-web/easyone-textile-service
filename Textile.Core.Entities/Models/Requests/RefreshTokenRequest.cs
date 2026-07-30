using Textile.Core.Entities.Enums;

namespace Textile.Core.Entities.Models.Requests
{
    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; }

        public ClientType ClientType { get; set; }
    }
}
