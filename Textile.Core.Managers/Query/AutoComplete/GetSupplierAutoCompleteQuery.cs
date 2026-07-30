using MediatR;
using Textile.Core.Entities.Models.Response.Suppliers;

namespace Textile.Core.Managers.Query.AutoComplete
{
    public class GetSupplierAutoCompleteQuery : IRequest<IEnumerable<SupplierTableResponse>>
    {
        public string Search { get; }

        public GetSupplierAutoCompleteQuery(string search)
        {
            Search = search;
        }
    }
}
