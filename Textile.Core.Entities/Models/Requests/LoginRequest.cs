using Textile.Core.Entities.Enums;

namespace Textile.Core.Entities.Models.Requests
{
    public class LoginRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public ClientType ClientType { get; set; }
    }
}
