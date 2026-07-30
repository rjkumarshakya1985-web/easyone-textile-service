using MediatR;
using Textile.Core.Entities.Models.Response;

namespace Textile.Core.Managers.Query.AutoComplete
{
    public class GetHsnAutoCompleteQuery : IRequest<IEnumerable<HsnCodeResponse>>
    {
        public string Search { get; }

        public GetHsnAutoCompleteQuery(string search)
        {
            Search = search;
        }
    }
}
