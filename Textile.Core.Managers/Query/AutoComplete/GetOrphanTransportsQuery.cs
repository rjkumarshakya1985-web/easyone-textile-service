using MediatR;
using Textile.Core.Entities.Models.Response;

namespace Textile.Core.Managers.Query.AutoComplete
{
    public class GetOrphanTransportsQuery :IRequest<IEnumerable<TransportResponse>>
    {
        public string Search { get; }
        public Guid SupplierId { get; }
        public GetOrphanTransportsQuery(Guid supplierId, string search)
        {

          Search = search;
          SupplierId = supplierId;

        }
    }
}
