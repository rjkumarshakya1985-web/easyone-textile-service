using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Textile.Core.Entities.Data;
using Textile.Core.Entities.Models.Requests;
using Textile.Core.Entities.Models.Requests.Agents;
using Textile.Core.Entities.Models.Response.Agents;
using Textile.Core.Interfaces.Services;
using Textile.Core.Managers.Commands.Agents;
using Textile.Core.Managers.Query.Agents;

namespace EasyOneService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "SuperAdmin,Supplier")]
    public class AgentController : BaseController
    {


        private readonly ILogger<AgentController> _logger;
        private readonly IMediator _mediator;
       
        public AgentController(IUserContextService userContextService,
           ILogger<AgentController> logger,
           IMediator mediator) : base(userContextService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpPost("agent-table")]
        public async Task<TableResult<AgentTableResponse>> GetAgentTableData(TableDataRequest tableDataRequest)
        {
            var query = new GetAgentTableFilterQuery(tableDataRequest);
            return await _mediator.Send(query);
        }

       
        // -------------------------
        // CREATE
        // -------------------------

        [HttpPost("create-agent")]
        public async Task<IActionResult> CreateAgent(AgentRequest agentRequest)
        {
            var command = new CreateAgentCommand(agentRequest, CurrentUserId, CurrentUserName);

            var agentId = await _mediator.Send(command);

            return Ok(new
            {
                Success = true,
                Message = "Agent created successfully",
                Data = agentId
            });
        }

        [HttpPost("update-status-agent")]
        public async Task<bool> UpdateStatusAgent(UpdateAgentStatusRequest updateAgent)
        {
            var command = new UpdateAgentStatusCommand(updateAgent);

            return await _mediator.Send(command);

        }


        [HttpGet("agent-detail/{agentId}")]
        public async Task<AgentDTO> GetAgentById(Guid agentId)
        {
            var query = new GetAgentByIdQuery(agentId);
            return await _mediator.Send(query);

        }

      
    }
}
