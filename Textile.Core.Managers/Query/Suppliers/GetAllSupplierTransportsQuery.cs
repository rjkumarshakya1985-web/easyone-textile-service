using MediatR;
using Textile.Core.Entities.Models.Response;

namespace Textile.Core.Managers.Query.Suppliers
{
    public class GetAllSupplierTransportsQuery : IRequest<IEnumerable<TransportResponse>>
    {
        public Guid SupplierId { get; set; }
        public GetAllSupplierTransportsQuery(Guid supplier) { 
        
            SupplierId = supplier;
        }
    }
}
