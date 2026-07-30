using Microsoft.EntityFrameworkCore;
using Textile.Core.Entities.Data;
using Textile.Core.Entities.DbEnitites.Sales;
using Textile.Core.Entities.Enums;
using Textile.Core.Entities.Models.Requests;
using Textile.Core.Entities.Models.Response.Billing.Invoices;
using Textile.Core.Entities.Models.Response.Invoices;
using Textile.Core.Entities.Views;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Interfaces.Data;
using Textile.Core.Interfaces.Services.Sales;

namespace Textile.Core.Managers.Services.Sales
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly TextileDbContext _context;
        public InvoiceService(IUnitOfWork unitOfWork, TextileDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }
        public async Task<InvoiceResponse?> GetInvoice(string number, int finYearId)
        {
            var invoice = await _context.Invoices
                .Where(x => x.InvoiceNumber == number
                         && x.FinanceYearId == finYearId
                         && !x.IsDeleted)
                .Select(x => new InvoiceResponse
                {
                    Id = x.Id,
                    Date = x.Date,
                    CustomerId = x.CustomerId,
                    InvoiceNumber = x.InvoiceNumber,
                    CustomerName = x.Customer.Name,
                    GstIn = x.Customer.GstIn,
                    Status = (InvoiceStatusEnum)x.Status,

                    Items = x.InvoiceItems.Select(i => new InvoiceItemResponse
                    {
                        Id = i.Id,
                        InvoiceId = i.InvoiceId,
                        StockId = i.StockId,
                        ProductCategory = i.Stock.Product.StockGroup.Name,
                        ProductName = i.Stock.Product.Name,
                        Qty = i.Qty,
                        SaleRate = i.SaleRate,
                        Amount = Math.Round(i.TotalAmount,0)
                    }).ToList(),
                   
                    TotalAmount = Math.Round(x.GrandTotal,0),
                     TotalQuantity = x.TotalQuantity
                })
                .FirstOrDefaultAsync();

          
            return invoice;
        }

        public async Task<InvoiceResponse?> GetInvoice(int id)
        {
            var invoice = await _context.Invoices
                .Where(x => x.Id == id && !x.IsDeleted)
                .Select(x => new InvoiceResponse
                {
                    Id = x.Id,
                    Date = x.Date,
                    CustomerId = x.CustomerId,
                    InvoiceNumber = x.InvoiceNumber,
                    CustomerName = x.Customer.Name,
                    GstIn = x.Customer.GstIn,
                    Status = (InvoiceStatusEnum)x.Status,

                    Items = x.InvoiceItems.Select(i => new InvoiceItemResponse
                    {
                        Id = i.Id,
                        InvoiceId = i.InvoiceId,
                        StockId = i.StockId,
                        ProductCategory = i.Stock.Product.StockGroup.Name,
                        ProductName = i.Stock.Product.Name,
                        Qty = i.Qty,
                        SaleRate = i.SaleRate,
                        Amount = Math.Round(i.TotalAmount, 0)
                    }).ToList(),

                    TotalAmount = Math.Round(x.GrandTotal, 0),
                    TotalQuantity = x.TotalQuantity

                })
                .FirstOrDefaultAsync();

            return invoice;
        }

        public async Task<List<StatusCountView>> GetInvoiceStatusCountsAsync(int financialYearId)
        {
            var query = _context.Invoices
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.FinanceYearId == financialYearId);

            return await query  
                .GroupBy(x => x.Status)
                .Select(g => new StatusCountView { Status = g.Key, Count = g.Count() })
                .ToListAsync();
        }

        public async Task<TableResult<InvoiceListResponse>> GetTableData(
      TableDataRequest tableDataRequest, int financialYear)
        {
            var query = _context.Invoices
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.FinanceYearId == financialYear);

          
            if (!string.IsNullOrWhiteSpace(tableDataRequest.Search))
            {
                var s = tableDataRequest.Search.Trim().ToLower(); query = query.Where(x => x.InvoiceNumber.ToLower().Contains(s) || x.Customer.Name != null && x.Customer.Name.ToLower().Contains(s));
            }

            query = ApplyFilters(query, tableDataRequest.Filters);
            var total = await query.CountAsync();

            var data = await query
                .OrderByDescending(x => x.Id) 
                .Skip(tableDataRequest.PageIndex * tableDataRequest.PageSize)
                .Take(tableDataRequest.PageSize)
                .Select(x => new InvoiceListResponse
                {
                    Id = x.Id,
                    InvoiceNumber = x.InvoiceNumber,
                    CustomerName = x.Customer != null ? x.Customer.Name : "",
                    Date = x.Date,
                    Quantity = x.TotalQuantity,
                    TotalAmount = x.TotalAmount,
                    Status = x.Status
                })
                .ToListAsync();

            return new TableResult<InvoiceListResponse>
            {
                TotalRows = total,
                Result = data
            };
        }

        private IQueryable<Invoice> ApplyFilters(
          IQueryable<Invoice> query,
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
                    case "InvoiceNumber":
                        query = query.Where(x => x.InvoiceNumber.Contains(value));
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
