using MediatR;
using Textile.Core.Entities.Models.Requests.Users;

namespace Textile.Core.Managers.Commands.Users
{

    public class CreateUserCommand : IRequest<Guid>
    {
        public UserRequest UserRequest { get; set; }

        public CreateUserCommand(UserRequest userRequest)
        {
            UserRequest = userRequest;
        }
    }
}
