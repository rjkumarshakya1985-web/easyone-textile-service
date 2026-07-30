using MediatR;
using Textile.Core.Entities.Models.Requests.Billing.Invoices;

namespace Textile.Core.Managers.Commands.Sales.Invoices
{
    public class CreateDeliveryChallansToInvoiceCommand : IRequest<int>
    {
        public DeliveryChallanToInvoiceRequest InvoiceRequest;
        public Guid CurrentUserId;
        public string CurrentUserName;

        public CreateDeliveryChallansToInvoiceCommand(DeliveryChallanToInvoiceRequest invoiceRequest, string currentUserName, Guid currentUserId)
        {
            InvoiceRequest = invoiceRequest ?? throw new ArgumentNullException(nameof(invoiceRequest));
            CurrentUserName = currentUserName;
            CurrentUserId = currentUserId;

        }
    }
}
