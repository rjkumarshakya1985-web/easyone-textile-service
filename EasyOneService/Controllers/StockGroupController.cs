using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Textile.Core.Entities.Models.Requests.Masters;
using Textile.Core.Entities.Models.Response.StockGroups;
using Textile.Core.Interfaces.Services;

namespace EasyOneService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SuperAdmin,Supplier")]
    public class StockGroupController : BaseController
    {
        private readonly ILogger<StockGroupController> _logger;
        private readonly IStockGroupService _stockGroupService;

        public StockGroupController(IUserContextService userContextService,
            ILogger<StockGroupController> logger,
            IStockGroupService stockGroupService) : base(userContextService)
        {
            _stockGroupService = stockGroupService
                ?? throw new ArgumentNullException(nameof(stockGroupService));

            _logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET: api/stock-groups
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StockGroupDto>>> Get()
        {
            var result = await _stockGroupService.GetAllAsync();
            return Ok(result);
        }

        // GET: api/stock-groups/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<StockGroupDto>> GetById(int id)
        {
            var result = await _stockGroupService.GetByIdAsync(id);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // POST: api/stock-groups
        [HttpPost]
        public async Task<bool> Create([FromBody] StockGroupRequest request)
        {
            return await _stockGroupService
                .CreateAsync(request, CurrentUserId, CurrentUserName);
        }

        [HttpPut]
        public async Task<bool> Update([FromBody] StockGroupRequest request)
        {
            return await _stockGroupService
                .UpdateAsync(request, CurrentUserId, CurrentUserName);
        }

        // DELETE: api/stock-groups/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _stockGroupService.DeleteAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }

        // PATCH: api/stock-groups/5/toggle-active
        [HttpPatch("{id:int}/toggle-active")]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var success = await _stockGroupService.ToggleActiveAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }

}
