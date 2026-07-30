using MediatR;
using Textile.Core.Entities.Models.Requests.Suppliers;

namespace Textile.Core.Managers.Commands.Suppliers
{
    public class AddSupplierTransportCommand : IRequest<bool>
    {

        public AddSupplierTransportRequest AddSupplierTransportRequest;

        public AddSupplierTransportCommand(AddSupplierTransportRequest addSupplierTransportRequest)
        {
            AddSupplierTransportRequest = addSupplierTransportRequest;
        }
    }
}
