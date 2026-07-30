using MediatR;
using Textile.Core.Entities.Models.Requests.PackingSlips;

namespace Textile.Core.Managers.Commands.PackingSlip
{

    public class UpdatePackingSlipCommand : IRequest<int>
    {
        public PackingSlipRequest PackingSlipRequest;
        public Guid CurrentUserId;
        public string CurrentUserName;
        public int PackingSlipId;
        public UpdatePackingSlipCommand(PackingSlipRequest request, Guid currentUserId, string currentUserName, int packingSlipId)
        {
            PackingSlipRequest = request;
            CurrentUserId = currentUserId;
            CurrentUserName = currentUserName;
            PackingSlipId = packingSlipId;
        }
    }
}
