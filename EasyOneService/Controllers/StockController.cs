
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Textile.Core.Entities.Data;
using Textile.Core.Entities.Models.Requests;
using Textile.Core.Entities.Models.Requests.Stock;
using Textile.Core.Entities.Models.Response.Stocks;
using Textile.Core.Entities.Views;
using Textile.Core.Interfaces.Services;

namespace EasyOneService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockController : BaseController
    {


        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IMediator _mediator;
        private readonly IStockService _stockService;
        private readonly IStockAdjustmentService _stockAdjustmentService;

        public StockController(IUserContextService userContextService,ILogger<WeatherForecastController> logger, IMediator mediator, IStockService stockService, IStockAdjustmentService stockAdjustmentService) :base(userContextService)
        {
            _logger = logger;
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _stockService = stockService;
            _stockAdjustmentService = stockAdjustmentService;   
        }

        [HttpPost("table")]
        public async Task<TableResult<StockTableResponse>> GetTableData(TableDataRequest tableDataRequest)
        {

            return await _stockService.GetTableData(tableDataRequest);
        }

        [HttpPost("table-ledger")]
        public async Task<TableResult<StockLedgerViews>> GetStockLedgerTableData(TableDataRequest tableDataRequest)
        {

            return await _stockService.GetStockLedgerTableData(tableDataRequest);
        }

        [HttpGet("barcode/{barcode}")]
        public async Task<IActionResult> GetSockItemsByBarcode(string barcode)
        {
            try
            {
                var result = await _stockService.GetStockItemsByBarcode(barcode);

                return ApiResponse(result, "Stock Item fetech successfully");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stock item fetech by barcode");
                return ApiError(ex.Message, 500);
            }

        }


        [HttpPost("adjust")]
        public async Task<bool> AdjustStock(StockAdjustmentRequest request)
        {
            return await _stockAdjustmentService.AdjustStockAsync(request,CurrentUserId,CurrentUserName);
        }

        [HttpGet("adjust-list/{id}")]
        public async Task<StockAdjustmentResponse> GetAdjustStockList(Guid id)
        {
            return await _stockAdjustmentService.GetStockAdjustments(id);
        }
    }
}
