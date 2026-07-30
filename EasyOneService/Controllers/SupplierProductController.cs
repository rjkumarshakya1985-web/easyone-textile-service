
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Textile.Core.Entities.Data;
using Textile.Core.Entities.Enums;
using Textile.Core.Entities.Models.Requests;
using Textile.Core.Entities.Models.Requests.Suppliers;
using Textile.Core.Entities.Models.Response.Suppliers;
using Textile.Core.Interfaces.Services;
using Textile.Core.Managers.Query.Suppliers;

namespace EasyOneService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SuperAdmin,Supplier")]
    public class SupplierProductController : BaseController
    {
        private readonly ILogger<SupplierProductController> _logger;
        private readonly ISupplierProductService _supplierProductService;
        private readonly ISupplierContextService _supplierContextService;

        public SupplierProductController(
            IUserContextService userContextService,
            ISupplierContextService supplierContextService,
            ILogger<SupplierProductController> logger,
            ISupplierProductService supplierProductService) : base(userContextService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _supplierProductService = supplierProductService ?? throw new ArgumentNullException(nameof(supplierProductService));
            _supplierContextService = supplierContextService ?? throw new ArgumentNullException(nameof(supplierContextService));
        }


        [HttpPost("supplier-product-table")]
        public async Task<TableResult<SupplierProductDto>> GetSupplierProductTableData(TableDataRequest tableDataRequest)
        {
            Guid? supplierId = null;
            if (CurrentUserRole == RoleEnum.Supplier)
            {
                supplierId = await _supplierContextService.GetSupplierIdAsync();
            }
            return await _supplierProductService.GetTableData(tableDataRequest, supplierId);
        }

        // GET: api/supplierproduct
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SupplierProductDto>>> Get()
        {
            var result = await _supplierProductService.GetAllAsync();
            return Ok(result);
        }



        // GET: api/supplierproduct/5
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<SupplierProductDto>> GetById(Guid id)
        {
            var result = await _supplierProductService.GetByIdAsync(id);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // GET: api/supplierproduct/5
        [HttpGet("product-view/{id:guid}")]
        public async Task<ActionResult<SupplierProductDto>> GetProductViewById(Guid id)
        {
            var result = await _supplierProductService.GetProductViewByIdAsync(id);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// Get Supplier Product Code
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// 



        [HttpGet("fetch-code")]
        public async Task<IActionResult> GetSupplierProductCode()
        {
            var query = new GetNewSupplierCodeQuery();
            var code = await _supplierProductService.FetchNextBarcodeNumber(); ;
            return ApiResponse(code);
        }
        // POST: api/supplierproduct
        [HttpPost]
        public async Task<bool> Create([FromBody] SupplierProductRequest request)
        {

            if (CurrentUserRole == RoleEnum.Supplier)
            {
                request.SupplierId = await _supplierContextService.GetSupplierIdAsync();
            }

            try
            {

                
                return await _supplierProductService
                    .CreateAsync(request, CurrentUserId, CurrentUserName);


            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is SqlException sqlEx &&
                    (sqlEx.Number == 2601 || sqlEx.Number == 2627))
                {
                    throw new Exception($"Product {request.Name} already exists.");
                }

                throw;
            }
        }

        // PUT: api/supplierproduct
        [HttpPut]
        public async Task<bool> Update([FromBody] SupplierProductRequest request)
        {
            try
            {
                return await _supplierProductService.UpdateAsync(request, CurrentUserId, CurrentUserName, CurrentUserRole);
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is SqlException sqlEx &&
                    (sqlEx.Number == 2601 || sqlEx.Number == 2627))
                {
                    throw new Exception($"Product {request.Name} already exists.");
                }

                throw;
            }


        }

        // DELETE: api/supplierproduct/5
        [HttpDelete("{id:guid}")]
        public async Task<bool> Delete(Guid id)
        {
            return await _supplierProductService.DeleteAsync(id);

        }

        // PATCH: api/supplierproduct/5/toggle-active
        [HttpPatch("{id:guid}/toggle-active")]
        public async Task<bool> ToggleActive(Guid id)
        {
            return await _supplierProductService.ToggleActiveAsync(id);

        }


    
        [HttpGet("product-price-history/{productId:guid}")]
        public async Task<IActionResult> GetProductPriceHistory(Guid productId)
        {
            var result = await _supplierProductService.GetProductPriceHistory(productId);

            return ApiResponse(result, "Product Price History Fetched Successfully");
        }

        [HttpDelete("product-price-history/{historyId:int}")]
        public async Task<IActionResult> DeleteProductPriceHistory(int historyId)
        {
            var result = await _supplierProductService.DeleteProductPriceHistoryAsync(historyId);

            if (!result)
                return NotFound("Product Price History not found.");

            return ApiResponse(result, "Product Price History Deleted Successfully");
        }


    }

}
