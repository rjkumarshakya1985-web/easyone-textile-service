
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Textile.Core.Entities.Models.Response.Dashboard;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Interfaces.Services;

namespace EasyOneService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SuperAdmin,Supplier")]
    public class DashboardController : BaseController
    {


        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IMediator _mediator;
        private readonly IDashboardService _dashboardService;
        private readonly TextileDbContext _textileDbContext;

        public DashboardController(IUserContextService userContextService,
            TextileDbContext textileDbContext,
            ILogger<WeatherForecastController> logger, 
            IMediator mediator,IDashboardService dashboardService) : base(userContextService)
        {
            _textileDbContext = textileDbContext ?? throw new ArgumentNullException(nameof(textileDbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger)); 
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _dashboardService = dashboardService ?? throw new ArgumentNullException(nameof(dashboardService));
        }

        [HttpGet("supplier-dashboard")]
        public async Task<SupplierDashboardResponse> GetSupplierDashboard()
        {
            var supplierId = await GetSupplierIdAsync(_textileDbContext);
            return await _dashboardService.GetSupplierDashboard(supplierId);
        }

        [HttpGet("admin-dashboard")]
        public async Task<AdminDashboardResponse> GetAdminDashboard()
        {
            return await _dashboardService.GetAdminDashboard();
        }
    }
}
