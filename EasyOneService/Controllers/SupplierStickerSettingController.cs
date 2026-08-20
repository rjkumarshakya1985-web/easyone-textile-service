using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Textile.Core.Entities.Models.Requests.StickerPrint;
using Textile.Core.Entities.Models.Response.StickerPrint;
using Textile.Core.Interfaces.Services;

namespace EasyOneService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Supplier")]
    public class SupplierStickerSettingController : BaseController
    {
        private readonly ISupplierContextService _supplierContextService;
        private readonly ISupplierStickerSettingService _supplierStickerSettingService;
        private readonly IStickerPrintSettingService _stickerPrintSettingService;

        public SupplierStickerSettingController(
            IUserContextService userContextService,
            ISupplierContextService supplierContextService,
            ISupplierStickerSettingService supplierStickerSettingService,
            IStickerPrintSettingService stickerPrintSettingService) : base(userContextService)
        {
            _supplierContextService = supplierContextService ?? throw new ArgumentNullException(nameof(supplierContextService));
            _supplierStickerSettingService = supplierStickerSettingService ?? throw new ArgumentNullException(nameof(supplierStickerSettingService));
            _stickerPrintSettingService = stickerPrintSettingService ?? throw new ArgumentNullException(nameof(stickerPrintSettingService));
        }

        [HttpGet("my")]
        public async Task<SupplierStickerSizeSettingResponse> GetMySetting()
        {
            var supplierId = await _supplierContextService.GetSupplierIdAsync();
            return await _supplierStickerSettingService.GetAsync(supplierId);
        }

        [HttpPost("my")]
        public async Task<bool> SaveMySetting(SupplierStickerSizeSettingRequest request)
        {
            var supplierId = await _supplierContextService.GetSupplierIdAsync();
            return await _supplierStickerSettingService.SaveAsync(supplierId, request);
        }

        [HttpGet("demo")]
        public async Task<StickerPrintSettingResponse> GetDemoSetting()
        {
            return await _stickerPrintSettingService.GetForPrintAsync();
        }
    }
}
