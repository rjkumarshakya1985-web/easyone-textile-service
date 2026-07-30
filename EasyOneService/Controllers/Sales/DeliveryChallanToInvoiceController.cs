
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Textile.Core.Entities.Models.Requests.Billing.Invoices;
using Textile.Core.Interfaces.Services;
using Textile.Core.Interfaces.Services.Sales;
using Textile.Core.Managers.Commands.Sales.Invoices;


namespace EasyOneService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeliveryChallanToInvoiceController : BaseController
    {


        private readonly ILogger<DeliveryChallanToInvoiceController> _logger;
        private readonly IMediator _mediator;
        private readonly IDeliveryChallanToInvoiceService _deliveryChallanToInvoiceService;
        public DeliveryChallanToInvoiceController(IUserContextService userContextService,
            ILogger<DeliveryChallanToInvoiceController> logger,
            IMediator mediator, IDeliveryChallanService deliveryChallanService,
            IDeliveryChallanToInvoiceService deliveryChallanToInvoiceService)
            : base(userContextService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger)); ;
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _deliveryChallanToInvoiceService = deliveryChallanToInvoiceService ?? throw new ArgumentNullException(nameof(deliveryChallanToInvoiceService));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DeliveryChallanToInvoiceRequest request)
        {
            try
            {
                var currentUserId = CurrentUserId;
                var currentUserName = CurrentUserName;



                var command = new CreateDeliveryChallansToInvoiceCommand(request,
                    CurrentUserName, CurrentUserId);

                var id = await _mediator.Send(command);

                if (id > 0)
                    return ApiResponse(id, "Invoice created successfully");

                return StatusCode(500, "Failed to create invoice");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating invoice");
                return ApiError("An error occurred while creating invoice.", 500);
            }
        }



        /// Delivery Challan Return
        /// 
        [HttpGet("delivery-challan/for-invoice/{number}/{financeYearId}")]
        public async Task<IActionResult> GetDeliveryChallanForInvoice(
         string number, int financeYearId)
        {
            if (string.IsNullOrWhiteSpace(number))
                return BadRequest("Delivery Challan number is required");

            if (financeYearId <= 0)
                return BadRequest("Invalid Finance Year");

            try
            {
                var result = await _deliveryChallanToInvoiceService
                    .GetDeliveryChallanForInvoiceByNumber(number, financeYearId);


                return ApiResponse(result, "Delivery Challan fetched successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error fetching Delivery Challan. Number: {Number}, FinanceYearId: {FinanceYearId}",
                    number, financeYearId);

                return ApiError("Something went wrong while fetching delivery challan", 500);
            }
        }

    }
}
