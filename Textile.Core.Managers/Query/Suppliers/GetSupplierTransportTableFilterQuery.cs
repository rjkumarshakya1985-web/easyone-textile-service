using MediatR;
using Textile.Core.Entities.Data;
using Textile.Core.Entities.Models.Requests;
using Textile.Core.Entities.Models.Response.Suppliers;

namespace Textile.Core.Managers.Query.Suppliers
{
    public class GetSupplierTransportTableFilterQuery : IRequest<TableResult<SupplierTransportResponse>>
    {
        public TableDataRequest DataRequest { get; }

        public GetSupplierTransportTableFilterQuery(TableDataRequest dataRequest)
        {
            DataRequest = dataRequest;
        }
    }
}
