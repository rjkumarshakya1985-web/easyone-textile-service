using Microsoft.AspNetCore.Mvc;
using Textile.Core.Entities.Models.Requests;
using Textile.Core.Entities.Models.Response.SalePersons;
using Textile.Core.Interfaces.Services;
using Textile.Core.Interfaces.Services.Sales;

namespace EasyOneService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalePersonController : BaseController
    {
        private readonly ILogger<SalePersonController> _logger;
        private readonly ISalesPersonService _salesPersonService;

        public SalePersonController(
            IUserContextService userContextService,
            ILogger<SalePersonController> logger,
            ISalesPersonService salesPersonService)
            : base(userContextService)
        {
            _logger = logger;
            _salesPersonService = salesPersonService ?? throw new ArgumentNullException(nameof(salesPersonService));
        }


        [HttpPost("table")]
        public async Task<IActionResult> GetTableData([FromBody] TableDataRequest request)
        {
            try
            {
                var result = await _salesPersonService.GetTableData(request);
                return ApiResponse(result, "Sales persons fetched successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching sales persons");
                return ApiError(ex.Message, 500);
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var result = await _salesPersonService.GetByIdAsync(id);

                if (result == null)
                    return ApiError("Sales person not found", 404);

                return ApiResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching sales person");
                return ApiError(ex.Message, 500);
            }
        }

        // ? Active List (Dropdown)
        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            try
            {
                var result = await _salesPersonService.GetActiveSalesPerson();
                return ApiResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching active sales persons");
                return ApiError(ex.Message, 500);
            }
        }


        [HttpPost]
        public async Task<IActionResult> Save([FromBody] SalePersonRequest request)
        {
            try
            {
                var result = await _salesPersonService.SaveAsync(
                    request,
                    CurrentUserId,
                    CurrentUserName);

                return ApiResponse(result, "Sales person saved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving sales person");
                return ApiError(ex.Message, 500);
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var result = await _salesPersonService.DeleteAsync(
                    id,
                    CurrentUserId,
                    CurrentUserName);

                if (!result)
                    return ApiError("Sales person not found", 404);

                return ApiResponse(true, "Sales person deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting sales person");
                return ApiError(ex.Message, 500);
            }
        }
    }
}
