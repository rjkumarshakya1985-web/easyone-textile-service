using MediatR;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Models.Requests.Agents;
using Textile.Core.Interfaces.Data;
using Textile.Core.Managers.Commands.Agents;

namespace Textile.Core.Managers.Handlers.Commands.Agents
{
    public class CreateAgentCommandHandler : IRequestHandler<CreateAgentCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator; // To call UserHandler

        public CreateAgentCommandHandler(IUnitOfWork unitOfWork, IMediator mediator
          )
        {
            _unitOfWork = unitOfWork;
            _mediator = mediator;
        }

        public async Task<Guid> Handle(CreateAgentCommand command, CancellationToken cancellationToken)
        {
            var request = command.AgentRequest;
            var agentRepo = _unitOfWork.Repository<Agent, Guid>();

            string agentCode = string.Empty;

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                bool isNewAgent = request.Id == null;

                if (isNewAgent)
                {


                  
                    var newAgent = MapAgentFields(request, command.CreatedBy, command.CreatedByUserName);
                    await agentRepo.AddAsync(newAgent);
                    await _unitOfWork.CommitTranscationAsync();
                    return newAgent.Id;

                }
                else
                {
                    
                    var existingAgent = await agentRepo.GetSingleAsync(a => a.Id == request.Id);
                    if (existingAgent == null)
                        throw new Exception("Agent not found.");

                    UpdateAgentFields(existingAgent, request, command.CreatedBy, command.CreatedByUserName);
                    await agentRepo.UpdateAsync(existingAgent);
                    await _unitOfWork.CommitTranscationAsync();
                    return existingAgent.Id;
                }
            }
            catch
            {
                await _unitOfWork.RollbackTranscationAsync();
                throw;
            }
        }

        private Agent MapAgentFields(AgentRequest request, Guid createdBy, string createdByUserName)
        {
            return new Agent
            {
                Id = Guid.NewGuid(),

                Name = request.Name,
                ContactPersonName = request.ContactPersonName,
                GSTIN = request.GSTIN,
                ContactPersonMobile = request.ContactPersonMobile,
                TallyLedgerName = request.TallyLedgerName,
                Address = request.Address,
                Pincode = request.Pincode,
                Area = request.Area,
                CityId = request.CityId,
                Email = request.Email,
                CreatedBy = createdBy,
                CreatedByUserName = createdByUserName,
                CreatedOn = DateTime.UtcNow,
                IsActive = true,
                IsDeleted = false,
            };


        }

        private void UpdateAgentFields(Agent agent, AgentRequest request, Guid modifiedBy, string modifiedByUserName)
        {

            agent.Name = request.Name;
            agent.ContactPersonName = request.ContactPersonName;
            agent.GSTIN = request.GSTIN;
            agent.PAN = request.PAN;
            agent.ContactPersonMobile = request.ContactPersonMobile;
            agent.Address = request.Address;
            agent.Pincode = request.Pincode;
            agent.Area = request.Area;
            agent.Email = request.Email;
            agent.CityId = request.CityId;
            agent.TallyLedgerName = request.TallyLedgerName;
            agent.ModifiedBy = modifiedBy;
            agent.ModifiedByUserName = modifiedByUserName;
            agent.ModifiedOn = DateTime.UtcNow;
        }




    }
}
