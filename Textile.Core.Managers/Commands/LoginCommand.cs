using MediatR;
using Textile.Core.Entities.Enums;
using Textile.Core.Entities.Models.Response;

namespace Textile.Core.Managers.Commands
{
    public class LoginCommand : IRequest<LoginResponse>
    {
        public string UserName { get; }
        public string Password { get; }

        public ClientType ClientType { get; }

        public LoginCommand(string userName, string password,ClientType clientType)
        {
            UserName = userName;
            Password = password;
            ClientType = clientType;
        }
    }

}
