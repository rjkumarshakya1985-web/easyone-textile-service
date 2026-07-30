using MediatR;
using Textile.Core.Entities.Models.Requests.Customers;

namespace Textile.Core.Managers.Commands.Customers
{
    public class UpdateCustomerStatusCommand : IRequest<bool>
    {
        public UpdateCustomerStatusRequest Request { get; }

        public UpdateCustomerStatusCommand(UpdateCustomerStatusRequest request)
        {
            Request = request;
        }
    }
}
