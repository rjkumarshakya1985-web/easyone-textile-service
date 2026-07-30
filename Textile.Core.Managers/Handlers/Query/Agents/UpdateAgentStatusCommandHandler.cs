using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Enums;
using Textile.Core.Interfaces.Data;
using Textile.Core.Managers.Commands.Agents;
using Textile.Core.Managers.Commands.Suppliers;

namespace Textile.Core.Managers.Handlers.Query.Agents
{
   
    public class UpdateAgentStatusCommandHandler
   : IRequestHandler<UpdateAgentStatusCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateAgentStatusCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateAgentStatusCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            var repository = _unitOfWork.Repository<Agent, Guid>();

            // ---------------------------
            // Find existing mapping
            // ---------------------------
            var entity = await repository.GetSingleAsync(x =>
                x.Id == request.AgentId);

            if (entity == null)
                throw new Exception("Agent not found");

            // ---------------------------
            // Perform Action
            // ---------------------------
            switch (request.ActionType)
            {
                case AgentStatusActionType.Delete:
                    entity.IsDeleted = true;
                    entity.IsActive = false;
                    await UpdateAgentUserStatus(entity, false);
                    break;

                case AgentStatusActionType.Activate:
                    entity.IsActive = true;
                    entity.IsDeleted = false;
                    await UpdateAgentUserStatus(entity, true);
                    break;

                case AgentStatusActionType.Deactivate:
                    entity.IsActive = false;
                    await UpdateAgentUserStatus(entity, false);
                    break;

                default:
                    throw new Exception("Invalid action type");
            }

            // ---------------------------
            // Save changes
            // ---------------------------
            await repository.UpdateAsync(entity);

            return true;
        }


        private async Task UpdateAgentUserStatus(Agent agent, bool status)
        {
            var repository = _unitOfWork.Repository<User, Guid>();
            var user = await repository.GetByIdAsync(agent.Id);
            if (user != null)
            {
                user.IsActive = status;

                await repository.UpdateAsync(user);

            }
        }
    }
}
