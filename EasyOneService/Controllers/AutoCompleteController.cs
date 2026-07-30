
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Textile.Core.Entities.Enums;
using Textile.Core.Entities.Models.Response;
using Textile.Core.Entities.Models.Response.Agents;
using Textile.Core.Entities.Models.Response.Suppliers;
using Textile.Core.Entities.Views;
using Textile.Core.Interfaces.Services;
using Textile.Core.Managers.Query.AutoComplete;

namespace EasyOneService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AutoCompleteController : BaseController
    {


        private readonly ILogger<AutoCompleteController> _logger;
        private readonly IMediator _mediator;
        private readonly ISupplierContextService _supplierContextService;

        public AutoCompleteController(IUserContextService userContextService,
            ILogger<AutoCompleteController> logger,
            ISupplierContextService supplierContextService,
            IMediator mediator) : base(userContextService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _supplierContextService = supplierContextService ?? throw new ArgumentNullException(nameof(supplierContextService));
        }

        [HttpGet("supplier-search/{search}")]
        public async Task<IEnumerable<SupplierTableResponse>> GetSupplierAutoComplete(string search)
        {
            var command = new GetSupplierAutoCompleteQuery(search);
            return await _mediator.Send(command);
        }

        [HttpGet("orphan-transport-search/{search}/{supplierid}")]
        public async Task<IEnumerable<TransportResponse>> GetSupplierOrphanTransportAutoComplete(string search, Guid supplierid)
        {
            var command = new GetOrphanTransportsQuery(supplierid, search);
            return await _mediator.Send(command);
        }

        [HttpGet("hsn-code-search/{search}")]
        public async Task<IEnumerable<HsnCodeResponse>> GetHsnCodeAutoComplete(string search)
        {
            var command = new GetHsnAutoCompleteQuery(search);
            return await _mediator.Send(command);
        }

        
        [HttpGet("supplier-product-search/{search}")]
        public async Task<IEnumerable<SupplierProductView>> GetSupplierProductAutoCompleteQuery(string search, [FromQuery] Guid? id)
        {

            Guid supplierId;

            if (CurrentUserRole == RoleEnum.Supplier)
            {
                supplierId = await _supplierContextService.GetSupplierIdAsync();
            }
            else
            {
                if (!id.HasValue)
                    throw new Exception("SupplierId is required.");

                supplierId = id.Value;
            }

            var query = new GetSupplierProductAutoCompleteQuery(
                supplierId,
                search
            );

            return await _mediator.Send(query);

        }

        [HttpGet("agent-search/{search}")]
        public async Task<IEnumerable<AgentTableResponse>> GetAgentAutoComplete(string search)
        {
            var command = new GetAgentAutoCompleteQuery(search);
            return await _mediator.Send(command);
        }

        [HttpGet("customer-agent-search/{search}")]
        public async Task<IEnumerable<AgentTableResponse>> GetCustomerAgentAutoComplete(string search)
        {
            var query = new GetCustomerAgentAutoCompleteQuery(search);
            return await _mediator.Send(query);
        }
    }
}
