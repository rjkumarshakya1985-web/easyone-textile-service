using MediatR;
using Textile.Core.Entities.Models.Requests.Suppliers;

namespace Textile.Core.Managers.Commands.Suppliers
{
    public class UpdateSupplierStatusCommand : IRequest<bool>
    {
        public UpdateSupplierStatusRequest Request { get; }

        public UpdateSupplierStatusCommand(UpdateSupplierStatusRequest request)
        {
            Request = request;
        }
    }
}
