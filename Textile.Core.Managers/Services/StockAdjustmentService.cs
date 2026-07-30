using Microsoft.EntityFrameworkCore;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Enums;
using Textile.Core.Entities.Models.Requests.Stock;
using Textile.Core.Entities.Models.Response.Stocks;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Interfaces.Data;
using Textile.Core.Interfaces.Services;

namespace Textile.Core.Managers.Services
{
    public class StockAdjustmentService : IStockAdjustmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly TextileDbContext _context;
        private readonly IStockService _stockService;

        public StockAdjustmentService(IUnitOfWork unitOfWork, TextileDbContext context, IStockService stockService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _stockService = stockService ?? throw new ArgumentNullException(nameof(stockService));
        }

        public async Task<bool> AdjustStockAsync(StockAdjustmentRequest request, Guid userId, string username)
        {
            try
            {
                var stock = await GetStockAsync(request.StockId);
                if (stock == null)
                    throw new Exception("Stock not found");

                var adjustment = CreateStockAdjustmentEntity(request, userId, username);

                ProcessStockAdjustment(request, stock, adjustment);

                await SaveAdjustmentAsync(adjustment);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adjusting stock.", ex);
            }
        }

        //  Get Stock
        private async Task<Stock?> GetStockAsync(Guid stockId)
        {
            return await _context.Stocks.FirstOrDefaultAsync(s => s.Id == stockId);
        }

       
        private StockAdjustment CreateStockAdjustmentEntity(StockAdjustmentRequest request, Guid userId, string username)
        {
            return new StockAdjustment
            {
                StockId = request.StockId,
                SystemQty = request.SystemQty,
                AdjustmentQty = request.AdjustmentQty,
                AdjustmentType = request.AdjustmentType,
                Reason = request.Reason,
                CreatedBy = userId,
                CreatedByUserName = username,
                CreatedOn = DateTime.Now,
                IsDeleted = false
            };
        }

       
        private void ProcessStockAdjustment(StockAdjustmentRequest request, Stock stock, StockAdjustment adjustment)
        {
            var now = DateTime.UtcNow;

            if (request.AdjustmentType == 1)
            {
                HandleIncrease(request, stock, adjustment, now);
            }
            else if (request.AdjustmentType == 2)
            {
                HandleDecrease(request, stock, adjustment, now);
            }
            else
            {
                throw new Exception("Invalid Adjustment Type");
            }
        }

        private void HandleIncrease(StockAdjustmentRequest request, Stock stock, StockAdjustment adjustment, DateTime now)
        {
            adjustment.NewQty = request.SystemQty + request.AdjustmentQty;
            stock.InwardQty += request.AdjustmentQty;

            var transaction = CreateStockTransaction(stock.ProductId, "IN", request.AdjustmentQty, now);
            _context.StockTransactions.Add(transaction);
        }

       
        private void HandleDecrease(StockAdjustmentRequest request, Stock stock, StockAdjustment adjustment, DateTime now)
        {
            adjustment.NewQty = request.SystemQty - request.AdjustmentQty;
            stock.OutwardQty += request.AdjustmentQty;

            var transaction = CreateStockTransaction(stock.ProductId, "OUT", request.AdjustmentQty, now);
            _context.StockTransactions.Add(transaction);
        }

        private StockTransaction CreateStockTransaction(Guid productId, string type, decimal qty, DateTime now)
        {
            return new StockTransaction
            {
                ProductId = productId,
                VoucherId = 0,
                VoucherType = (int)VoucherTypeEnum.StockAdjustment,
                TransactionType = type,
                Quantity = qty,
                TransactionDate = now,
                CreatedAt = now
            };
        }

       
        private async Task SaveAdjustmentAsync(StockAdjustment adjustment)
        {
            var repository = _unitOfWork.Repository<StockAdjustment, Guid>();
            await repository.AddAsync(adjustment);
            await _context.SaveChangesAsync();
        }

        public async Task<StockAdjustmentResponse> GetStockAdjustments(Guid stockId)
        {
         
            var stock = await _stockService.GetStockByIdAsync(stockId);

            var adjustments = await _context.StockAdjustments
                .Where(sa => sa.StockId == stockId && !sa.IsDeleted)
                .Select(sa => new StockAdjustmentDetailsResponse
                {
                    Id = sa.Id,
                    SystemQty = sa.SystemQty,
                    AdjustmentQty = sa.AdjustmentQty,
                    AdjustmentType = sa.AdjustmentType,
                    Reason = sa.Reason,
                    NewQty = sa.NewQty,
                    CreatedByUserName = sa.CreatedByUserName,
                    CreatedOn = sa.CreatedOn
                })
                .ToListAsync();

            return new StockAdjustmentResponse
            {
                Stock = stock,
                Adjustments = adjustments
            };
        }


    }
}
