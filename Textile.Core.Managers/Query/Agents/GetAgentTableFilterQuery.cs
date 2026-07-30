using MediatR;
using Textile.Core.Entities.Data;
using Textile.Core.Entities.Models.Requests;
using Textile.Core.Entities.Models.Response.Agents;

namespace Textile.Core.Managers.Query.Agents
{
 
    public class GetAgentTableFilterQuery : IRequest<TableResult<AgentTableResponse>>
    {
        public TableDataRequest DataRequest { get; }

        public GetAgentTableFilterQuery(TableDataRequest dataRequest)
        {
            DataRequest = dataRequest;
        }
    }
}
