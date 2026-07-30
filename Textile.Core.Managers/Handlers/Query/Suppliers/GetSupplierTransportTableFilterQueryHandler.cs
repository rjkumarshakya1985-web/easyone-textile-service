using MediatR;
using Microsoft.EntityFrameworkCore;
using Textile.Core.Entities.Data;
using Textile.Core.Entities.Models.Response;
using Textile.Core.Entities.Models.Response.Suppliers;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Managers.Query.Suppliers;

namespace Textile.Core.Managers.Handlers.Query.Suppliers
{

    public class GetSupplierTransportTableFilterQueryHandler
     : IRequestHandler<GetSupplierTransportTableFilterQuery, TableResult<SupplierTransportResponse>>
    {
        private readonly TextileDbContext _context;

        public GetSupplierTransportTableFilterQueryHandler(TextileDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<TableResult<SupplierTransportResponse>> Handle(
            GetSupplierTransportTableFilterQuery request,
            CancellationToken cancellationToken)
        {
            var req = request.DataRequest;

            // Base query
            var query = _context.SupplierTransports
                .AsNoTracking()
                .Include(st => st.Supplier)
                .Include(st => st.Transport)
                    .ThenInclude(t => t.City)
                        .ThenInclude(c => c.State)
                .Select(st => new
                {
                    st.SupplierId,
                    SupplierName = st.Supplier.Name,
                    SupplerCode = st.Supplier.Code,
                    st.TransportId,
                    TransportName = st.Transport.Name,
                    City = st.Transport.City.Name,
                    State = st.Transport.City.State.Name,
                    st.IsActive
                });

            // SEARCH
            if (!string.IsNullOrWhiteSpace(req.Search))
            {
                string s = req.Search.Trim().ToLower();

                query = query.Where(x =>
                    x.SupplierName.ToLower().Contains(s) ||
                    x.SupplerCode.ToLower().Contains(s) 
                );
            }

            // Total after search
            int total = await query.CountAsync(cancellationToken);

            // ORDER BY SUPPLIER
            query = query.OrderBy(x => x.SupplierName);

            // Pagination
            var data = await query
                .Skip(req.PageIndex * req.PageSize)
                .Take(req.PageSize)
                .ToListAsync(cancellationToken);

            // GROUP BY SUPPLIER
            var grouped = data
                .GroupBy(x => new { x.SupplierId, x.SupplierName,x.SupplerCode })
                .Select(g => new SupplierTransportResponse
                {
                    SupplierId = g.Key.SupplierId,
                    Name = g.Key.SupplierName,
                    Code = g.Key.SupplerCode,
                    TransportResponses = g.Select(t => new TransportResponse
                    {
                        Id = t.TransportId,
                        Name = t.TransportName,
                        City = t.City,
                        State = t.State,
                        
                    })
                })
                .ToList();

            return new TableResult<SupplierTransportResponse>
            {
                TotalRows = total,
                Result = grouped
            };
        }
    }

}

