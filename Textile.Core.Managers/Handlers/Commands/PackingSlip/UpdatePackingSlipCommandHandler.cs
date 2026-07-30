using MediatR;
using Microsoft.EntityFrameworkCore;

using Textile.Core.Entities.DbEnitites.Sales;
using Textile.Core.Entities.Models.Requests.PackingSlips;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Managers.Commands.PackingSlip;

namespace Textile.Core.Managers.Handlers.Commands.PackingSlip
{

    public class UpdatePackingSlipCommandHandler
     : IRequestHandler<UpdatePackingSlipCommand, int>
    {
        private readonly TextileDbContext _context;

        public UpdatePackingSlipCommandHandler(TextileDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(UpdatePackingSlipCommand command, CancellationToken cancellationToken)
        {
            command.PackingSlipRequest.Id = command.PackingSlipId;
            await UpdateAsync(command.PackingSlipRequest, command.CurrentUserId, command.CurrentUserName);

            return command.PackingSlipRequest.Id.Value;
        }

        private async Task UpdateAsync(PackingSlipRequest request, Guid currentUserId, string currentUserName)
        {
            if (request.Items == null || !request.Items.Any())
                throw new Exception("Packing slip must contain at least one item");

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var packingSlip = await _context.PackingSlips
                    .Include(x => x.Items)
                    .FirstOrDefaultAsync(x => x.Id == request.Id);

                if (packingSlip == null)
                    throw new Exception("Packing slip not found");

                var allStockIds = request.Items.Select(x => x.StockId)
                    .Union(packingSlip.Items.Select(x => x.StockId))
                    .ToList();

                var stocks = await _context.Stocks
                    .Where(x => allStockIds.Contains(x.Id))
                    .ToDictionaryAsync(x => x.Id);

                // ✅ HEADER
                packingSlip.SalesPersonId = request.SalesPersonId;
                packingSlip.VisitorId = request.VisitorId;
                packingSlip.ModifiedBy = currentUserId;
                packingSlip.ModifiedByUserName = currentUserName;
                packingSlip.DiscountPercent = request.DiscountPercent;

                packingSlip.ModifiedOn = DateTime.Now;

                foreach (var itemRequest in request.Items)
                {
                    var stock = stocks[itemRequest.StockId];

                    var existingItem = packingSlip.Items
                        .FirstOrDefault(x => x.StockId == itemRequest.StockId);

                    var taxable = itemRequest.SaleRate * itemRequest.Qty;
                    var discountPercent = itemRequest.DiscountPercent ?? 0;
                    var discountAmount = taxable * discountPercent / 100;
                    var netAmount = taxable - discountAmount;
                    var gstAmount = netAmount * itemRequest.GstPercent / 100;
                    var total = Math.Round(netAmount + gstAmount, 2, MidpointRounding.AwayFromZero);

                    if (existingItem != null)
                    {
                        var diff = itemRequest.Qty - existingItem.Qty;

                        if (diff > 0 && stock.AvailableQty < diff)
                            throw new Exception($"Insufficient stock for {stock.Id}");

                        stock.ReservedQty += diff;

                        existingItem.Qty = itemRequest.Qty;
                        existingItem.SaleRate = itemRequest.SaleRate;
                        existingItem.GstPercent = itemRequest.GstPercent;
                        existingItem.DiscountPercent = discountPercent;
                        existingItem.TaxableAmount = taxable;
                        existingItem.DiscountAmount = discountAmount;
                        existingItem.NetAmount = netAmount;
                        existingItem.GstAmount = gstAmount;
                        existingItem.TotalAmount = total;
                    }
                    else
                    {
                        if (stock.AvailableQty < itemRequest.Qty)
                            throw new Exception($"Insufficient stock for {stock.Id}");

                        stock.ReservedQty += itemRequest.Qty;

                        packingSlip.Items.Add(new PackingSlipItem
                        {
                            StockId = itemRequest.StockId,
                            Qty = itemRequest.Qty,
                            SaleRate = itemRequest.SaleRate,
                            GstPercent = itemRequest.GstPercent,
                            DiscountPercent = discountPercent,
                            TaxableAmount = taxable,
                            DiscountAmount = discountAmount,
                            NetAmount = netAmount,
                            GstAmount = gstAmount,
                            TotalAmount = total
                        });
                    }
                }

                var itemsToRemove = packingSlip.Items
                    .Where(x => !request.Items.Any(r => r.StockId == x.StockId))
                    .ToList();

                foreach (var removeItem in itemsToRemove)
                {
                    var stock = stocks[removeItem.StockId];
                    stock.ReservedQty -= removeItem.Qty;

                    _context.PackingSlipItems.Remove(removeItem);
                }

                packingSlip.TotalQuantity = packingSlip.Items.Sum(x => x.Qty);
                packingSlip.TotalDiscount = Math.Round(packingSlip.Items.Sum(x => x.DiscountAmount), 2);
                packingSlip.TotalGst = Math.Round(packingSlip.Items.Sum(x => x.GstAmount), 2);
                packingSlip.TotalAmount = Math.Round(packingSlip.Items.Sum(x => x.TotalAmount), 2);

               
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }

}
