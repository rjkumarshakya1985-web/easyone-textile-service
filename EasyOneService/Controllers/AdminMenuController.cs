using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Textile.Core.Entities.Models.Requests.AdminMenu;
using Textile.Core.Entities.Models.Response.AdminMenu;
using Textile.Core.Interfaces.Services;

namespace EasyOneService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SuperAdmin")]
    public class AdminMenuController : BaseController
    {
        private readonly IAdminMenuService _adminMenuService;

        public AdminMenuController(
            IUserContextService userContextService,
            IAdminMenuService adminMenuService) : base(userContextService)
        {
            _adminMenuService = adminMenuService ?? throw new ArgumentNullException(nameof(adminMenuService));
        }

        [HttpGet]
        public async Task<List<AdminMenuSettingResponse>> Get()
        {
            return await _adminMenuService.GetAsync();
        }

        [HttpPost]
        public async Task<bool> Save(AdminMenuSettingRequest request)
        {
            return await _adminMenuService.SaveAsync(request, CurrentUserId);
        }
    }
}
