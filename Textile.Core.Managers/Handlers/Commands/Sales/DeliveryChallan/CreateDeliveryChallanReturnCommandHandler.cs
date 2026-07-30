using MediatR;
using Microsoft.EntityFrameworkCore;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.DbEnitites.Sales;
using Textile.Core.Entities.Enums;
using Textile.Core.Entities.Models.Requests.Billing.DeliveryChallan;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Managers.Commands.Sales;
using Textile.Core.Managers.Commands.Sales.DeliveryNotes;

namespace Textile.Core.Managers.Handlers.Commands.Sales.DeliveryChallan
{

    public class CreateDeliveryChallanReturnCommandHandler
    : IRequestHandler<CreateDeliveryChallanReturnCommand, int>
    {
        private readonly TextileDbContext _context;
        private readonly IMediator _mediator;

        public CreateDeliveryChallanReturnCommandHandler(
            TextileDbContext context,
            IMediator mediator)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<int> Handle(CreateDeliveryChallanReturnCommand cmd, CancellationToken ct)
        {
            var now = DateTime.UtcNow;

            ValidateRequest(cmd);

            var request = cmd.DeliveryChalanReturnRequest;

            var number = await GenerateVoucherNumber(request, ct);

            await using var transaction = await _context.Database.BeginTransactionAsync(ct);

            try
            {
                var challan = await GetChallan(request, ct);

                var returnEntity = CreateReturnHeader(cmd, request, number, now);

                _context.DeliveryChallanReturns.Add(returnEntity);
                await _context.SaveChangesAsync(ct);

                var stocks = await GetStocks(request, ct);
                var returnData = await GetReturnData(request, ct);

                var (returnItems, stockTransactions) = ProcessItems(
                    request, challan, returnEntity, stocks, returnData, now);

                _context.DeliveryChallanReturnItems.AddRange(returnItems);
                _context.StockTransactions.AddRange(stockTransactions);

                await _context.SaveChangesAsync(ct);

                // 🔥 IMPORTANT: reload to avoid stale computed values
                await _context.Entry(challan)
                    .Collection(x => x.DeliveryChallanItems)
                    .LoadAsync(ct);

                UpdateChallanTotals(challan);
                UpdateChallanStatus(challan, request, returnData);

                await _context.SaveChangesAsync(ct);

                await transaction.CommitAsync(ct);

                return returnEntity.Id;
            }
            catch
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        }
        // ================= VALIDATION =================

        private void ValidateRequest(CreateDeliveryChallanReturnCommand cmd)
        {
            if (cmd?.DeliveryChalanReturnRequest == null)
                throw new ArgumentNullException(nameof(cmd));

            var items = cmd.DeliveryChalanReturnRequest.DeliveryChallanReturnItems;

            if (items == null || !items.Any())
                throw new Exception("Return items required");

            if (items.GroupBy(x => x.DeliveryChallanItemId).Any(g => g.Count() > 1))
                throw new Exception("Duplicate items in return request");
        }

        // ================= GENERATE NUMBER =================

        private async Task<string> GenerateVoucherNumber(DeliveryChalanReturnRequest request, CancellationToken ct)
        {
            return await _mediator.Send(
                new GenerateVoucherNumberCommand(
                    VoucherTypeEnum.DeliveryChallanReturn,
                    request.FinanceYearId), ct);
        }

        // ================= GET CHALLAN =================

        private async Task<Textile.Core.Entities.DbEnitites.Sales.DeliveryChallan> GetChallan(DeliveryChalanReturnRequest request, CancellationToken ct)
        {
            var challan = await _context.DeliveryChallans
                .Include(x => x.DeliveryChallanItems)
                .FirstOrDefaultAsync(x => x.Id == request.DeliveryChallanId && !x.IsDeleted, ct);

            if (challan == null)
                throw new Exception("Delivery Challan not found");

            if (challan.CustomerId != request.CustomerId)
                throw new Exception("Customer mismatch");

            return challan;
        }

        // ================= HEADER =================

        private DeliveryChallanReturn CreateReturnHeader(
            CreateDeliveryChallanReturnCommand cmd,
            DeliveryChalanReturnRequest request,
            string number,
            DateTime now)
        {
            return new DeliveryChallanReturn
            {
                ReturnNumber = number,
                ReturnDate = now,
                DeliveryChallanId = request.DeliveryChallanId,
                FinanceYearId = request.FinanceYearId,
                CustomerId = request.CustomerId,
                CreatedBy = cmd.CurrentUserId,
                CreatedByUserName = cmd.CurrentUserName,
                CreatedOn = now,
                IsDeleted = false
            };
        }

        // ================= STOCK =================

        private async Task<Dictionary<Guid, Stock>> GetStocks(DeliveryChalanReturnRequest request, CancellationToken ct)
        {
            var stockIds = request.DeliveryChallanReturnItems
                .Select(x => x.StockId)
                .Distinct()
                .ToList();

            return await _context.Stocks
                .Where(x => stockIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, ct);
        }

        // ================= RETURN DATA =================

        private async Task<Dictionary<int, int>> GetReturnData(DeliveryChalanReturnRequest request, CancellationToken ct)
        {
            var dcItemIds = request.DeliveryChallanReturnItems
                .Select(x => x.DeliveryChallanItemId)
                .ToList();

            return await _context.DeliveryChallanReturnItems
                .Where(x => dcItemIds.Contains(x.DeliveryChallanItemId))
                .GroupBy(x => x.DeliveryChallanItemId)
                .Select(g => new
                {
                    DeliveryChallanItemId = g.Key,
                    ReturnedQty = g.Sum(x => x.ReturnQty)
                })
                .ToDictionaryAsync(x => x.DeliveryChallanItemId, x => x.ReturnedQty, ct);
        }

        // ================= MAIN PROCESS =================

        private (List<DeliveryChallanReturnItem>, List<StockTransaction>) ProcessItems(
      DeliveryChalanReturnRequest request,
      Textile.Core.Entities.DbEnitites.Sales.DeliveryChallan challan,
      DeliveryChallanReturn returnEntity,
      Dictionary<Guid, Stock> stocks,
      Dictionary<int, int> returnData,
      DateTime now)
        {
            var returnItems = new List<DeliveryChallanReturnItem>();
            var stockTransactions = new List<StockTransaction>();

            var dcItemsDict = challan.DeliveryChallanItems.ToDictionary(x => x.Id);

            foreach (var item in request.DeliveryChallanReturnItems)
            {
                if (item.ReturnQty <= 0)
                    throw new Exception("Invalid return qty");

                if (!dcItemsDict.TryGetValue(item.DeliveryChallanItemId, out var dcItem))
                    throw new Exception("Invalid DC Item");

                returnData.TryGetValue(dcItem.Id, out var alreadyReturned);

                if (item.ReturnQty > (dcItem.Qty - alreadyReturned))
                    throw new Exception("Return qty exceeds balance");

                if (!stocks.TryGetValue(item.StockId, out var stock))
                    throw new Exception("Stock not found");

                returnItems.Add(new DeliveryChallanReturnItem
                {
                    DeliveryChallanReturnId = returnEntity.Id,
                    DeliveryChallanItemId = dcItem.Id,
                    StockId = item.StockId,
                    ReturnQty = item.ReturnQty,
                    SalesPersonId = dcItem.SalesPersonId
                });

                // 🔥 only state update
                dcItem.ReturnQty += item.ReturnQty;

                stock.InwardQty += item.ReturnQty;

                stockTransactions.Add(new StockTransaction
                {
                    ProductId = stock.ProductId,
                    VoucherId = returnEntity.Id,
                    VoucherType = (int)VoucherTypeEnum.DeliveryChallanReturn,
                    TransactionType = "IN",
                    Quantity = item.ReturnQty,
                    TransactionDate = now,
                    CreatedAt = now
                });
            }

            return (returnItems, stockTransactions);
        }

        // ================= DC ITEM UPDATE =================

        private void UpdateDcItem(DeliveryChallanItem dcItem, int returnQty)
        {
            dcItem.ReturnQty += returnQty;

            var remainingQty = dcItem.Qty - dcItem.ReturnQty;
        }

        // ================= TOTAL =================

      

        private void UpdateChallanTotals(Textile.Core.Entities.DbEnitites.Sales.DeliveryChallan challan)
        {
            challan.TotalDiscount = 0;
            challan.TotalGst = 0;
            challan.TotalAmount = 0;
            challan.TotalReturnQty = challan.DeliveryChallanItems.Sum(x => x.ReturnQty);

            foreach (var item in challan.DeliveryChallanItems)
            {
                var netQty = item.Qty - item.ReturnQty;
               
                var baseAmount = netQty * item.SaleRate;

                var discount = item.DiscountAmount;

                var taxable = baseAmount - discount;

                var gst = Math.Round(taxable * item.GstPercent / 100, 2);

                var total = taxable + gst;

                challan.TotalDiscount += Math.Round(discount,0);
                challan.TotalGst += Math.Round(gst,0);
                challan.TotalAmount += Math.Round(total,0);
            }
        }

        // ================= STATUS =================

        private void UpdateChallanStatus(
            Textile.Core.Entities.DbEnitites.Sales.DeliveryChallan challan,
            DeliveryChalanReturnRequest request,
            Dictionary<int, int> returnData)
        {
            int fullyReturned = 0;
            int partialReturned = 0;

            var requestDict = request.DeliveryChallanReturnItems
                .ToDictionary(x => x.DeliveryChallanItemId, x => x.ReturnQty);

            foreach (var dcItem in challan.DeliveryChallanItems)
            {
                var alreadyReturned = returnData.GetValueOrDefault(dcItem.Id);
                var currentReturn = requestDict.GetValueOrDefault(dcItem.Id);

                var totalReturned = alreadyReturned + currentReturn;

                if (totalReturned == 0) continue;

                if (totalReturned == dcItem.Qty)
                    fullyReturned++;
                else
                    partialReturned++;
            }

            if (fullyReturned == challan.DeliveryChallanItems.Count)
                challan.Status = (int)DeliveryChallanStatusEnum.FullyReturned;
            else if (fullyReturned > 0 || partialReturned > 0)
                challan.Status = (int)DeliveryChallanStatusEnum.PartiallyReturned;
        }
    }
}
