using Microsoft.AspNetCore.Mvc;
using Textile.Core.Entities.Enums;
using Textile.Core.Entities.Models.Requests;
using Textile.Core.Entities.Models.Response.Parcels;
using Textile.Core.Interfaces.Services;

namespace EasyOneService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParcelController : BaseController
    {
        private readonly IParcelService _parcelService;
        private readonly ISupplierProductService _supplierProductService;
        public ParcelController(ISupplierProductService supplierProductService, IUserContextService userContextService, IParcelService parcelService, ILogger<PrintController> logger) : base(userContextService)
        {
            _supplierProductService = supplierProductService ?? throw new ArgumentNullException(nameof(supplierProductService));
            _parcelService = parcelService ?? throw new ArgumentNullException(nameof(parcelService));
        }

        [HttpGet("scan-info/{id}/status/{status}")]
        public async Task<ParcelResponse> GetParcelScanInfoAsync(int id,ParcelStatusEnum status) 
        {
            return await _parcelService.GetParcelScanInfoAsync(id, status);
        }

        [HttpPut("change")]
        public async Task<bool> ChangeParcelStatus(ParcelScanRequest request)
        {
            if(request.StatusEnum==ParcelStatusEnum.PackedAtLocation)
            {
                await _supplierProductService.UpdateProductPriceHistoryAsync(request.SaleVoucherId);
            }
            return await _parcelService.ChangeSaleVouchersStatus(request,CurrentUserId,CurrentUserName);
        }


        [HttpPut("movesalevouchertostock")]
        public async Task<bool> MoveSaleVoucherProductsToStockAsync(ParcelScanRequest request)
        {
             await _parcelService.MoveSaleVoucherProductsToStockAsync(request, CurrentUserId, CurrentUserName);

            return await _parcelService.ChangeSaleVouchersStatus(request, CurrentUserId, CurrentUserName);
        }
    }
}
