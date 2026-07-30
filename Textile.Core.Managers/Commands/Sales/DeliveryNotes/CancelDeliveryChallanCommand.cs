using MediatR;

namespace Textile.Core.Managers.Commands.Sales.DeliveryNotes
{
    public class CancelDeliveryChallanCommand : IRequest<bool>
    {
        public int DeliveryChallanId { get; set; }
        public Guid CurrentUserId { get; set; }
        public string CurrentUserName { get; set; }

        public CancelDeliveryChallanCommand(int deliveryChallanId, string currentUserName, Guid currentUserId)
        {
            DeliveryChallanId = deliveryChallanId;
            CurrentUserName = currentUserName;
            CurrentUserId = currentUserId;
        }
    }
}
