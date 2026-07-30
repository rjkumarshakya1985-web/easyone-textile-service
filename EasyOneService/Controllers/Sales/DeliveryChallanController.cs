
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Textile.Core.Entities.Data;
using Textile.Core.Entities.Models.Requests;
using Textile.Core.Entities.Models.Requests.Billing;
using Textile.Core.Entities.Models.Requests.Billing.DeliveryChallan;
using Textile.Core.Entities.Models.Requests.Billing.DeliveryChallans;
using Textile.Core.Entities.Models.Response.DeliveryChallan;
using Textile.Core.Interfaces.Services;
using Textile.Core.Interfaces.Services.Sales;
using Textile.Core.Managers.Commands.Sales.DeliveryNotes;


namespace EasyOneService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeliveryChallanController : BaseController
    {


        private readonly ILogger<DeliveryChallanController> _logger;
        private readonly IMediator _mediator;
        private readonly IDeliveryChallanReturnService _deliveryChallanReturnService;
        private readonly IDeliveryChallanService _deliveryChallanService;
        public DeliveryChallanController(IUserContextService userContextService,
            ILogger<DeliveryChallanController> logger,
            IMediator mediator,
            IDeliveryChallanReturnService deliveryChallanReturnService,
            IDeliveryChallanService deliveryChallanService)
            : base(userContextService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger)); ;
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _deliveryChallanReturnService = deliveryChallanReturnService;
            _deliveryChallanService = deliveryChallanService;
        }

        [HttpPost("table/{finYearId}")]
        public async Task<TableResult<DeliveryChallanListResponse>> GetTableData(TableDataRequest tableDataRequest, int finYearId)
        {

            return await _deliveryChallanService.GetTableData(tableDataRequest, finYearId);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BillingRequest request)
        {
            try
            {
                var currentUserId = CurrentUserId;
                var currentUserName = CurrentUserName;

                var command = new CreateDeliveryChallanCommand(request, currentUserName, currentUserId);

                var id = await _mediator.Send(command);
                if (id > 0)
                    return ApiResponse(id, "Delivery challan created successfully");

                return StatusCode(500, "Failed to create delivery challan");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating delivery challan");
                return ApiError("An error occurred while creating delivery challan.", 500);
            }
        }



        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateDeliveryChallanRequest request)
        {
            try
            {
                var currentUserId = CurrentUserId;
                var currentUserName = CurrentUserName;

                var command = new UpdateDeliveryChallanCommand(request, currentUserName, currentUserId);

                var id = await _mediator.Send(command);
                if (id > 0)
                    return ApiResponse(id, "Delivery challan created successfully");

                return StatusCode(500, "Failed to create delivery challan");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating delivery challan");
                return ApiError("An error occurred while creating delivery challan.", 500);
            }
        }

        [HttpDelete("cancel/{Id}")]
        public async Task<IActionResult> Cancel(int Id)
        {
            try
            {
                var currentUserId = CurrentUserId;
                var currentUserName = CurrentUserName;

                var command = new CancelDeliveryChallanCommand(Id, currentUserName, currentUserId);

                var status = await _mediator.Send(command);
                return ApiResponse(status, "Delivery challan cancel successfully");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating delivery challan");
                return ApiError("An error occurred while creating delivery challan.", 500);
            }
        }

        [HttpGet("delivery-challan/{number}/{financeYearId}")]
        public async Task<IActionResult> GetDeleveryChallanDetail(string number, int financeYearId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(number))
                    return BadRequest("Delivery Challan number is required");

                var result = await _deliveryChallanReturnService
                    .GetDeliveryChallan(number, financeYearId);

                return ApiResponse(result, "Delivery Challan fetched successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error fetching Delivery Challan for Number: {Number}, FinanceYearId: {FinanceYearId}",
                    number, financeYearId);

                return ApiError("Something went wrong while fetching delivery challan", 500);
            }
        }

        /// Delivery Challan Return
        /// 
        [HttpGet("delivery-challan/for-return/{number}/{financeYearId}")]
        public async Task<IActionResult> GetDeleveryChallanDetailForInvoice(string number, int financeYearId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(number))
                    return BadRequest("Delivery Challan number is required");

                var result = await _deliveryChallanReturnService
                    .GetDeliveryChallanForReturn(number, financeYearId);

                return ApiResponse(result, "Delivery Challan fetched successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error fetching Delivery Challan for Number: {Number}, FinanceYearId: {FinanceYearId}",
                    number, financeYearId);

                return ApiError("Something went wrong while fetching delivery challan", 500);
            }
        }

        [HttpPost("delivery-challan/return")]
        public async Task<IActionResult> CreateDeliveryChallanReturn([FromBody] DeliveryChalanReturnRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest("Invalid request");

                if (request.DeliveryChallanId <= 0)
                    return BadRequest("DeliveryChallanId is required");

                if (request.DeliveryChallanReturnItems == null || !request.DeliveryChallanReturnItems.Any())
                    return BadRequest("Return items are required");

                var command = new CreateDeliveryChallanReturnCommand(request, CurrentUserName, CurrentUserId);

                var result = await _mediator.Send(command);

                return ApiResponse(0, "Delivery Challan Return created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error creating Delivery Challan Return for ChallanId: {DeliveryChallanId}",
                    request?.DeliveryChallanId);

                return ApiError("Something went wrong while creating delivery challan return", 500);
            }
        }
    }
}
