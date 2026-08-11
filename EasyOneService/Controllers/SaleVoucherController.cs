
using Azure.Core;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Textile.Core.Entities.Data;
using Textile.Core.Entities.Enums;
using Textile.Core.Entities.Models.Requests;
using Textile.Core.Entities.Models.Requests.SaleVouchers;
using Textile.Core.Entities.Models.Response.SaleVouchers;
using Textile.Core.Interfaces.Services;
using Textile.Core.Managers.Commands.SaleVouchers;
using Textile.Core.Managers.Query.SaleVouchers;
using Textile.Core.Managers.Services;

namespace EasyOneService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SuperAdmin,Supplier")]
    public class SaleVoucherController : BaseController
    {


        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IMediator _mediator;
        private readonly ISaleVoucherService _saleVoucherService;
        private readonly ISupplierContextService _supplierContextService;
        private readonly ISupplierProductService _supplierProductService;

        public SaleVoucherController(ISupplierProductService supplierProductService,IUserContextService userContextService,
                ISupplierContextService supplierContextService, ILogger<WeatherForecastController> logger,
            ISaleVoucherService saleVoucherService, IMediator mediator) : base(userContextService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger)); ;
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _saleVoucherService = saleVoucherService ?? throw new ArgumentNullException(nameof(saleVoucherService));
            _supplierContextService = supplierContextService ?? throw new ArgumentNullException(nameof(supplierContextService));

            _supplierProductService = supplierProductService ?? throw new ArgumentNullException(nameof(supplierProductService));
        }

        [HttpPost("salevoucher-table")]
        public async Task<TableResult<SaleVoucherTableResponse>> GetSaleVoucherTableData(TableDataRequest tableDataRequest)
        {
            Guid? supplierId = null;
            if (CurrentUserRole == RoleEnum.Supplier)
            {
                supplierId = await _supplierContextService.GetSupplierIdAsync();
            }
            return await _saleVoucherService.GetTableData(tableDataRequest, supplierId);
        }

        [HttpPost("mobile/salevoucher-list")]
        public async Task<TableResult<SaleVoucherMobileResponse>> GetMobileSaleVoucherList(TableDataRequest tableDataRequest)
        {
            Guid? supplierId = null;
            if (CurrentUserRole == RoleEnum.Supplier)
            {
                supplierId = await _supplierContextService.GetSupplierIdAsync();
            }

            return await _saleVoucherService.GetMobileTableData(tableDataRequest, supplierId);
        }

        [HttpGet("mobile/{id}/products")]
        public async Task<List<SaleVoucherMobileProductResponse>> GetMobileSaleVoucherProducts(int id)
        {
            Guid? supplierId = null;
            if (CurrentUserRole == RoleEnum.Supplier)
            {
                supplierId = await _supplierContextService.GetSupplierIdAsync();
            }

            return await _saleVoucherService.GetMobileProductsAsync(id, supplierId);
        }

        [HttpGet("{id}")]
        public async Task<SaleVoucherResponse> GetSaleVoucher(int id)
        {
            var query = new GetSaleVoucherDetailByIdQuery(id);
            return await _mediator.Send(query);
        }

        [HttpPost("create")]
        public async Task<int> CreateSaleVoucher(SaleVoucherRequest saleVoucherRequest)
        {

            if (CurrentUserRole == RoleEnum.Supplier)
            {
                saleVoucherRequest.SupplierId = await _supplierContextService.GetSupplierIdAsync();
            }

          
            var command = new CreateSaleVoucherCommand(saleVoucherRequest, CurrentUserId, CurrentUserName);
            var result = await _mediator.Send(command);

            if (saleVoucherRequest.Status == (int)ParcelStatusEnum.PackedAtLocation)
            {
                List<int> ints = new List<int>();
                ints.Add(result);
                await _supplierProductService.UpdateProductPriceHistoryAsync(ints);
            }
            return result;
        }

        [HttpPost("update")]
        public async Task<int> UpdateSaleVoucher(SaleVoucherRequest saleVoucherRequest)
        {

            if (CurrentUserRole == RoleEnum.Supplier)
            {
                saleVoucherRequest.SupplierId = await _supplierContextService.GetSupplierIdAsync();
            }
            var command = new UpdateSaleVoucherCommand(saleVoucherRequest, CurrentUserId, CurrentUserName);
            return await _mediator.Send(command);

        }

        [HttpDelete("{id}")]
        public async Task<bool> DeleteSaleVoucher(int id)
        {
            return await _saleVoucherService.DeleteAsync(id, CurrentUserId, CurrentUserName);
        }

        [HttpPut("{id}")]
        public async Task<SaleVoucherDto> IsExported(int id)
        {
            return await _saleVoucherService.IsExport(id);
        }

        [HttpGet("export")]
        public async Task<IEnumerable<SaleVoucherDto>> GetExportSaleVoucher()
        {
            return await _saleVoucherService.GetAllExportAsync();
        }


        [HttpPost("lr")]
        public async Task<bool> SaveLr(LrRequest request)
        {
            
            return await _saleVoucherService.SaveLR(request, CurrentUserId, CurrentUserName);
        }
    }
}
