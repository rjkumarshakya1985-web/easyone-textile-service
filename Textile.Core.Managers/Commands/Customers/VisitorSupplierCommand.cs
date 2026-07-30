using MediatR;
using Textile.Core.Entities.Models.Requests.Customers;
using Textile.Core.Entities.Models.Response.Visitors;

namespace Textile.Core.Managers.Commands.Customers
{

    public class VisitorSupplierCommand : IRequest<VisitorResponse>
    {
        public CustomerRequest Request { get; }
        public Guid CurrentUserId { get; }
        public string CurrentUserName { get; }
        public int VisitorId { get; }

        public VisitorSupplierCommand(CustomerRequest request,
            Guid currentUserId, string currentUserName, int visitorId)
        {
            Request = request;
            CurrentUserId = currentUserId;
            CurrentUserName = currentUserName;
            VisitorId = visitorId;
        }
    }
}
