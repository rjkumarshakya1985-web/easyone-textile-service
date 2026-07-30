using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Textile.Core.Entities.Models.Requests.Tally;
using Textile.Core.Interfaces.Services;
using Textile.Core.Managers.Commands.Tally;
using Textile.Core.Managers.Query.Tally;

namespace EasyOneService.Controllers.Tally
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SuperAdmin,StockIncharge,PackingslipOperator,Cashier")]
    public class TallyProcessController : BaseController
    {
        private readonly ILogger<TallyProcessController> _logger;
        private readonly IMediator _mediator;
        public TallyProcessController(IUserContextService userContextService,
            ILogger<TallyProcessController> logger, IMediator mediator) : base(userContextService)
        {

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }        
        [HttpPost("savetallyprocess")]
        public async Task<IActionResult> SaveTallyProcess([FromBody] List<TallyProcessRequest> requests)
        {        
            if (requests == null || !requests.Any())
                return BadRequest("No logs received");
            try
            {
                var command = new SaveTallyProcessCommand(requests);
                var result = await _mediator.Send(command);

                return Ok(result);
            }
            catch (Exception ex)
            {
                // ? VERY IMPORTANT (so UI sees actual error)
                return StatusCode(500, ex.Message);
            }          
        }        
        [HttpGet("tallyprocess/{companyId}")]
        public async Task<IActionResult> GetTallyProcess(int companyId)
        {
            var result = await _mediator.Send(
                new GetTallyProcessQuery { CompanyId = companyId });

            return Ok(result);
        }
        [HttpGet("tallyprocess/{companyId}/{financialYearId}/{processType}/{referenceNo}")]
        public async Task<IActionResult> GetTallyProcessSteps(int companyId, int financialYearId, int processType, string referenceNo)
        {
            try
            {
                var result = await _mediator.Send(
                 new GetTallyProcessStepsQuery
                 {
                     CompanyId = companyId,
                     FinancialYearId = financialYearId,
                     processType = processType,
                     ReferenceNo = referenceNo
                 });
                return ApiResponse(result, "Tally Steps fetched successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Tally Steps");
                return ApiError(ex.Message, 500);
            }
        }
    }
}
