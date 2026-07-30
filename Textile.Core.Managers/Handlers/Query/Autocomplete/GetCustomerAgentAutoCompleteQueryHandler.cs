using MediatR;
using Microsoft.EntityFrameworkCore;
using Textile.Core.Entities.Models.Response.Agents;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Managers.Query.AutoComplete;

namespace Textile.Core.Managers.Handlers.Query.Autocomplete
{
    public class GetCustomerAgentAutoCompleteQueryHandler
        : IRequestHandler<GetCustomerAgentAutoCompleteQuery, IEnumerable<AgentTableResponse>>
    {
        private readonly TextileDbContext _context;

        public GetCustomerAgentAutoCompleteQueryHandler(TextileDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AgentTableResponse>> Handle(
            GetCustomerAgentAutoCompleteQuery request, CancellationToken cancellationToken)
        {
            var search = request.Search?.Trim().ToLower() ?? string.Empty;
            var query = _context.CustomerAgents.AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted);

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>
                    x.Name.ToLower().Contains(search) ||
                    (x.Code ?? string.Empty).ToLower().Contains(search) ||
                    (x.ContactPersonMobile ?? string.Empty).Contains(search));
            }

            return await query.OrderBy(x => x.Name).Take(10)
                .Select(x => new AgentTableResponse
                {
                    Id = x.Id, Name = x.Name, Code = x.Code,
                    ContactPersonName = x.ContactPersonName,
                    ContactPersonMobile = x.ContactPersonMobile,
                    GSTIN = x.GSTIN, PAN = x.PAN, IsActive = x.IsActive
                }).ToListAsync(cancellationToken);
        }
    }
}
