using MediatR;
using Microsoft.EntityFrameworkCore;
using Textile.Core.Entities.Models.Response.Agents;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Managers.Query.AutoComplete;

namespace Textile.Core.Managers.Handlers.Query.Autocomplete
{

    public class GetAgentAutoCompleteQueryHandler
   : IRequestHandler<GetAgentAutoCompleteQuery, IEnumerable<AgentTableResponse>>
    {
        private readonly TextileDbContext _context;

        public GetAgentAutoCompleteQueryHandler(TextileDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<AgentTableResponse>> Handle(
            GetAgentAutoCompleteQuery request,
            CancellationToken cancellationToken)
        {
            string search = request.Search?.Trim().ToLower() ?? string.Empty;

            var query = _context.Agents
                .AsNoTracking()
                .Select(s => new AgentTableResponse
                {
                    Id = s.Id,
                    Code = s.Code,
                    Name = s.Name,
                    PAN = s.PAN
                });

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>
                    x.Name.ToLower().Contains(search) ||
                    x.Code.ToLower().Contains(search)
                );
            }

            // Return Top 10 Only
            return await query
                .OrderBy(x => x.Name)
                .Take(10)
                .ToListAsync(cancellationToken);
        }
    }

}
