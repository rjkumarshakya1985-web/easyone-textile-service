using MediatR;

namespace Textile.Core.Managers.Commands.Sales.Invoices
{
    public class CancelInvoiceCommand : IRequest<bool>
    {
        public int InvoiceId { get; set; }
        public Guid CurrentUserId { get; set; }
        public string CurrentUserName { get; set; }

        public CancelInvoiceCommand(int invoiceId, string currentUserName, Guid currentUserId)
        {
            InvoiceId = invoiceId;
            CurrentUserName = currentUserName;
            CurrentUserId = currentUserId;
        }
    }
}
