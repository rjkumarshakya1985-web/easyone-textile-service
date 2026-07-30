using AutoMapper;
using MediatR;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Models.Response.Agents;
using Textile.Core.Interfaces.Data;
using Textile.Core.Managers.Query.Agents;

namespace Textile.Core.Managers.Handlers.Query.Agents
{
    public class GetAgentByIdQueryHandler : IRequestHandler<GetAgentByIdQuery, AgentDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAgentByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<AgentDTO> Handle(GetAgentByIdQuery request, CancellationToken cancellationToken)
        {
            var agentRepository = _unitOfWork.Repository<Agent, Guid>();

            var agent = await agentRepository.GetByIdAsync(request.AgentId, s => s.City.State);

            if (agent == null)
                return null;

            var agentDto = _mapper.Map<AgentDTO>(agent);
            agentDto.StateId = agent.City?.StateId;
            return agentDto;
        }
    }
}
