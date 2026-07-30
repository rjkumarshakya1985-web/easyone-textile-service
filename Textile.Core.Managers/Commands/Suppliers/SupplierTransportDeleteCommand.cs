using MediatR;
using Textile.Core.Entities.Models.Requests.Suppliers;

namespace Textile.Core.Managers.Commands.Suppliers
{


    public class SupplierTransportDeleteCommand : IRequest<bool>
    {
        public SupplierTransportDeleteRequest SupplierTransportDeleteRequest;

        public SupplierTransportDeleteCommand(SupplierTransportDeleteRequest supplierTransportDeleteRequest)
        {
            SupplierTransportDeleteRequest = supplierTransportDeleteRequest;
        }
    }
}
