using MediatR;
using Microsoft.EntityFrameworkCore;
using Textile.Core.Entities.DbEnitites.Sales;
using Textile.Core.Entities.Models.Requests.Billing.DeliveryChallans;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Managers.Commands.Sales.DeliveryNotes;

namespace Textile.Core.Managers.Handlers.Commands.Sales.DeliveryChallan
{

    public class UpdateDeliveryChallanCommandHandler
     : IRequestHandler<UpdateDeliveryChallanCommand, int>
    {
        private readonly TextileDbContext _context;
        private readonly IMediator _mediator;

        public UpdateDeliveryChallanCommandHandler(
            TextileDbContext context,
            IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<int> Handle(UpdateDeliveryChallanCommand cmd, CancellationToken ct)
        {
            await UpdateAsync(cmd.Request, cmd.CurrentUserId, cmd.CurrentUserName);

            return cmd.Request.Id;
        }

        private async Task UpdateAsync(
      UpdateDeliveryChallanRequest request,
      Guid currentUserId,
      string currentUserName)
        {
            if (request.Items == null || !request.Items.Any())
                throw new Exception("Delivery challan must contain at least one item");

            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var deliveryChallan = await _context.DeliveryChallans
                    .Include(x => x.DeliveryChallanItems)
                    .FirstOrDefaultAsync(x => x.Id == request.Id);

                if (deliveryChallan == null)
                    throw new Exception("Delivery challan not found");

                var allStockIds = request.Items
                    .Select(x => x.StockId)
                    .Union(deliveryChallan.DeliveryChallanItems.Select(x => x.StockId))
                    .Distinct()
                    .ToList();

                var stocks = await _context.Stocks
                    .Where(x => allStockIds.Contains(x.Id))
                    .ToDictionaryAsync(x => x.Id);

                // HEADER UPDATE
                deliveryChallan.DiscountPercent = request.DiscountPercent;
                deliveryChallan.ModifiedBy = currentUserId;
                deliveryChallan.ModifiedByUserName = currentUserName;
                deliveryChallan.ModifiedOn = DateTime.Now;

                foreach (var itemRequest in request.Items)
                {
                    var stock = stocks[itemRequest.StockId];

                    DeliveryChallanItem? existingItem = null;

                    // UPDATE
                    if (itemRequest.Id.HasValue)
                    {
                        existingItem = deliveryChallan.DeliveryChallanItems
                            .FirstOrDefault(x => x.Id == itemRequest.Id.Value);

                        if (existingItem == null)
                        {
                            throw new Exception(
                                $"Delivery challan item not found : {itemRequest.Id}");
                        }
                    }

                    if (existingItem != null)
                    {
                        // STOCK CHANGED
                        if (existingItem.StockId != itemRequest.StockId)
                        {
                            var oldStock = stocks[existingItem.StockId];

                            // release old reserved qty
                            oldStock.ReservedQty -= existingItem.Qty;

                            // reserve new stock qty
                            if (stock.AvailableQty < itemRequest.Qty)
                            {
                                throw new Exception(
                                    $"Insufficient stock for {stock.Id}");
                            }

                            stock.ReservedQty += itemRequest.Qty;

                            existingItem.StockId = itemRequest.StockId;
                        }
                        else
                        {
                            // same stock qty diff
                            var diff = itemRequest.Qty - existingItem.Qty;

                            if (diff > 0 && stock.AvailableQty < diff)
                            {
                                throw new Exception(
                                    $"Insufficient stock for {stock.Id}");
                            }

                            stock.OutwardQty += diff;
                            stock.InwardQty -= diff;
                        }

                        existingItem.Qty = itemRequest.Qty;

                        existingItem.SaleRate = itemRequest.SaleRate;

                        existingItem.GstPercent = itemRequest.GstPercent;

                        existingItem.DiscountPercent =
                            request.DiscountPercent;
                    }
                    else
                    {
                        // INSERT
                        if (stock.AvailableQty < itemRequest.Qty)
                        {
                            throw new Exception(
                                $"Insufficient stock for {stock.Id}");
                        }

                        stock.OutwardQty += itemRequest.Qty;
                        stock.InwardQty -= itemRequest.Qty;

                        deliveryChallan.DeliveryChallanItems
                            .Add(new DeliveryChallanItem
                            {
                                DeliveryChallanId = deliveryChallan.Id,

                                StockId = itemRequest.StockId,

                                Qty = itemRequest.Qty,

                                ReturnQty = 0,

                                SaleRate = itemRequest.SaleRate,

                                GstPercent = itemRequest.GstPercent,

                                DiscountPercent =
                                    request.DiscountPercent
                            });
                    }
                }

                // REMOVE ITEMS
                var requestItemIds = request.Items
                    .Where(x => x.Id.HasValue)
                    .Select(x => x.Id.Value)
                    .ToHashSet();

                var itemsToRemove = deliveryChallan.DeliveryChallanItems
                    .Where(x => !requestItemIds.Contains(x.Id) && x.Id!=0)
                    .ToList();

                foreach (var removeItem in itemsToRemove)
                {
                    var stock = stocks[removeItem.StockId];

                    stock.OutwardQty -= removeItem.Qty;
                    stock.InwardQty += removeItem.Qty;
                    _context.DeliveryChallanItems.Remove(removeItem);
                }

                // SAVE ITEM CHANGES FIRST
                await _context.SaveChangesAsync();

                // RELOAD COMPUTED COLUMNS
                await _context.Entry(deliveryChallan)
                    .Collection(x => x.DeliveryChallanItems)
                    .LoadAsync();

                // HEADER TOTALS
                deliveryChallan.TotalQuantity =
                    deliveryChallan.DeliveryChallanItems.Sum(x => x.Qty);

                deliveryChallan.TotalReturnQty =
                    deliveryChallan.DeliveryChallanItems.Sum(x => x.ReturnQty);

                deliveryChallan.TotalEffectiveQty =
                    deliveryChallan.DeliveryChallanItems.Sum(x => x.EffectiveQty);

                deliveryChallan.TotalDiscount =
                    Math.Round(
                        deliveryChallan.DeliveryChallanItems
                            .Sum(x => x.DiscountAmount),
                        2);

                deliveryChallan.TotalGst =
                    Math.Round(
                        deliveryChallan.DeliveryChallanItems
                            .Sum(x => x.GstAmount),
                        2);

                deliveryChallan.TotalAmount =
                    Math.Round(
                        deliveryChallan.DeliveryChallanItems
                            .Sum(x => x.TotalAmount),
                        2);

                // SAVE HEADER TOTALS
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
