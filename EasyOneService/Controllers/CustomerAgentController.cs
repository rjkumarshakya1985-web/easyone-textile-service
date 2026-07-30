using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Textile.Core.Entities.Data;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Enums;
using Textile.Core.Entities.Models.Requests;
using Textile.Core.Entities.Models.Requests.Agents;
using Textile.Core.Entities.Models.Response.Agents;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Interfaces.Services;

namespace EasyOneService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "SuperAdmin")]
    public class CustomerAgentController : BaseController
    {
        private readonly TextileDbContext _context;

        public CustomerAgentController(IUserContextService userContextService, TextileDbContext context)
            : base(userContextService)
        {
            _context = context;
        }

        [HttpPost("customer-agent-table")]
        public async Task<TableResult<AgentTableResponse>> GetTable(TableDataRequest request)
        {
            var query = _context.CustomerAgents.AsNoTracking()
                .Where(x => !x.IsDeleted)
                .Include(x => x.City)
                .ThenInclude(x => x.State)
                .Select(x => new AgentTableResponse
                {
                    Id = x.Id, Code = x.Code, Name = x.Name,
                    ContactPersonName = x.ContactPersonName,
                    ContactPersonMobile = x.ContactPersonMobile,
                    GSTIN = x.GSTIN, PAN = x.PAN,
                    City = x.City != null ? x.City.Name : null,
                    State = x.City != null && x.City.State != null ? x.City.State.Name : null,
                    Address = x.Address, TallyLedgerName = x.TallyLedgerName,
                    Area = x.Area, Pincode = x.Pincode,
                    IsActive = x.IsActive, IsDeleted = x.IsDeleted
                });

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(x =>
                    (x.Name ?? string.Empty).ToLower().Contains(search) ||
                    (x.Code ?? string.Empty).ToLower().Contains(search) ||
                    (x.ContactPersonName ?? string.Empty).ToLower().Contains(search) ||
                    (x.GSTIN ?? string.Empty).ToLower().Contains(search) ||
                    (x.PAN ?? string.Empty).ToLower().Contains(search));
            }

            var total = await query.CountAsync();
            query = ApplySorting(query, request.SortField, request.SortOrder);
            var result = await query.Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize).ToListAsync();

            return new TableResult<AgentTableResponse> { TotalRows = total, Result = result };
        }

        [HttpPost("create-customer-agent")]
        public async Task<IActionResult> Save(AgentRequest request)
        {
            CustomerAgent entity;
            if (request.Id.HasValue)
            {
                entity = await _context.CustomerAgents.FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted)
                    ?? throw new Exception("Customer Agent not found.");
                MapFields(entity, request);
                entity.ModifiedBy = CurrentUserId;
                entity.ModifiedByUserName = CurrentUserName;
                entity.ModifiedOn = DateTime.UtcNow;
            }
            else
            {
                entity = new CustomerAgent
                {
                    Id = Guid.NewGuid(), CreatedBy = CurrentUserId,
                    CreatedByUserName = CurrentUserName, CreatedOn = DateTime.UtcNow,
                    IsActive = true, IsDeleted = false
                };
                MapFields(entity, request);
                _context.CustomerAgents.Add(entity);
            }

            await _context.SaveChangesAsync();
            return Ok(new { Success = true, Data = entity.Id });
        }

        [HttpGet("customer-agent-detail/{id}")]
        public async Task<AgentDTO?> Get(Guid id)
        {
            return await _context.CustomerAgents.AsNoTracking()
                .Where(x => x.Id == id && !x.IsDeleted)
                .Select(x => new AgentDTO
                {
                    Id = x.Id, Code = x.Code, Name = x.Name,
                    ContactPersonName = x.ContactPersonName,
                    ContactPersonMobile = x.ContactPersonMobile,
                    GSTIN = x.GSTIN, PAN = x.PAN, CityId = x.CityId,
                    StateId = x.City != null ? x.City.StateId : null,
                    Email = x.Email, Pincode = x.Pincode,
                    TallyLedgerName = x.TallyLedgerName, Area = x.Area, Address = x.Address
                }).FirstOrDefaultAsync();
        }

        [HttpPost("update-status-customer-agent")]
        public async Task<bool> UpdateStatus(UpdateAgentStatusRequest request)
        {
            var entity = await _context.CustomerAgents.FirstOrDefaultAsync(x => x.Id == request.AgentId)
                ?? throw new Exception("Customer Agent not found.");

            switch (request.ActionType)
            {
                case AgentStatusActionType.Delete:
                    entity.IsDeleted = true; entity.IsActive = false; break;
                case AgentStatusActionType.Activate:
                    entity.IsDeleted = false; entity.IsActive = true; break;
                case AgentStatusActionType.Deactivate:
                    entity.IsActive = false; break;
                default: throw new Exception("Invalid action type.");
            }

            await _context.SaveChangesAsync();
            return true;
        }

        private static void MapFields(CustomerAgent entity, AgentRequest request)
        {
            entity.Name = request.Name;
            entity.Code = request.Code;
            entity.ContactPersonName = request.ContactPersonName;
            entity.ContactPersonMobile = request.ContactPersonMobile;
            entity.GSTIN = request.GSTIN;
            entity.PAN = request.PAN;
            entity.CityId = request.CityId;
            entity.Email = request.Email;
            entity.Pincode = request.Pincode;
            entity.TallyLedgerName = request.TallyLedgerName;
            entity.Area = request.Area;
            entity.Address = request.Address;
        }

        private static IQueryable<AgentTableResponse> ApplySorting(
            IQueryable<AgentTableResponse> query, string? field, int order)
        {
            return (field?.ToLower(), order) switch
            {
                ("name", 1) => query.OrderBy(x => x.Name),
                ("name", -1) => query.OrderByDescending(x => x.Name),
                ("contactpersonname", 1) => query.OrderBy(x => x.ContactPersonName),
                ("contactpersonname", -1) => query.OrderByDescending(x => x.ContactPersonName),
                ("contactpersonmobile", 1) => query.OrderBy(x => x.ContactPersonMobile),
                ("contactpersonmobile", -1) => query.OrderByDescending(x => x.ContactPersonMobile),
                ("gstin", 1) => query.OrderBy(x => x.GSTIN),
                ("gstin", -1) => query.OrderByDescending(x => x.GSTIN),
                ("pan", 1) => query.OrderBy(x => x.PAN),
                ("pan", -1) => query.OrderByDescending(x => x.PAN),
                ("tallyledgername", 1) => query.OrderBy(x => x.TallyLedgerName),
                ("tallyledgername", -1) => query.OrderByDescending(x => x.TallyLedgerName),
                ("state", 1) => query.OrderBy(x => x.State),
                ("state", -1) => query.OrderByDescending(x => x.State),
                ("city", 1) => query.OrderBy(x => x.City),
                ("city", -1) => query.OrderByDescending(x => x.City),
                ("pincode", 1) => query.OrderBy(x => x.Pincode),
                ("pincode", -1) => query.OrderByDescending(x => x.Pincode),
                ("address", 1) => query.OrderBy(x => x.Address),
                ("address", -1) => query.OrderByDescending(x => x.Address),
                _ => query.OrderByDescending(x => x.Name)
            };
        }
    }
}
