using MediatR;
using Textile.Core.Entities.Models.Requests.Billing.DeliveryChallans;

namespace Textile.Core.Managers.Commands.Sales.DeliveryNotes
{

    public class UpdateDeliveryChallanCommand : IRequest<int>
    {
        public UpdateDeliveryChallanRequest Request { get; set; }
        public string CurrentUserName { get; set; }
        public Guid CurrentUserId { get; set; }
        public UpdateDeliveryChallanCommand(UpdateDeliveryChallanRequest request, string currentUserName, Guid currentUserId)
        {
            Request = request ?? throw new ArgumentNullException(nameof(request));
            CurrentUserName = currentUserName;
            CurrentUserId = currentUserId;
        }
    }
}
