using MediatR;
using Textile.Core.Entities.Data;
using Microsoft.EntityFrameworkCore;
using Textile.Core.Entities.Models.Response.Agents;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Managers.Query.Agents;

namespace Textile.Core.Managers.Handlers.Query.Agents
{   
    public class GetAgentTableFilterQueryHandler : IRequestHandler<GetAgentTableFilterQuery, TableResult<AgentTableResponse>>
    {
        private readonly TextileDbContext _TextileDbContext;

        public GetAgentTableFilterQueryHandler(TextileDbContext textileDbContext)
        {
            _TextileDbContext = textileDbContext ?? throw new ArgumentNullException(nameof(textileDbContext));
        }

        public async Task<TableResult<AgentTableResponse>> Handle(
        GetAgentTableFilterQuery request,
         CancellationToken cancellationToken)
        {
            var req = request.DataRequest;

            var query = _TextileDbContext.Agents
                .AsNoTracking()
                .Where(t => !t.IsDeleted)
                .Include(c => c.City)
                .ThenInclude(c => c.State)

                .Select(t => new AgentTableResponse
                {
                    Id = t.Id,
                    Code = t.Code,
                    Name =t.Name,
                    ContactPersonMobile = t.ContactPersonMobile,
                    ContactPersonName = t.ContactPersonName,
                    GSTIN = t.GSTIN,
                    PAN = t.PAN,
                    IsActive = t.IsActive,
                    City = t.City.Name,                  
                    State = t.City.State.Name,
                    Address = t.Address,
                    TallyLedgerName=t.TallyLedgerName,
                    Area= t.Area,
                    Pincode = t.Pincode,


                });

            // ----------------------------
            // MAGIC SEARCH IMPLEMENTATION
            // ----------------------------
            if (!string.IsNullOrWhiteSpace(req.Search))
            {
                string s = req.Search.Trim().ToLower();

                query = query.Where(t =>
                    t.Name.ToLower().Contains(s) ||
                    t.Code.ToLower().Contains(s) ||
                    t.ContactPersonName.ToLower().Contains(s) ||
                    t.GSTIN.ToLower().Contains(s) ||
                    t.PAN.ToLower().Contains(s)||
                    t.TallyLedgerName.ToLower().Contains(s)||
                    t.Area.ToLower().Contains(s)

                );
            }

            // Count AFTER search filter
            int total = await query.CountAsync(cancellationToken);

            // ----------------------------
            // ORDER BY Sorting
            // ----------------------------         
            query = ApplySorting(query, req.SortField, req.SortOrder);

            // Pagination
            var result = await query
                .Skip(req.PageIndex * req.PageSize)
                .Take(req.PageSize)
                .ToListAsync(cancellationToken);

            return new TableResult<AgentTableResponse>
            {
                TotalRows = total,
                Result = result
            };
        }
        private IQueryable<AgentTableResponse> ApplySorting(
IQueryable<AgentTableResponse> query,
string? sortField,
int sortOrder)
        {
            if (string.IsNullOrWhiteSpace(sortField))
            {
                // Default sorting
                return query.OrderByDescending(x => x.Name);
            }

            return (sortField.ToLower(), sortOrder) switch
            {
                ("name", 1) => query.OrderBy(x => x.Name),
                ("name", -1) => query.OrderByDescending(x => x.Name),

                ("code", 1) => query.OrderBy(x => x.Code),
                ("code", -1) => query.OrderByDescending(x => x.Code),

                ("contactpersonname", 1) => query.OrderBy(x => x.ContactPersonName),
                ("contactpersonname", -1) => query.OrderByDescending(x => x.ContactPersonName),

                ("contactpersonmobile", 1) => query.OrderBy(x => x.ContactPersonMobile),
                ("contactpersonmobile", -1) => query.OrderByDescending(x => x.ContactPersonMobile),

                ("gstin", 1) => query.OrderBy(x => x.GSTIN),
                ("gstin", -1) => query.OrderByDescending(x => x.GSTIN),

                ("address", 1) => query.OrderBy(x => x.Address),
                ("address", -1) => query.OrderByDescending(x => x.Address),

                ("area", 1) => query.OrderBy(x => x.Area),
                ("area", -1) => query.OrderByDescending(x => x.Area),

                ("pan", 1) => query.OrderBy(x => x.PAN),
                ("pan", -1) => query.OrderByDescending(x => x.PAN),

                ("pincode", 1) => query.OrderBy(x => x.Pincode),
                ("pincode", -1) => query.OrderByDescending(x => x.Pincode),

                 ("tallyledgername", 1) => query.OrderBy(x => x.TallyLedgerName),
                ("tallyledgername", -1) => query.OrderByDescending(x => x.TallyLedgerName),

                 ("city", 1) => query.OrderBy(x => x.City),
                ("city", -1) => query.OrderByDescending(x => x.City),

                 ("isactive", 1) => query.OrderBy(x => x.IsActive),
                ("isactive", -1) => query.OrderByDescending(x => x.IsActive),


                _ => query.OrderByDescending(x => x.Name)
            };
        }

    }
}
