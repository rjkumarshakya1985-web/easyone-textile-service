using MediatR;
using Microsoft.EntityFrameworkCore;
using Textile.Core.Entities.Enums;
using Textile.Core.Entities.Models.Response;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Managers.Query.AutoComplete;

namespace Textile.Core.Managers.Handlers.Query.Autocomplete
{
    public class GetOrphanTransportsQueryHandler
    : IRequestHandler<GetOrphanTransportsQuery, IEnumerable<TransportResponse>>
    {
        private readonly TextileDbContext _context;

        public GetOrphanTransportsQueryHandler(TextileDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<TransportResponse>> Handle(
            GetOrphanTransportsQuery request,
            CancellationToken cancellationToken)
        {
            var search = request.Search?.Trim().ToLower() ?? string.Empty;
            var supplierId = request.SupplierId;

            // All transports not mapped to this supplier
            var query = _context.Transports.Where(t =>
        t.IsActive &&
        !t.IsDeleted &&
        t.TransportType != (int)TransportTypeEnum.Sales)
                .AsNoTracking()
                .Include(t => t.City)
                    .ThenInclude(c => c.State)
                // Filter out transports already mapped to the supplier
                .Where(t => !_context.SupplierTransports
                    .Any(st => st.TransportId == t.Id && st.SupplierId == supplierId));

            // Apply search if provided
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(t =>
                    t.Name.ToLower().Contains(search) 
                );
            }

            var result = await query
                .Select(t => new TransportResponse
                {
                    Id = t.Id,
                    Name = t.Name,
                    RegistrationType= t.RegistrationType,
                })
                .Take(10) // first 10 records
                .ToListAsync(cancellationToken);

            return result;
        }
    }

}
