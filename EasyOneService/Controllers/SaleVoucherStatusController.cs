
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Textile.Core.Entities.Views;
using Textile.Core.Interfaces.Services;

namespace EasyOneService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SuperAdmin,Supplier")]
    public class SaleVoucherStatusController : BaseController
    {


        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IMediator _mediator;
        private readonly ISaleVoucherStatusService _saleVoucherStatusService;

        public SaleVoucherStatusController(IUserContextService userContextService,ILogger<WeatherForecastController> logger,
            IMediator mediator, ISaleVoucherStatusService saleVoucherStatusService):base(userContextService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _saleVoucherStatusService = saleVoucherStatusService ?? throw new ArgumentNullException(nameof(saleVoucherStatusService));
        }

        [HttpPost]
        public async Task<bool> Post(SaleVoucherStatusView request)
        {
            request.CreatedBy = CurrentUserId;
            request.CreatedByUserName = CurrentUserName;

            return await _saleVoucherStatusService.AddAsync(request);
        }

        [HttpGet("{id}")]
        public async Task<List<SaleVoucherStatusView>> GetAll(int id)
        {
            
            return await _saleVoucherStatusService.GetAll(id);
        }
    }
}
