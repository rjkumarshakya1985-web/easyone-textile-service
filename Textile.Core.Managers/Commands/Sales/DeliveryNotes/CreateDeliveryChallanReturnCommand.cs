using MediatR;
using Textile.Core.Entities.Models.Requests.Billing.DeliveryChallan;

namespace Textile.Core.Managers.Commands.Sales.DeliveryNotes
{

    public class CreateDeliveryChallanReturnCommand : IRequest<int>
    {

        public DeliveryChalanReturnRequest DeliveryChalanReturnRequest { get; set; }
        public string CurrentUserName { get; set; }
        public Guid CurrentUserId { get; set; }
        public CreateDeliveryChallanReturnCommand(DeliveryChalanReturnRequest deliveryChalanReturnRequest, string currentUserName, Guid currentUserId)
        {
            DeliveryChalanReturnRequest = deliveryChalanReturnRequest ?? throw new ArgumentNullException(nameof(deliveryChalanReturnRequest));
            CurrentUserName = currentUserName;
            CurrentUserId = currentUserId;
        }
    }
}
