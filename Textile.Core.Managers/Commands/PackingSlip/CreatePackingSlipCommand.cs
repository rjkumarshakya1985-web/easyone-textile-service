using MediatR;
using Textile.Core.Entities.Models.Requests.PackingSlips;

namespace Textile.Core.Managers.Commands.PackingSlip
{
    public class CreatePackingSlipCommand : IRequest<int>
    {
        public PackingSlipRequest PackingSlipRequest;
        public Guid CurrentUserId;
        public string CurrentUserName;

        public CreatePackingSlipCommand(PackingSlipRequest request, Guid currentUserId, string currentUserName)
        {
            PackingSlipRequest = request;
            CurrentUserId = currentUserId;
            CurrentUserName = currentUserName;
        }
    }
}
