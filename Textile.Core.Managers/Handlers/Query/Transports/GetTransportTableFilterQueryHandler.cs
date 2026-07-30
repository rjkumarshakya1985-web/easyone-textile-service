using MediatR;
using Microsoft.EntityFrameworkCore;
using Textile.Core.Entities.Data;
using Textile.Core.Entities.Enums;
using Textile.Core.Entities.Models.Response;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Managers.Query.Transports;

namespace Textile.Core.Managers.Handlers.Query.Transports
{
    public class GetTransportTableFilterQueryHandler : IRequestHandler<GetTransportTableFilterQuery, TableResult<TransportResponse>>
    {
        private readonly TextileDbContext _TextileDbContext;

        public GetTransportTableFilterQueryHandler(TextileDbContext textileDbContext)
        {
            _TextileDbContext = textileDbContext ?? throw new ArgumentNullException(nameof(textileDbContext));
        }

        public async Task<TableResult<TransportResponse>> Handle(
    GetTransportTableFilterQuery request,
    CancellationToken cancellationToken)
        {
            var req = request.DataRequest;

            var query = _TextileDbContext.Transports
                .AsNoTracking()
                .Include(c => c.City)
                .ThenInclude(c => c.State)
                .Select(t => new TransportResponse
                {
                    Id = t.Id,
                    Name = t.Name,
                    City = t.City.Name,
                    State = t.City.State.Name,
                    GstIn = t.GstIn,
                    RegistrationType = t.RegistrationType,
                    Address = t.Address,
                    PinCode = t.Pincode,
                    Mobile = t.Mobile,
                    Email = t.Email,
                    TransportType = (TransportTypeEnum) t.TransportType,
                    Remarks = t.Remarks
                });

            // ----------------------------
            // MAGIC SEARCH IMPLEMENTATION
            // ----------------------------
            if (!string.IsNullOrWhiteSpace(req.Search))
            {
                string s = req.Search.Trim().ToLower();

                query = query.Where(t =>
                    t.Name.ToLower().Contains(s) ||
                    t.City.ToLower().Contains(s) ||
                    t.State.ToLower().Contains(s) ||
                    t.GstIn.ToLower().Contains(s) ||
                    t.Mobile.ToLower().Contains(s) ||
                    t.Email.ToLower().Contains(s) ||
                    t.PinCode.ToLower().Contains(s)
                );
            }

            // Count AFTER search filter
            int total = await query.CountAsync(cancellationToken);

            // ----------------------------
            // ORDER BY NAME
            // ----------------------------
            query = query.OrderBy(t => t.Name);

            // Pagination
            var result = await query
                .Skip(req.PageIndex * req.PageSize)
                .Take(req.PageSize)
                .ToListAsync(cancellationToken);

            return new TableResult<TransportResponse>
            {
                TotalRows = total,
                Result = result
            };
        }

    }
}
