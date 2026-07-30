
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Textile.Core.Entities.Data;
using Textile.Core.Entities.Models.Requests;
using Textile.Core.Entities.Models.Response;
using Textile.Core.Managers.Commands.Transports;
using Textile.Core.Managers.Query.Transports;

namespace EasyOneService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SuperAdmin")]
    public class TransportController : ControllerBase
    {


        private readonly ILogger<TransportController> _logger;
        private readonly IMediator _mediator;

        public TransportController(ILogger<TransportController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpPost("table")]
        public async Task<TableResult<TransportResponse>> GetTransportTableData(TableDataRequest tableDataRequest)
        {
            var query = new GetTransportTableFilterQuery(tableDataRequest);
            return await _mediator.Send(query);
        }

        [HttpGet("{id}")]
        public async Task<TransportResponse> GetTransport(int id)
        {
            var query = new GetTransportQuery(id);
            return await _mediator.Send(query);
        }

        [HttpPost("add")]
        public async Task<bool> AddTransport(TransportRequest transport)
        {
            var query = new AddTransportCommand(transport);
            return await _mediator.Send(query);
        }
    }
}
