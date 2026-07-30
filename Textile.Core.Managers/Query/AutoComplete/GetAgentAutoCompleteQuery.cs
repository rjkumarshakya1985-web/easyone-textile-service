using MediatR;
using Textile.Core.Entities.Models.Response.Agents;

namespace Textile.Core.Managers.Query.AutoComplete
{


    public class GetAgentAutoCompleteQuery : IRequest<IEnumerable<AgentTableResponse>>
    {
        public string Search { get; }

        public GetAgentAutoCompleteQuery(string search)
        {
            Search = search;
        }
    }
}
