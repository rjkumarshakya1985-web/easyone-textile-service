using MediatR;
using Textile.Core.Entities.Models.Requests.Billing;

namespace Textile.Core.Managers.Commands.Sales.Invoices
{
   
    public class CreateInvoiceCommand : IRequest<int>
    {

        public BillingRequest BillingRequest { get; set; }
        public string CurrentUserName { get; set; }
        public Guid CurrentUserId { get; set; }
        public CreateInvoiceCommand(BillingRequest billingRequest, string currentUserName, Guid currentUserId)
        {
            BillingRequest = billingRequest ?? throw new ArgumentNullException(nameof(billingRequest));
            CurrentUserName = currentUserName;
            CurrentUserId = currentUserId;
        }
    }
}
