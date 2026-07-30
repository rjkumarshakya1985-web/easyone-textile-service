using MediatR;
using Textile.Core.Entities.Models.Requests.Agents;

namespace Textile.Core.Managers.Commands.Agents
{
    public class CreateAgentCommand : IRequest<Guid>
    {
        public Guid CreatedBy { get; }
        public string CreatedByUserName { get; }
        public AgentRequest AgentRequest { get; }

        public CreateAgentCommand(AgentRequest agentRequest, Guid createBy, string createdByUserName)
        {
            CreatedBy = createBy;
            CreatedByUserName = createdByUserName;
            AgentRequest = agentRequest;
        }
    }
}
