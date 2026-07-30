
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Textile.Core.Entities.Enums;
using Textile.Core.Managers.Commands;

namespace EasyOneService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ValuesController : ControllerBase
    {


        private readonly ILogger<ValuesController> _logger;
        private readonly IMediator _mediator;

        public ValuesController(ILogger<ValuesController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        //[HttpGet]
        //public async Task<string> Get()
        //{
        //    var command = new GenerateVoucherNumberCommand(VoucherTypeEnum.PackingSlip);
        //    return await _mediator.Send(command);
        //}
    }
}
