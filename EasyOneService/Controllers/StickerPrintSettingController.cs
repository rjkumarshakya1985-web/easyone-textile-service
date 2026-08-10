using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Textile.Core.Entities.Models.Requests.StickerPrint;
using Textile.Core.Entities.Models.Response.StickerPrint;
using Textile.Core.Interfaces.Services;

namespace EasyOneService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SuperAdmin")]
    public class StickerPrintSettingController : BaseController
    {
        private readonly IStickerPrintSettingService _stickerPrintSettingService;

        public StickerPrintSettingController(
            IUserContextService userContextService,
            IStickerPrintSettingService stickerPrintSettingService) : base(userContextService)
        {
            _stickerPrintSettingService = stickerPrintSettingService ?? throw new ArgumentNullException(nameof(stickerPrintSettingService));
        }

        [HttpGet]
        public async Task<StickerPrintSettingResponse> Get()
        {
            return await _stickerPrintSettingService.GetAsync();
        }

        [HttpPost]
        public async Task<bool> Save(StickerPrintSettingRequest request)
        {
            return await _stickerPrintSettingService.SaveAsync(request, CurrentUserId);
        }
    }
}
