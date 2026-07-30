using Microsoft.EntityFrameworkCore;
using Textile.Core.Entities.Data;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Models.Requests;
using Textile.Core.Entities.Models.Response.Stocks;
using Textile.Core.Entities.Views;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Interfaces.Data;
using Textile.Core.Interfaces.Services;

namespace Textile.Core.Managers.Services
{
    public class StockService : IStockService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly TextileDbContext _context;
        public StockService(IUnitOfWork unitOfWork, TextileDbContext context)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<StockTableResponse> GetStockByIdAsync(Guid id)
        {
            var stock = await _context.Stocks
                .Where(x => x.Id == id)
                .Select(x => new StockTableResponse
                {
                    Id = x.Id,
                    SupplierName = x.Product.Supplier.Name,
                    ProductId = x.Product.Id,
                    Barcode = x.Product.Barcode,
                    ProductName = x.Product.PrintName,
                    StockGroup = x.Product.StockGroup.Name,

                    OpeningQty = x.OpeningQty,
                    InwardQty = x.InwardQty,
                    OutwardQty = x.OutwardQty,
                    ReservedQty = x.ReservedQty,
                    DamagedQty = x.DamagedQty,
                    TotalQty = x.TotalQty,
                    AvailableQty = x.AvailableQty,

                    PurchaseRate = x.PurchaseRate,
                    Discount = x.Discount,
                    WholeSaleMargin = x.WholeSaleMargin,
                    RetailMargin = x.RetailMargin,
                    MrpMargin = x.MrpMargin,

                    WholeSaleRate = x.WholeSaleRate,
                    RetailRate = x.RetailRate,
                    MrpRate = x.MrpRate,

                    CreatedAt = x.CreatedAt
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (stock == null)
                throw new Exception("Stock not found");

            return stock;
        }

        public async Task<IEnumerable<CurrentStockView>> GetStockItemsByBarcode(string barcode)
        {
            var repository = _unitOfWork.Repository<CurrentStockView, Guid>();

            return await repository.GetAllAsync(x=>x.BarCode==barcode);
        }

        public async Task<TableResult<StockLedgerViews>> GetStockLedgerTableData(TableDataRequest dataRequest)
        {
            var query = _context.StockLedgerViews.AsNoTracking();


            // 🔍 SEARCH
            if (!string.IsNullOrWhiteSpace(dataRequest.Search))
            {
                var s = dataRequest.Search.Trim().ToLower();

                query = query.Where(x =>
                  x.ProductName == s);
            }

            // 🔢 TOTAL COUNT (before paging)
            int total = await query.CountAsync();
            //Object reference not set to an instance of an object.

            // 📄 PAGED DATA
            var data = await query
                .OrderByDescending(x => x.Date) // single ordering
                .Skip(dataRequest.PageIndex * dataRequest.PageSize)
                .Take(dataRequest.PageSize).ToListAsync();

            return new TableResult<StockLedgerViews>
            {
                TotalRows = total,
                Result = data
            };
        }

        public async Task<TableResult<StockTableResponse>> GetTableData(TableDataRequest dataRequest)
        {
            var query = _context.Stocks.Include(x => x.Product).ThenInclude(x => x.StockGroup).Include(x => x.Product)
                   .ThenInclude(x => x.Supplier).AsNoTracking();


            // 🔍 SEARCH
            if (!string.IsNullOrWhiteSpace(dataRequest.Search))
            {
                var s = dataRequest.Search.Trim().ToLower();

                query = query.Where(x =>
                  x.Product.PrintName.ToLower().Contains(s) ||
                  x.Product.Name.ToLower().Contains(s) ||
                  x.Product.Supplier.Name.ToLower().Contains(s))
                  ;
            }

            // 🔢 TOTAL COUNT (before paging)
            int total = await query.CountAsync();

            // 📄 PAGED DATA
            var data = await query
                .OrderByDescending(x => x.Product.PrintName) // single ordering
                .Skip(dataRequest.PageIndex * dataRequest.PageSize)
                .Take(dataRequest.PageSize)
                .Select(x => new StockTableResponse
                {
                    // Implement the mapping from Stock to StockTableResponse

                    Id = x.Id,
                    SupplierName = x.Product.Supplier.Name,
                    ProductId = x.Product.Id,
                    Barcode = x.Product.Barcode,
                    ProductName = x.Product.PrintName,
                    StockGroup = x.Product.StockGroup.Name,
                    OpeningQty = x.OpeningQty,
                    InwardQty = x.InwardQty,
                    OutwardQty = x.OutwardQty,
                    ReservedQty = x.ReservedQty,
                    DamagedQty = x.DamagedQty,
                    TotalQty = x.TotalQty,
                    AvailableQty = x.AvailableQty,
                    PurchaseRate = x.PurchaseRate,
                    Discount = x.Discount,
                    WholeSaleMargin = x.WholeSaleMargin,
                    RetailMargin = x.RetailMargin,
                    MrpMargin = x.MrpMargin,
                    WholeSaleRate = x.WholeSaleRate,
                    RetailRate = x.RetailRate,
                    MrpRate = x.MrpRate,
                    CreatedAt = x.CreatedAt,


                })
                .ToListAsync();

            return new TableResult<StockTableResponse>
            {
                TotalRows = total,
                Result = data
            };
        }
    }
}
