using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Textile.Core.Entities.Models.Requests.StockGroups;
using Textile.Core.Entities.Models.Response.StockGroups;
using Textile.Core.Interfaces.Services;

namespace EasyOneService.Controllers
{
    
        [ApiController]
        [Route("api/[controller]")]
        [Authorize(Roles = "SuperAdmin,Supplier")]
    public class GstRuleController : BaseController
        {
            private readonly ILogger<GstRuleController> _logger;
            private readonly IGstRuleService _gstRuleService;

            public GstRuleController(
                IUserContextService userContextService,
                ILogger<GstRuleController> logger,
                IGstRuleService gstRuleService)
                : base(userContextService)
            {
                _gstRuleService = gstRuleService
                    ?? throw new ArgumentNullException(nameof(gstRuleService));

                _logger = logger
                    ?? throw new ArgumentNullException(nameof(logger));
            }

            // -----------------------------
            // GET ALL
            // -----------------------------
            [HttpGet]
            public async Task<IEnumerable<GstRuleDto>> GetAll()
            {
               return await _gstRuleService.GetAllAsync();
               
            }

            // -----------------------------
            // GET BY ID
            // -----------------------------
            [HttpGet("{id:int}")]
            public async Task<GstRuleDto> GetById(int id)
            {
                return await _gstRuleService.GetByIdAsync(id);
            }

            // -----------------------------
            // CREATE
            // -----------------------------
            [HttpPost]
            public async Task<bool> Create([FromBody] GstRuleRequest request)
            {
              return   await _gstRuleService.CreateAsync(
                request,
                CurrentUserId,
                CurrentUserName);
            }

            // -----------------------------
            // UPDATE
            // -----------------------------
            [HttpPut()]
            public async Task<bool> Update(int id, [FromBody] GstRuleRequest request)
            {
               return await _gstRuleService.UpdateAsync(
                request,
                CurrentUserId,
                CurrentUserName);
            }

            // -----------------------------
            // DELETE (SOFT DELETE)
            // -----------------------------
            [HttpDelete("{id:int}")]
            public async Task<bool> Delete(int id)
            {
               return await _gstRuleService.DeleteAsync(id);
            }
        }
    

}
