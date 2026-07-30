
using Microsoft.EntityFrameworkCore;
using Textile.Core.Entities.Data;
using Textile.Core.Entities.DbEnitites.Sales;
using Textile.Core.Entities.Models.Requests;
using Textile.Core.Entities.Models.Response.DeliveryChallan;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Interfaces.Data;
using Textile.Core.Interfaces.Services.Sales;

namespace Textile.Core.Managers.Services.Sales
{
    public class DeliveryChallanService : IDeliveryChallanService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly TextileDbContext _context;
        public DeliveryChallanService(IUnitOfWork unitOfWork, TextileDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<TableResult<DeliveryChallanListResponse>> GetTableData(TableDataRequest tableDataRequest, int financialYear)
        {
            var query = _context.DeliveryChallans.Include(x => x.Customer).AsNoTracking().Where(x => !x.IsDeleted && x.FinanceYearId == financialYear);


            if (!string.IsNullOrWhiteSpace(tableDataRequest.Search))
            {
                var s = tableDataRequest.Search.Trim().ToLower();

                query = query.Where(x =>
                  x.DeliveryChallanNumber.ToLower().Contains(s) ||
                  x.Customer.Name != null && x.Customer.Name.ToLower().Contains(s) )
                  ;
            }

            query = ApplyFilters(query, tableDataRequest.Filters);
            int total = await query.CountAsync();

            var data = await query
                .OrderBy(x => x.DeliveryChallanNumber)
                .Skip(tableDataRequest.PageIndex * tableDataRequest.PageSize)
                .Take(tableDataRequest.PageSize)
                .ToListAsync();

            return new TableResult<DeliveryChallanListResponse>
            {
                TotalRows = total,
                Result = data.Select(x => new DeliveryChallanListResponse
                {
                    Id = x.Id,
                    ChallanNumber = x.DeliveryChallanNumber,
                    CustomerName = x.Customer.Name,
                    Date = x.Date,
                    Quantity = x.TotalQuantity,
                    ReturnQuantity = x.TotalReturnQty,
                    BalanceQuantity = x.TotalEffectiveQty,
                    TotalAmount = x.TotalAmount,
                    Status = x.Status
                }).ToList()
            };

        }


        private IQueryable<DeliveryChallan> ApplyFilters(
        IQueryable<DeliveryChallan> query,
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
                    case "DeliveryChallanNumber":
                        query = query.Where(x => x.DeliveryChallanNumber.Contains(value));
                        break;
                    case "CustomerName":
                        query = query.Where(x => x.Customer.Name.ToLower().Contains(value));
                        break;

                    case "Status":
                        if (int.TryParse(value, out var status))
                            query = query.Where(x => x.Status == status);
                        break;

                }
            }

            return query;
        }
    }
}
