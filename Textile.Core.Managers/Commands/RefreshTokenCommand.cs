using MediatR;
using Textile.Core.Entities.Enums;
using Textile.Core.Entities.Models.Response;

namespace Textile.Core.Managers.Commands
{
    public class RefreshTokenCommand : IRequest<LoginResponse>
    {
        public string RefreshToken { get; }

        public ClientType ClientType { get; }
        public RefreshTokenCommand(string refreshToken,ClientType clientType)
        {
            RefreshToken = refreshToken;
            ClientType = clientType;
        }
    }

}
