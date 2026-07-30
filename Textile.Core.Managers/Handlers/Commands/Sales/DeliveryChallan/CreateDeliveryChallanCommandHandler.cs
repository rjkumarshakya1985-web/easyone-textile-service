using MediatR;
using Microsoft.EntityFrameworkCore;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.DbEnitites.Sales;
using Textile.Core.Entities.Enums;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Managers.Commands.Sales;
using Textile.Core.Managers.Commands.Sales.DeliveryNotes;

namespace Textile.Core.Managers.Handlers.Commands.Sales.DeliveryChallan
{

    public class CreateDeliveryChallanCommandHandler
     : IRequestHandler<CreateDeliveryChallanCommand, int>
    {
        private readonly TextileDbContext _context;
        private readonly IMediator _mediator;

        public CreateDeliveryChallanCommandHandler(
            TextileDbContext context,
            IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<int> Handle(CreateDeliveryChallanCommand cmd, CancellationToken ct)
        {
            if (cmd?.BillingRequest == null)
                throw new ArgumentNullException(nameof(cmd));

            var request = cmd.BillingRequest;
            var now = DateTime.UtcNow;

            if (request.VisitorId == 0)
                throw new ArgumentException("Invalid VisitorId");

            if (request.PackingSlipIds == null || !request.PackingSlipIds.Any())
                throw new ArgumentException("Packing slips required");

            // 🔢 Generate Voucher Number
            var number = await _mediator.Send(
                new GenerateVoucherNumberCommand(
                    VoucherTypeEnum.DeliveryChallan,
                    request.FinanceYearId), ct);

            await using var transaction = await _context.Database.BeginTransactionAsync(ct);

            try
            {
                // 👤 Ensure Customer
                if (request.CustomerId == null)
                {
                    request.CustomerId = await CreateCustomer(
                        request.VisitorId,
                        cmd.CurrentUserId,
                        cmd.CurrentUserName);
                }

                // 📦 Fetch Packing Slips
                var packingSlips = await _context.PackingSlips
                    .AsTracking()
                    .Include(x => x.Items)
                    .Where(x => request.PackingSlipIds.Contains(x.Id) && !x.IsDeleted)
                    .ToListAsync(ct);

                if (!packingSlips.Any())
                    throw new Exception("No valid packing slips found");

                if (packingSlips.Any(x => x.Status != (int)PackingSlipStatusEnum.Created))
                    throw new Exception("Some packing slips already processed");

                if (packingSlips.Any(x => x.VisitorId != request.VisitorId))
                    throw new Exception("All packing slips must belong to same visitor");

                // 📊 Fetch Stocks
                var stockIds = packingSlips
                    .SelectMany(x => x.Items ?? Enumerable.Empty<PackingSlipItem>())
                    .Select(x => x.StockId)
                    .Distinct()
                    .ToList();

                var stocks = await _context.Stocks
                    .Where(x => stockIds.Contains(x.Id))
                    .ToDictionaryAsync(x => x.Id, ct);

                // 🧾 STEP 1: Create Challan FIRST (Id required)
                var challan = new Textile.Core.Entities.DbEnitites.Sales.DeliveryChallan
                {
                    DeliveryChallanNumber = number,
                    UserId = cmd.CurrentUserId,
                    FinanceYearId = request.FinanceYearId,
                    CustomerId = request.CustomerId.Value,
                    VisitorId = request.VisitorId,
                    CreatedBy = cmd.CurrentUserId,
                    CreatedByUserName = cmd.CurrentUserName,
                    CreatedOn = now,
                    Date = now,
                    Status = (int)DeliveryChallanStatusEnum.Created,
                    IsDeleted = false
                };

                _context.DeliveryChallans.Add(challan);
                await _context.SaveChangesAsync(ct); // ✅ Id generated

                var items = new List<DeliveryChallanItem>();
                var stockTransactions = new List<StockTransaction>();
                var maps = new List<DeliveryChallanPackingSlipMap>();

                // 🔁 Process Slips
                foreach (var slip in packingSlips)
                {
                    foreach (var item in slip.Items ?? Enumerable.Empty<PackingSlipItem>())
                    {
                        if (item.Qty <= 0)
                            throw new Exception("Invalid quantity");

                        if (!stocks.TryGetValue(item.StockId, out var stock))
                            throw new Exception($"Stock not found for {item.StockId}");

                        if (stock.ReservedQty < item.Qty)
                            throw new Exception("Insufficient reserved stock");

                        // ✅ Only base fields
                        var dcItem = new DeliveryChallanItem
                        {
                            DeliveryChallanId = challan.Id,
                            PackingSlipItemId = item.Id,
                            StockId = item.StockId,
                            SalesPersonId = slip.SalesPersonId,
                            Qty = item.Qty,
                            SaleRate = item.SaleRate,
                            DiscountPercent = item.DiscountPercent,
                            GstPercent = item.GstPercent
                        };

                        items.Add(dcItem);

                        // 📉 Stock Transaction (GENERIC SYSTEM)
                        stockTransactions.Add(new StockTransaction
                        {
                            ProductId = stock.ProductId,
                            VoucherId = challan.Id, // ✅ SAFE NOW
                            VoucherType = (int)VoucherTypeEnum.DeliveryChallan,
                            TransactionType = "OUT",
                            Quantity = item.Qty,
                            TransactionDate = now,
                            CreatedAt = now
                        });

                        // 🔄 Update stock
                        stock.ReservedQty -= item.Qty;
                        stock.OutwardQty += item.Qty;
                    }

                    // 🔗 Mapping
                    maps.Add(new DeliveryChallanPackingSlipMap
                    {
                        DeliveryChallanId = challan.Id,
                        PackingSlipId = slip.Id
                    });

                    slip.Status = (int)PackingSlipStatusEnum.DeliveryChallan;
                }

                // 📥 Bulk Insert
                _context.DeliveryChallanItems.AddRange(items);
                _context.StockTransactions.AddRange(stockTransactions);
                _context.DeliveryChallanPackingSlipMaps.AddRange(maps);

                await _context.SaveChangesAsync(ct);

                // 🔄 Reload Items (computed columns)
                var challanItems = await _context.DeliveryChallanItems
                    .Where(x => x.DeliveryChallanId == challan.Id)
                    .ToListAsync(ct);

                // 🧮 Calculate totals
                challan.TotalAmount = challanItems.Sum(x => x.TotalAmount);
                challan.TotalGst = challanItems.Sum(x => x.GstAmount);
                challan.TotalDiscount = challanItems.Sum(x => x.DiscountAmount);
                challan.TotalQuantity = challanItems.Sum(x => x.Qty);
                challan.DiscountPercent = packingSlips.FirstOrDefault().DiscountPercent;
                await _context.SaveChangesAsync(ct);

                await transaction.CommitAsync(ct);

                return challan.Id;
            }
            catch
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        }

        // 👤 Create Customer
        private async Task<Guid?> CreateCustomer(int visitorId, Guid userId, string userName)
        {
            var visitor = await _context.Visitors.FindAsync(visitorId);

            if (visitor == null)
                throw new Exception("Visitor not found");

            if (visitor.CustomerId != null)
                return visitor.CustomerId;

            var customer = new Customer
            {
                Name = visitor.Name,
                Alias = visitor.Name,
                PrintName = visitor.Name,
                CityId = visitor.CityId ?? 0,
                CreatedBy = userId,
                CreatedByUserName = userName,
                CreatedOn = DateTime.UtcNow,
                IsActive = true,
                IsDeleted = false
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            visitor.CustomerId = customer.Id;
            await _context.SaveChangesAsync();

            return customer.Id;
        }
    }
}
