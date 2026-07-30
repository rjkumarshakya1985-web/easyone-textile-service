using Microsoft.AspNetCore.Mvc;
using Textile.Core.Entities.Models.Requests.FinanceYears;
using Textile.Core.Interfaces.Services;

namespace EasyOneService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FinanceYearsController : BaseController
    {
        private readonly ILogger<FinanceYearsController> _logger;
        private readonly IFinanceYearService _financeYearService;

        public FinanceYearsController(
            IUserContextService userContextService,
            ILogger<FinanceYearsController> logger,
            IFinanceYearService financeYearService)
            : base(userContextService)
        {
            _logger = logger;
            _financeYearService = financeYearService;
        }

        //  Get All
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _financeYearService.GetFinanceYears();
            return Ok(data);
        }

        //  Get Active
        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {

            try
            {

                var result = await _financeYearService.GetActiveFinanceYears();
                return ApiResponse(result, "Active financial year retrieved successfully");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error active financial year retrieve");
                return ApiError(ex.Message, 500);
            }

        }

        //  Create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] FinanceYearRequest request)
        {
            var result = await _financeYearService.AddFinanceYear(request, CurrentUserId, CurrentUserName);
            return Ok(result);
        }

        //  Activate Finance Year (BEST)
        [HttpPut("{id}/activate")]
        public async Task<IActionResult> Activate(int id)
        {
            var result = await _financeYearService.ToggleFinanceYearStatus(id, CurrentUserId, CurrentUserName);
            return Ok(result);
        }


    }

}
