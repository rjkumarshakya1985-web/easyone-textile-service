using MediatR;
using Textile.Core.Entities.Models.Response.Suppliers;

namespace Textile.Core.Managers.Query.Suppliers
{

    public class GetSupplierByIdQuery : IRequest<SupplierDTO>
    {
        public Guid SupplierId { get; set; }

        public GetSupplierByIdQuery(Guid supplierId)
        {
            SupplierId = supplierId;
        }
    }
}
