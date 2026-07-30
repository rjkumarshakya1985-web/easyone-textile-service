using MediatR;
using Microsoft.AspNetCore.Mvc;
using Textile.Core.Entities.Data;
using Textile.Core.Entities.Models.Requests;
using Textile.Core.Entities.Models.Requests.PackingSlips;
using Textile.Core.Entities.Models.Response.PackingSlip;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Interfaces.Services;
using Textile.Core.Interfaces.Services.Sales;
using Textile.Core.Managers.Commands.PackingSlip;



namespace EasyOneService.Controllers.Sales
{
    [ApiController]
    [Route("api/[controller]")]
    public class PackingSlipController : BaseController
    {


        private readonly ILogger<PackingSlipController> _logger;
        private readonly IMediator _mediator;
        private readonly IPackingSlipService _packingSlipService;
        private readonly TextileDbContext _textileDbContext;

        public PackingSlipController(IUserContextService userContextService,
            ILogger<PackingSlipController> logger, IMediator mediator,
            IPackingSlipService packingSlipService, TextileDbContext textileDbContext) : base(userContextService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

            _packingSlipService = packingSlipService ?? throw new ArgumentNullException(nameof(packingSlipService));
            _textileDbContext = textileDbContext ?? throw new ArgumentNullException(nameof(textileDbContext));
        }

        [HttpPost("table")]
        public async Task<TableResult<PackingSlipListResponse>> GetTableData(TableDataRequest tableDataRequest)
        {

            return await _packingSlipService.GetTableData(tableDataRequest);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PackingSlipRequest request)
        {

            try
            {
                // Fill FinanceYearId from BaseController
                request.FinanceYearId = await GetCurrentFinanceYearIdAsync(_textileDbContext);

                var command = new CreatePackingSlipCommand(
                   request,
                   CurrentUserId,
                   CurrentUserName);

                // Call service to create packing slip
                var id = await _mediator.Send(command);

                if (id > 0)
                    return ApiResponse(id, "Invoice created successfully");

                return StatusCode(500, "Failed to create invoice");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating packing slip");
                return ApiError(ex.Message, 500);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PackingSlipRequest request)
        {


            try
            {


                var command = new UpdatePackingSlipCommand(
                   request,
                   CurrentUserId,
                   CurrentUserName,
                   id);

                var Id = await _mediator.Send(command);

                if (Id > 0)
                    return ApiResponse(id, "Invoice updated successfully");

                return StatusCode(500, "Failed to update invoice");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error update packing slip");
                return ApiError(ex.Message, 500);
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var currentUserId = CurrentUserId;
                var currentUserName = CurrentUserName;

                var result = await _packingSlipService.DeleteAsync(id, currentUserId, currentUserName);

                if (result)
                    return Ok(new { message = "Packing slip deleted successfully" });

                return StatusCode(500, "Failed to delete packing slip");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting packing slip");
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetPackingSlip(int id)
        {
            try
            {
                var result = await _packingSlipService.GetByIdAsync(id);
                return ApiResponse(result, "Packing Slip fetched successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching packing slip");
                return ApiError(ex.Message, 500);
            }
        }

        [HttpGet("number/{number}")]
        public async Task<IActionResult> GetPackingSlipByNumber(string number)
        {
            try
            {
                var result = await _packingSlipService.GetByPackingSlipNumberAsync(number);
                return ApiResponse(result, "Packing Slip fetched successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching packing slip by number");
                return ApiError(ex.Message, 500);
            }
        }

        [HttpGet("number/{number}/{financeYearId}")]
        public async Task<IActionResult> GetPackingSlipByFinId(string number, int financeYearId)
        {
            try
            {
                var result = await _packingSlipService.GetPackingSlipNumberForBillingAsync(number, financeYearId);
                return ApiResponse(result, $"Packing Slip fetched  successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching packing slip by number : {number} and financeYearId :{financeYearId}");
                return ApiError(ex.Message, 500);
            }
        }

        [HttpGet("billing-packingslips/{visitorId}/{financeYearId}")]
        public async Task<IActionResult> GetPackingSlipsByVisitorId(int visitorId, int financeYearId)
        {
            try
            {
                var result = await _packingSlipService.GetPackingSlipsNumberForBillingByVisitorIdAsync(visitorId, financeYearId);
                return ApiResponse(result, $"Packing Slip fetched  successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching packing slips by visitorid : {visitorId}");
                return ApiError(ex.Message, 500);
            }
        }


        [HttpGet("pending-packing-slips-for-bill")]
        public async Task<IActionResult> GetPendingPackingSlipsForBill([FromQuery] int? financeYearId)
        {
            try
            {
                var result = await _packingSlipService
                    .GetPendingPackingSlipForBilling(CurrentUserId, CurrentUserRole, financeYearId);

                return ApiResponse(result, "Packing slips fetched successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    $"Error fetching pending packing slips for financeYearId : {financeYearId}");

                return ApiError(ex.Message, 500);
            }
        }
    }
}
