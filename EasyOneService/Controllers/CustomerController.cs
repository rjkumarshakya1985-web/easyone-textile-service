using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Textile.Core.Entities.Data;
using Textile.Core.Entities.Models.Requests;
using Textile.Core.Entities.Models.Requests.Customers;
using Textile.Core.Entities.Models.Response.Customers;
using Textile.Core.Entities.Models.Response.Visitors;
using Textile.Core.Interfaces.Services;
using Textile.Core.Managers.Commands.Customers;
using Textile.Core.Managers.Common.Exceptions;
using Textile.Core.Managers.Query.Customers;

namespace EasyOneService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SuperAdmin,Supplier,StockIncharge,Cashier, PackingSlipOperator")]
    public class CustomerController : BaseController
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<CustomerController> _logger;
        private readonly IMediator _mediator;
        public CustomerController(IUserContextService userContextService,
            ICustomerService customerService,
            ILogger<CustomerController> logger, IMediator mediator) : base(userContextService)
        {
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        // -------------------------
        // CREATE
        // -------------------------
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CustomerRequest request)
        {
            try
            {
                var customerId = await _customerService.CreateAsync(request, CurrentUserId, CurrentUserName);
                var customer = await _customerService.GetByIdAsync(customerId);

                return ApiResponse(customer, "Customer created successfully.");
            }
            catch (DuplicateEntityException ex)
            {
                return ApiError(ex.Message, StatusCodes.Status409Conflict);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating customer");
                return ApiError(ex.Message, 500);
            }
        }


        // -------------------------
        // UPDATE
        // -------------------------
        [HttpPut("{id:guid}")]
        public async Task<bool> Update(Guid id, [FromBody] CustomerRequest request)
        {
            if (id != request.Id)
                throw new Exception("Invalid customer id");

            var userId = CurrentUserId;
            var userName = CurrentUserName;

            return await _customerService.UpdateAsync(request, userId, userName);

        }

        // -------------------------
        // DELETE (Soft delete)
        // -------------------------
        [HttpDelete("{id:guid}")]
        public async Task<bool> Delete(Guid id)
        {
            return await _customerService.DeleteAsync(id);
            
        }

        // -------------------------
        // GET BY ID
        // -------------------------
        [HttpGet("{id:guid}")]
        public async Task<CustomerResponse?> GetById(Guid id)
        {
            return await _customerService.GetByIdAsync(id);
           
        }        

        // -------------------------
        // GET Customer BY Mobile Number
        // -------------------------
        [HttpGet("mobile-lookup/{mobile}")]
        public async Task<CustomerResponse> GetByMobile(string mobile)
        {
            var query = new GetCustomerMobileQuery(mobile);
            return await _mediator.Send(query);
        }
     
        // -------------------------
        // PAGINATION / TABLE
        // -------------------------
        [HttpPost("table")]
        public async Task<TableResult<CustomerResponse>> GetTableData([FromBody] TableDataRequest request)
        {
            var result = await _customerService.GetTableData(request);
            return result;
        }
        [HttpPost("update-status-customer")]
        public async Task<bool> UpdateStatusCustomer(UpdateCustomerStatusRequest updateCustomer)
        {
            var command = new UpdateCustomerStatusCommand(updateCustomer);

            return await _mediator.Send(command);

        }

        [HttpPost("mobile/create-supplier/{visitorId}")]
        public async Task<VisitorResponse> CreateSupplier(int visitorId, [FromBody] CustomerRequest request)
        {
            var userId = CurrentUserId;
            var userName = CurrentUserName;

            var command = new VisitorSupplierCommand(request,userId,userName, visitorId);

            var result =   await _mediator.Send(command);

            return result;
        }



        // -------------------------
        // GET BY ID
        // -------------------------
        [HttpGet("billing-customers")]
        public async Task<IActionResult> GetBillingCustomers()
        {
            try
            {
                var result = await _customerService.GetBillingCustomers();
                return ApiResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching billing customers");
                return ApiError(ex.Message, 500);
            }
        }

    }

}
