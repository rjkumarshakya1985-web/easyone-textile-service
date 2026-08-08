namespace Textile.Core.Entities.Models.Response
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }

        public string RoleName { get; set; }

        public string UserName { get; set; }

        public string Name { get; set; }

        public bool IsDeveloper { get; set; }

        public bool IsLoginFailed { get; set; }

        public string Message { get; set; }
    }
}
