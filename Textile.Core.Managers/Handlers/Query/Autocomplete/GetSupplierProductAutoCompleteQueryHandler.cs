using MediatR;
using Microsoft.EntityFrameworkCore;
using Textile.Core.Entities.Views;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Managers.Query.AutoComplete;

namespace Textile.Core.Managers.Handlers.Query.Autocomplete
{

    public class GetSupplierProductAutoCompleteQueryHandler
    : IRequestHandler<GetSupplierProductAutoCompleteQuery, IEnumerable<SupplierProductView>>
    {
        private readonly TextileDbContext _context;

        public GetSupplierProductAutoCompleteQueryHandler(TextileDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<SupplierProductView>> Handle(
            GetSupplierProductAutoCompleteQuery request,
            CancellationToken cancellationToken)
        {
            string search = request.Search?.Trim();

            var query = _context.SupplierProductViews
                        .AsNoTracking()
                        .Where(x =>
                         x.SupplierId == request.SupplierId &&
                         !x.IsDeleted
                         );

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();

                query = query.Where(x => 
                    x.Name.ToLower().Contains(search)
                );
            }


            return await query
                .OrderBy(x => x.Name)
                .Take(10)
                .ToListAsync(cancellationToken);
        }
    }

}
