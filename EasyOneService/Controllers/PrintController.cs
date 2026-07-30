
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Textile.Core.Entities.Models.Response.Suppliers.Print;
using Textile.Core.Interfaces.Services;
using Textile.Core.Managers.Query.SaleVouchers.Print;

namespace EasyOneService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SuperAdmin,Supplier")]
    public class PrintController : ControllerBase
    {


        private readonly ILogger<PrintController> _logger;
        private readonly IMediator _mediator;
        private readonly IPrintService _printService;

        public PrintController(ILogger<PrintController> logger,
            IMediator mediator,IPrintService printService)
        {
            _logger = logger;
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _printService = printService ?? throw new ArgumentNullException(nameof(printService));
        }

        [HttpGet("{id}")]
        public async Task<SaleVoucherPrintResponse> SaleVoucherPrint(int id)
        {

            var query = new GetSaleVoucherStickerPrintQuery(id);
            return await _mediator.Send(query);
        }

        [HttpGet("product-barcode-sticker/{id}")]
        public async Task<StickerPrint> ProductBarcodeSticker(Guid id, bool isSaleVoucher = false)
        {

            return await _printService.GetStickerByProduct(id,isSaleVoucher);
        }

    }
}
