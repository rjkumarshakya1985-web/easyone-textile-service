using MediatR;
using Microsoft.EntityFrameworkCore;
using Textile.Core.Entities.Models.Response;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Managers.Query.AutoComplete;

namespace Textile.Core.Managers.Handlers.Query.Autocomplete
{
    public class GetHsnAutoCompleteQueryHandler
    : IRequestHandler<GetHsnAutoCompleteQuery, IEnumerable<HsnCodeResponse>>
    {
        private readonly TextileDbContext _context;

        public GetHsnAutoCompleteQueryHandler(TextileDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<HsnCodeResponse>> Handle(
            GetHsnAutoCompleteQuery request,
            CancellationToken cancellationToken)
        {
            string search = request.Search?.Trim().ToLower() ?? string.Empty;

            var query = _context.HsnCodes.Where(x=>!x.IsDeleted)
                .AsNoTracking()
                .Select(s => new HsnCodeResponse
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                });

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>
                    x.Name.ToLower().Contains(search)
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
