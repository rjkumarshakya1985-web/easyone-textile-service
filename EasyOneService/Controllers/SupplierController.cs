using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Textile.Core.Entities.Data;
using Textile.Core.Entities.Enums;
using Textile.Core.Entities.Models.Requests;
using Textile.Core.Entities.Models.Requests.Suppliers;
using Textile.Core.Entities.Models.Response;
using Textile.Core.Entities.Models.Response.Masters;
using Textile.Core.Entities.Models.Response.Suppliers;
using Textile.Core.Interfaces.Services;
using Textile.Core.Managers.Commands.Suppliers;
using Textile.Core.Managers.Query.Suppliers;

namespace EasyOneService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SuperAdmin,Supplier")]
    public class SupplierController : BaseController
    {


        private readonly ILogger<SupplierController> _logger;
        private readonly IMediator _mediator;
        private readonly ISupplierContextService _supplierContextService;
        private readonly ISupplierStockGroupService _supplierStockGroupService;
        private readonly ISupplierHsnCodeService _supplierHsnCodeService;

        public SupplierController(IUserContextService userContextService,
           ISupplierStockGroupService supplierStockGroupService,
           ISupplierHsnCodeService supplierHsnCodeService,
           ISupplierContextService supplierContextService, ILogger<SupplierController> logger,
           IMediator mediator) : base(userContextService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _supplierStockGroupService = supplierStockGroupService ?? throw new ArgumentNullException(nameof(supplierStockGroupService));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _supplierContextService = supplierContextService ?? throw new ArgumentNullException(nameof(supplierContextService));
            _supplierHsnCodeService = supplierHsnCodeService ?? throw new ArgumentNullException(nameof(supplierHsnCodeService));
        }

        [HttpPost("supplier-table")]
        public async Task<TableResult<SupplierTableResponse>> GetSupplierTableData(TableDataRequest tableDataRequest)
        {
            var query = new GetSupplierTableFilterQuery(tableDataRequest);
            return await _mediator.Send(query);
        }




        [HttpGet("supplier-code")]
        public async Task<IActionResult> GetSupplierCode()
        {
            var query = new GetNewSupplierCodeQuery();
            var code = await _mediator.Send(query);
            return ApiResponse(code);
        }


        [HttpPost("create-supplier")]
        public async Task<IActionResult> CreateSupplier(SupplierRequest supplierRequest)
        {
            var command = new CreateSupplierCommand(supplierRequest, CurrentUserId, CurrentUserName);

            var supplierId = await _mediator.Send(command);

            return Ok(new
            {
                Success = true,
                Message = "Supplier created successfully",
                Data = supplierId
            });
        }

        [HttpPost("update-status-supplier")]
        public async Task<bool> UpdateStatusSupplier(UpdateSupplierStatusRequest updateSupplier)
        {
            var command = new UpdateSupplierStatusCommand(updateSupplier);

            return await _mediator.Send(command);

        }


        [HttpGet("supplier-detail/{supplierId}")]
        public async Task<SupplierDTO> GetSupplierById(Guid supplierId)
        {
            var query = new GetSupplierByIdQuery(supplierId);
            return await _mediator.Send(query);

        }

        [HttpGet("current-supplier")]
        public async Task<SupplierDTO> GetCurrentSupplier()
        {
            var supplierId = await _supplierContextService.GetSupplierIdAsync();
            var query = new GetSupplierByIdQuery(supplierId);
            return await _mediator.Send(query);

        }

        #region Supplier Transport

        [HttpPost("supplier-transport-mapping-table")]
        public async Task<TableResult<SupplierTransportResponse>> GetSupplierTransportMappings(TableDataRequest tableDataRequest)
        {
            var query = new GetSupplierTransportTableFilterQuery(tableDataRequest);
            return await _mediator.Send(query);
        }

        [HttpPost("supplier-transport-delete")]
        public async Task<bool> SupplierTransportDelete(SupplierTransportDeleteRequest supplierTransport)
        {
            var query = new SupplierTransportDeleteCommand(supplierTransport);
            return await _mediator.Send(query);
        }

        [HttpGet("supplier-transports/{id?}")]
        public async Task<IEnumerable<TransportResponse>> SupplierTransports(Guid? id)
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

            var query = new GetAllSupplierTransportsQuery(supplierId);
            return await _mediator.Send(query);
        }

        [HttpPost("assign-supplier-transport")]
        public async Task<bool> AssignSupplierTransport(AddSupplierTransportRequest addSupplierTransport)
        {
            var command = new AddSupplierTransportCommand(addSupplierTransport);
            return await _mediator.Send(command);
        }

        #endregion


        #region Supplier Stock Group

        [HttpPost("supplier-stockgroup-mapping-table")]
        public async Task<TableResult<SupplierStockGroupResponse>> GetSupplierStockGroupMappings(TableDataRequest tableDataRequest)
        {
            return await _supplierStockGroupService.GetSupplierStockGroupMappings(tableDataRequest);

        }

        [HttpGet("orphan-stockgroup/{supplierid}")]
        public async Task<IEnumerable<StockGroupResponse>> GetOprhanStockGroup(Guid supplierid)
        {
            return await _supplierStockGroupService.GetSupplierOrphanStockGroups(supplierid);

        }

        [HttpPost("supplier-stockgroup-delete")]
        public async Task<bool> SupplierStockGroupDelete(SupplierStockGroupDeleteRequest supplierStockGroupDeleteRequest)
        {
            return await _supplierStockGroupService.SupplierStockGroupDelete(supplierStockGroupDeleteRequest);
        }




        [HttpGet("supplier-stockgroups/{id?}")]
        public async Task<IEnumerable<StockGroupResponse>> SupplierStockGroups(Guid? id)
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

            return await _supplierStockGroupService.SupplierStockGroups(supplierId);
        }


        [HttpPost("assign-supplier-stockgroup")]
        public async Task<bool> AssignSupplierStockGroup(AddSupplierStockGroupRequest addSupplierStockGroupRequest)
        {
            return await _supplierStockGroupService.AssignSupplierStockGroup(addSupplierStockGroupRequest);
        }

        #endregion


        #region Supplier Hsn Code

        [HttpPost("supplier-hsncode-mapping-table")]
        public async Task<TableResult<SupplierHsnCodeResponse>> GetSupplierHsnCodeMappings(TableDataRequest tableDataRequest)
        {
            return await _supplierHsnCodeService.GetSupplierHsnCodeMappings(tableDataRequest);

        }

        [HttpGet("orphan-hsncode/{supplierid}/{stockGroupId}/{search}")]
        public async Task<IEnumerable<HsnCodeResponse>> GetOprhanHsnCode(Guid supplierid, int stockGroupId, string search)
        {
            return await _supplierHsnCodeService.GetSupplierOrphanHsnCodes(supplierid, stockGroupId, search);

        }

        [HttpPost("supplier-hsncode-delete")]
        public async Task<bool> SupplierHsnCodeDelete(SupplierHsnCodeRequest request)
        {
            return await _supplierHsnCodeService.SupplierHsnCodeDelete(request);
        }

        [HttpGet("supplier-hsncodes/{stockGroupId}/{supplierId?}")]
        public async Task<IEnumerable<HsnCodeResponse>> SupplierHsnCodes(int stockGroupId,Guid? supplierId)
        {
            Guid finalSupplierId;

            if (CurrentUserRole == RoleEnum.Supplier)
            {
                // Supplier khud ka hi data dekhe
                finalSupplierId = await _supplierContextService.GetSupplierIdAsync();
            }
            else
            {
               
                if (!supplierId.HasValue)
                    return Enumerable.Empty<HsnCodeResponse>(); // ya BadRequest()

                finalSupplierId = supplierId.Value;
            }

            return await _supplierHsnCodeService
                .GetSupplierStockGroupHsnCodes(finalSupplierId, stockGroupId);
        }


        [HttpPost("assign-hsncode-stockgroup")]
        public async Task<bool> AssignSupplierHsnCode(SupplierHsnCodeRequest request)
        {
            return await _supplierHsnCodeService.AssignSupplierHsnCode(request);
        }

        #endregion
    }
}
