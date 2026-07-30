using MediatR;
using Textile.Core.Entities.Views;

namespace Textile.Core.Managers.Query.AutoComplete
{
    public class GetSupplierProductAutoCompleteQuery : IRequest<IEnumerable<SupplierProductView>>
    {
      
        public Guid SupplierId { get; set; }

        public string Search { get; set; }
        public GetSupplierProductAutoCompleteQuery(Guid supplierId, string search)
        {
            
            SupplierId = supplierId;
            Search = search;
        }
    }
}
