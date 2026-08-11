using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Textile.Core.Entities.Models.Requests.SaleVoucherPrintDetails;
using Textile.Core.Entities.Models.Response.SaleVoucherPrintDetails;
using Textile.Core.Interfaces.Services;

namespace EasyOneService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SuperAdmin")]
    public class SaleVoucherPrintDetailController : BaseController
    {
        private readonly ISaleVoucherPrintDetailService _saleVoucherPrintDetailService;

        public SaleVoucherPrintDetailController(
            IUserContextService userContextService,
            ISaleVoucherPrintDetailService saleVoucherPrintDetailService) : base(userContextService)
        {
            _saleVoucherPrintDetailService = saleVoucherPrintDetailService ?? throw new ArgumentNullException(nameof(saleVoucherPrintDetailService));
        }

        [HttpGet]
        public async Task<SaleVoucherPrintDetailResponse> Get()
        {
            return await _saleVoucherPrintDetailService.GetAsync();
        }

        [HttpPost]
        public async Task<bool> Save(SaleVoucherPrintDetailRequest request)
        {
            return await _saleVoucherPrintDetailService.SaveAsync(request, CurrentUserId);
        }
    }
}
