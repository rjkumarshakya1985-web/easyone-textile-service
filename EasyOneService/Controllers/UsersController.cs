
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Textile.Core.Entities.Data;
using Textile.Core.Entities.Models.Requests;
using Textile.Core.Entities.Models.Requests.Users;
using Textile.Core.Entities.Models.Response.Users;
using Textile.Core.Interfaces.Services;
using Textile.Core.Managers.Commands;
using Textile.Core.Managers.Commands.Users;

namespace EasyOneService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SuperAdmin")]
    public class UsersController : BaseController
    {


        private readonly ILogger<UsersController> _logger;
        private readonly IMediator _mediator;
        private readonly IUserService _userService;

        public UsersController(IUserContextService userContextService, IUserService userService, ILogger<UsersController> logger, IMediator mediator) : base(userContextService)
        {
            _logger = logger;
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));

        }

        [HttpPost("table")]
        public async Task<TableResult<UserResponse>> GetTable(TableDataRequest tableDataRequest)
        {
            return await _userService.GetTableData(tableDataRequest);
        }

        [HttpGet("current")]
        public async Task<UserResponse?> GetCurrent()
        {
            return await _userService.GetByIdAsync(CurrentUserId);
        }

        [HttpGet("{id:guid}")]
        public async Task<UserResponse?> Get(Guid id)
        {
            return await _userService.GetByIdAsync(id);
        }

        [HttpPost]
        public async Task<Guid> SaveUser(UserRequest userRequest)
        {
            userRequest.CreatedBy = CurrentUserId;
            userRequest.CreatedByUserName = CurrentUserName;

            var command = new CreateUserCommand(userRequest);
            return await _mediator.Send(command);
        }

        [HttpGet("toggle/{id:guid}")]
        public async Task<UserResponse?> Toggle(Guid id)
        {
            return await _userService.GetByIdAsync(id);
        }


        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var command = new ChangePasswordCommand(request, CurrentUserId);
            var response = await _mediator.Send(command);
            return Ok(response);
        }
    }
}
