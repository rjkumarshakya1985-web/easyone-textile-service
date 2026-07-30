using MediatR;
using Microsoft.AspNetCore.Mvc;
using Textile.Core.Entities.Data;
using Textile.Core.Entities.Models.Requests;
using Textile.Core.Entities.Models.Requests.Visitors;
using Textile.Core.Entities.Models.Response.Visitors;
using Textile.Core.Interfaces.Services;
using Textile.Core.Managers.Commands.Visitors;
using Textile.Core.Managers.Query.Visitors;

namespace EasyOneService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VisitorController : BaseController
    {
        private readonly ILogger<VisitorController> _logger;
        private readonly IMediator _mediator;
        private readonly IVisitorService _visitorService;
        public VisitorController(IUserContextService userContextService, IVisitorService visitorService,
           ILogger<VisitorController> logger, IMediator mediator) : base(userContextService)
        {
            _visitorService = visitorService ?? throw new ArgumentNullException(nameof(visitorService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        [HttpPost]
        public async Task<int> CreateVisitor(VisitorRequest request)
        {
            var userId = CurrentUserId;
            var userName = CurrentUserName;
            var command = new CreateVisitorCommand(request, userId, userName);
            return await _mediator.Send(command);
        }

        [HttpGet("{id}")]
        public async Task<VisitorResponse?> GetVisitor(int id)
        {
            var query = new GetVisitorQuery(id);
            return await _mediator.Send(query);
        }




        // -------------------------
        // PAGINATION / TABLE
        // -------------------------
        [HttpPost("table")]
        public async Task<TableResult<VisitorResponse>> GetTableData([FromBody] TableDataRequest request)
        {
            var result = await _visitorService.GetTableData(request);
            return result;
        }

        ////  Mobile
        ///
        [HttpGet("mobile/{id:int}")]
        public async Task<IActionResult> GetMobileVisitor(int id)
        {
            try
            {
                var result = await _visitorService.GetVisitoryById(id);
                return ApiResponse(result, "Packing slip created successfully");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating packing slip");
                return ApiError(ex.Message, 500);
            }

        }


        [HttpGet("mobile/{value}")]
        public async Task<IActionResult> GetMobileVisitor(string value)
        {
            try
            {

                var result = await _visitorService.GetVisitoryByMobile(value);
                return ApiResponse(result, "Packing slip created successfully");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating packing slip");
                return ApiError(ex.Message, 500);
            }
        }


    }
}
