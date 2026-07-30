using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Textile.Core.Entities.Enums;
using Textile.Core.Entities.Models.Requests;
using Textile.Core.Entities.Models.Requests.Tally;
using Textile.Core.Interfaces.Services;
using Textile.Core.Interfaces.Services.Tally;
using Textile.Core.Managers.Query.Tally;

namespace EasyOneService.Controllers.Tally
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SuperAdmin,StockIncharge,PackingslipOperator,Cashier")]
    public class TallyTransactionController : BaseController
    {
        private readonly ILogger<TallyTransactionController> _logger;
        private readonly IMediator _mediator;
        private readonly ITallyNameService _tallyNameService;
        private readonly IParcelService _parcelService;
        public TallyTransactionController(IUserContextService userContextService,
            ILogger<TallyTransactionController> logger, IMediator mediator, ITallyNameService tallyNameService, IParcelService parcelService) : base(userContextService)
        {

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _tallyNameService = tallyNameService ?? throw new ArgumentNullException(nameof(tallyNameService));
            _parcelService = parcelService ?? throw new ArgumentNullException(nameof(parcelService));
        }
        [HttpGet("purchase/{id}")]
        public async Task<IActionResult> GetTallyPurchase(int id)
        {
            try
            {
                var result = await _mediator.Send(
                new GetTallyTransactionPurchaseQuery { Id = id });
                return ApiResponse(result, "Sale Voucher fetched successfully");
            }
            catch (InvalidOperationException ex)
            {
                return ApiError(ex.Message, 400); // ? Business rule violation
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Sale Voucher Not Found");

                return ApiError(ex.Message, 404); // ? Not Found
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Sale Voucher");

                return ApiError(ex.Message, 500); // ? Generic message
            }

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTallyData(int id, [FromBody] List<TallyNameRequest> request,bool isStockTransfer)
        {
            try
            {
                // Step 1: Create SaleVoucherStatusView object
                var parcelScanRequest = new ParcelScanRequest
                {
                    SaleVoucherId = new List<int> { id },
                    StatusEnum = ParcelStatusEnum.TallySynced
                };
                // Step 2: Call existing service             
                await _parcelService.ChangeSaleVouchersStatus(parcelScanRequest, CurrentUserId, CurrentUserName);

                // Step 3: Update Tally Names in Bulk
                var result = await _tallyNameService.UpdateBulkTallyNames(request);

                // Step 3: Stock Update
                if (isStockTransfer)
                {
                    await _parcelService.MoveSaleVoucherProductsToStockAsync(parcelScanRequest, CurrentUserId, CurrentUserName);

                }
                return ApiResponse(result, "Tally Name Updated Successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating Tally Names");
                return StatusCode(500, ex.Message);
            }

        }



    }
}
