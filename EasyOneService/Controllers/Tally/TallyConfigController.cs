using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Textile.Core.Entities.Models.Response.Tally;
using Textile.Core.Interfaces.Services;
using Textile.Core.Managers.Query.Tally;
using Textile.Core.Managers.Services;

namespace EasyOneService.Controllers.Tally
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SuperAdmin,StockIncharge,PackingslipOperator,Cashier")]
    public class TallyConfigController : BaseController
    {
            private readonly ILogger<TallyConfigController> _logger;
            private readonly IMediator _mediator;
        private readonly ITallyConfigService _tallyConfigService;
        public TallyConfigController(IUserContextService userContextService,
                ILogger<TallyConfigController> logger, IMediator mediator,ITallyConfigService tallyConfigService) : base(userContextService)
            {
           
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _tallyConfigService= tallyConfigService ?? throw new ArgumentNullException(nameof(tallyConfigService));
        }       
        [HttpGet("{companyId}")]
        public async Task<IActionResult> GetTallyConfig(int companyId)
        {            
            try
            {
                var result = await _mediator.Send(
                new GetTallyConfigQuery { CompanyId = companyId });
                return ApiResponse(result, "Tally Configs fetched successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Tally Configs");
                return ApiError(ex.Message, 500);
            }
        }
        //  Get All
        [HttpGet("companies")]
        public async Task<IActionResult> GetAllCompanies()
        {

            try
            {

                var result = await _tallyConfigService.GetAllCompanies();
                return ApiResponse(result, "Active Tally Companies retrieved successfully");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retieving Tally Companies");
                return ApiError(ex.Message, 500);
            }

        }

    }
}
