using Microsoft.AspNetCore.Mvc;
using Textile.Core.Interfaces.Services;
using Textile.Core.Interfaces.Services.Sales;


namespace EasyOneService.Controllers.Sales
{
    [ApiController]
    [Route("api/[controller]")]
    public class BillingPrintController : BaseController
    {


        private readonly ILogger<BillingPrintController> _logger;
        private readonly IBillingPrintService _billingPrintService;

        public BillingPrintController(IUserContextService userContextService,
            ILogger<BillingPrintController> logger, IBillingPrintService billingPrintService) : base(userContextService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _billingPrintService = billingPrintService ?? throw new ArgumentNullException(nameof(billingPrintService));

        }

        [HttpGet("packingslip/{id}")]
        public async Task<IActionResult> GetPackingSlipPrint(int id)
        {
            try
            {
                var result = await _billingPrintService.GetPackingSlipPrint(id);
                return ApiResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching billing print for ID {Id}", id);
                return ApiError(ex.Message, 500);
            }

        }

        [HttpGet("delivery-challan/{id}")]
        public async Task<IActionResult> GetDeliveryChallan(int id)
        {
            try
            {
                var result = await _billingPrintService.GetDeliveryChallanPrint(id);
                return ApiResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching billing print for ID {Id}", id);
                return ApiError(ex.Message, 500);
            }

        }
    }
}
