using MediatR;
using Textile.Core.Entities.Models.Response.Agents;

namespace Textile.Core.Managers.Query.AutoComplete
{
    public class GetCustomerAgentAutoCompleteQuery : IRequest<IEnumerable<AgentTableResponse>>
    {
        public string Search { get; }

        public GetCustomerAgentAutoCompleteQuery(string search)
        {
            Search = search;
        }
    }
}
