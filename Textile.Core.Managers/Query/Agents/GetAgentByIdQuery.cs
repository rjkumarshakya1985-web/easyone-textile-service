using MediatR;
using Textile.Core.Entities.Models.Response.Agents;

namespace Textile.Core.Managers.Query.Agents
{
    public class GetAgentByIdQuery : IRequest<AgentDTO>
    {
        public Guid AgentId { get; set; }

        public GetAgentByIdQuery(Guid agentId)
        {
            AgentId = agentId;
        }
    }
}
