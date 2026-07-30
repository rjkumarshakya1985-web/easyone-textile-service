using MediatR;
using Microsoft.EntityFrameworkCore;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Enums;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Managers.Commands.Sales.DeliveryNotes;

namespace Textile.Core.Managers.Handlers.Commands.Sales.DeliveryChallan
{
    public class CancelDeliveryChallanCommandHandler
    : IRequestHandler<CancelDeliveryChallanCommand, bool>
    {
        private readonly TextileDbContext _context;

        public CancelDeliveryChallanCommandHandler(TextileDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(CancelDeliveryChallanCommand request, CancellationToken ct)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(ct);

            try
            {
                var challan = await _context.DeliveryChallans
                    .Include(x => x.DeliveryChallanItems)
                    .FirstOrDefaultAsync(x =>
                        x.Id == request.DeliveryChallanId &&
                        !x.IsDeleted, ct);

                if (challan == null)
                    throw new Exception("Delivery Challan not found");

                // 🔴 STATUS VALIDATION
                if (challan.Status != (int)DeliveryChallanStatusEnum.Created &&
                    challan.Status != (int)DeliveryChallanStatusEnum.Dispatched)
                {
                    throw new Exception("Cancel allowed only for Created or Dispatched challan");
                }

                // 🔴 Already returned check (extra safety)
                if (challan.DeliveryChallanItems.Any(x => x.ReturnQty > 0))
                {
                    throw new Exception("Cannot cancel. Return already initiated.");
                }

                // 🔥 STOCK REVERSE (OUT → IN reverse)
                var stockIds = challan.DeliveryChallanItems
                    .Select(x => x.StockId)
                    .Distinct()
                    .ToList();

                var stocks = await _context.Stocks
                    .Where(x => stockIds.Contains(x.Id))
                    .ToDictionaryAsync(x => x.Id, ct);

                var stockTransactions = new List<StockTransaction>();

                foreach (var item in challan.DeliveryChallanItems)
                {
                    if (!stocks.TryGetValue(item.StockId, out var stock))
                        throw new Exception($"Stock not found: {item.StockId}");

                    // 🔁 Reverse stock (OUT cancel → IN)
                    stock.InwardQty += item.Qty;

                    stockTransactions.Add(new StockTransaction
                    {
                        ProductId = stock.ProductId,
                        VoucherId = challan.Id,
                        VoucherType = (int)VoucherTypeEnum.DeliveryChallan,
                        TransactionType = "IN", // reverse
                        Quantity = item.Qty,
                        TransactionDate = DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow
                    });
                }

                // 🔥 Update status
                challan.Status = (int)DeliveryChallanStatusEnum.Cancelled;

                challan.ModifiedBy = request.CurrentUserId;
                challan.ModifiedByUserName = request.CurrentUserName;
                challan.ModifiedOn = DateTime.UtcNow;


             //   var packingSlips = await _context.DeliveryChallanPackingSlipMaps
             // .Where(x =>
             //x.DeliveryChallanId == request.DeliveryChallanId &&
             //x.PackingSlip.Status != (int)PackingSlipStatusEnum.Cancelled)
             // .Select(x => x.PackingSlip)
             // .Distinct()
             // .ToListAsync(ct);

             //   packingSlips.ForEach(ps =>
             //   {
             //       ps.Status = (int)PackingSlipStatusEnum.Cancelled;
             //       ps.ModifiedBy = request.CurrentUserId;
             //       ps.ModifiedByUserName = request.CurrentUserName;
             //       ps.ModifiedOn = DateTime.UtcNow;
             //   });

                _context.StockTransactions.AddRange(stockTransactions);

                await _context.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);

                return true;
            }
            catch
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        }
    }
}
