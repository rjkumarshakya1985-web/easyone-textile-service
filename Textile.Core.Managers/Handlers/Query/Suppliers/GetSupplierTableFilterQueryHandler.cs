using MediatR;
using Microsoft.EntityFrameworkCore;
using Textile.Core.Entities.Data;
using Textile.Core.Entities.Models.Response.Suppliers;
using Textile.Core.Entities.Views;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Managers.Query.Suppliers;

namespace Textile.Core.Managers.Handlers.Query.Suppliers
{
    public class GetSupplierTableFilterQueryHandler : IRequestHandler<GetSupplierTableFilterQuery, TableResult<SupplierTableResponse>>
    {
        private readonly TextileDbContext _TextileDbContext;

        public GetSupplierTableFilterQueryHandler(TextileDbContext textileDbContext)
        {
            _TextileDbContext = textileDbContext ?? throw new ArgumentNullException(nameof(textileDbContext));
        }

        public async Task<TableResult<SupplierTableResponse>> Handle(
        GetSupplierTableFilterQuery request,
         CancellationToken cancellationToken)
        {
            var req = request.DataRequest;

            var query = _TextileDbContext.Suppliers
                .AsNoTracking()
                .Where(t => !t.IsDeleted)
                .Select(t => new SupplierTableResponse
                {
                    Id = t.Id,
                    Code= t.Code,
                    Name = t.Name,
                    GstIn = t.GstIn,
                    PAN = t.PAN,
                    IsActive = t.IsActive,
                    City = t.City.Name,
                    State = t.City.State.Name,
                    Address = t.Address,
                    UserName = t.User.UserName,
                    Password = t.User.Password,
                    AgentId = t.AgentId,
                    AgentName = t.Agent != null ? t.Agent.Name : null
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
                    t.GstIn.ToLower().Contains(s) ||
                    t.PAN.ToLower().Contains(s) 
                );
            }



            query = ApplyFilters(query, req.Filters);
            query = ApplySorting(query, req.SortField, req.SortOrder);

            int total = await query.CountAsync(cancellationToken);

            // Pagination
            var result = await query
                .Skip(req.PageIndex * req.PageSize)
                .Take(req.PageSize)
                .ToListAsync(cancellationToken);

            return new TableResult<SupplierTableResponse>
            {
                TotalRows = total,
                Result = result
            };
        }

        private IQueryable<SupplierTableResponse> ApplyFilters(
       IQueryable<SupplierTableResponse> query,
      Dictionary<string, string>? filters)
        {
            if (filters == null || !filters.Any())
                return query;

            foreach (var filter in filters)
            {
                if (string.IsNullOrWhiteSpace(filter.Value))
                    continue;

                var value = filter.Value.Trim().ToLower();

                switch (filter.Key)
                {
                    case "code":
                        query = query.Where(x => x.Code.ToLower().Contains(value));
                        break;

                    case "name":
                        query = query.Where(x => x.Name.ToLower().Contains(value));
                        break;

                    case "agent":
                        query = query.Where(x => x.AgentName.ToLower().Contains(value));
                        break;
                  
                    case "gstIn":
                        query = query.Where(x => x.GstIn.ToLower().Contains(value));
                        break;

                }
            }

            return query;
        }
        private IQueryable<SupplierTableResponse> ApplySorting(
  IQueryable<SupplierTableResponse> query,
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

                ("agentname", 1) => query.OrderBy(x => x.AgentName),
                ("agentname", -1) => query.OrderByDescending(x => x.AgentName),

                ("gstin", 1) => query.OrderBy(x => x.GstIn),
                ("gstin", -1) => query.OrderByDescending(x => x.GstIn),

                ("username", 1) => query.OrderBy(x => x.UserName),
                ("username", -1) => query.OrderByDescending(x => x.UserName),

                ("address", 1) => query.OrderBy(x => x.Address),
                ("address", -1) => query.OrderByDescending(x => x.Address),



                _ => query.OrderByDescending(x => x.Name)
            };
        }

    }
}
