using MediatR;
using Textile.Core.Entities.Data;
using Textile.Core.Entities.Models.Requests;
using Textile.Core.Entities.Models.Response.Suppliers;

namespace Textile.Core.Managers.Query.Suppliers
{

    public class GetSupplierTableFilterQuery : IRequest<TableResult<SupplierTableResponse>>
    {
        public TableDataRequest DataRequest { get; }

        public GetSupplierTableFilterQuery(TableDataRequest dataRequest)
        {
            DataRequest = dataRequest;
        }
    }
}
