using MediatR;
using Microsoft.AspNetCore.Mvc;
using Textile.Core.Entities.Data;
using Textile.Core.Entities.Models.Requests;
using Textile.Core.Entities.Models.Requests.Billing;
using Textile.Core.Entities.Models.Response.Invoices;
using Textile.Core.Entities.Views;
using Textile.Core.Interfaces.Services;
using Textile.Core.Interfaces.Services.Sales;
using Textile.Core.Managers.Commands.Sales.Invoices;

namespace EasyOneService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceController : BaseController
    {
        private readonly ILogger<InvoiceController> _logger;
        private readonly IInvoiceService _invoiceService;
        private readonly IMediator _mediator;

        public InvoiceController(
            IUserContextService userContextService,
            IMediator mediator,
            ILogger<InvoiceController> logger,
            IInvoiceService invoiceService)
            : base(userContextService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _invoiceService = invoiceService ?? throw new ArgumentNullException(nameof(invoiceService));
        }


        [HttpPost("table/{finYearId}")]
        public async Task<TableResult<InvoiceListResponse>> GetTableData(TableDataRequest tableDataRequest, int finYearId)
        {

            return await _invoiceService.GetTableData(tableDataRequest, finYearId);
        }

        [HttpGet("status-counts/{financialYearId}")]
        public async Task<IActionResult> GetInvoiceStatusCounts(int financialYearId)
        {
            var result = await _invoiceService.GetInvoiceStatusCountsAsync(financialYearId);
            return ApiResponse(result);
        }

        //  CREATE INVOICE
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BillingRequest request)
        {
            try
            {
                var command = new CreateInvoiceCommand(
                    request,
                    CurrentUserName,
                    CurrentUserId);

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


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var data = await _invoiceService.GetInvoice(id);

                return ApiResponse(data, "Invoice data fetech successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching invoice: {id}");
                return ApiError("Error fetching invoice", 500);
            }
        }


        [HttpGet("{number}/{finYearId}")]
        public async Task<IActionResult> GetByNumber(
            string number,
           int finYearId)
        {
            try
            {
                var data = await _invoiceService.GetInvoice(number, finYearId);

                return ApiResponse(data, "Invoice data fetech successfully");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching invoice: {number}");
                return ApiError("Error fetching invoice", 500);
            }
        }


        [HttpDelete("cancel/{Id}")]
        public async Task<IActionResult> Cancel(int Id)
        {
            try
            {
                var currentUserId = CurrentUserId;
                var currentUserName = CurrentUserName;

                var command = new CancelInvoiceCommand(Id, currentUserName, currentUserId);

                var status = await _mediator.Send(command);
                return ApiResponse(status, "Invoice cancel successfully");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating invoice");
                return ApiError("An error occurred while creating invoice.", 500);
            }
        }
    }
}
