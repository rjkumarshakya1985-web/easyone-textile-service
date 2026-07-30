using MediatR;
using Textile.Core.Entities.Models.Requests;

namespace Textile.Core.Managers.Commands
{

    public class ChangePasswordCommand : IRequest<bool>
    {
        public Guid UserId { get; }

        public ChangePasswordRequest changePasswordRequest { get; }

        public ChangePasswordCommand(ChangePasswordRequest _changePasswordRequest, Guid _userId)
        {
            changePasswordRequest = _changePasswordRequest;
            UserId = _userId;
        }
    }
}
