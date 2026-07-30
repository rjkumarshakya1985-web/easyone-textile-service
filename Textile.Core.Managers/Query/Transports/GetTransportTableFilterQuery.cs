using MediatR;
using Textile.Core.Entities.Data;
using Textile.Core.Entities.Models.Requests;
using Textile.Core.Entities.Models.Response;

namespace Textile.Core.Managers.Query.Transports
{
    public class GetTransportTableFilterQuery : IRequest<TableResult<TransportResponse>>
    {
        public TableDataRequest DataRequest { get; }

        public GetTransportTableFilterQuery(TableDataRequest dataRequest)
        {
            DataRequest = dataRequest;
        }
    }
}
