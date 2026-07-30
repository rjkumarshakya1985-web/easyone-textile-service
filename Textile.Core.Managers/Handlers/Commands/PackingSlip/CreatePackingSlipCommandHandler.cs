using MediatR;
using Microsoft.EntityFrameworkCore;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.DbEnitites.Sales;
using Textile.Core.Entities.Enums;
using Textile.Core.Entities.Models.Requests.PackingSlip;
using Textile.Core.Entities.Models.Requests.PackingSlips;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Managers.Commands.PackingSlip;
using Textile.Core.Managers.Commands.Sales;

namespace Textile.Core.Managers.Handlers.Commands.PackingSlip
{

    public class CreatePackingSlipCommandHandler
    : IRequestHandler<CreatePackingSlipCommand, int>
    {
        private readonly IMediator _mediator;
        private readonly TextileDbContext _context;


        public CreatePackingSlipCommandHandler(IMediator mediator, TextileDbContext context)
        {
            _mediator = mediator;
            _context = context;
        }

        public async Task<int> Handle(CreatePackingSlipCommand requestCommand, CancellationToken cancellationToken)
        {
            var request = requestCommand.PackingSlipRequest;

            ValidateRequest(request);

            var slipNumber = await GenerateSlipNumber(request.FinanceYearId.Value);

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var visitor = await GetVisitor(request.VisitorId);

                var stocks = await GetStocks(request.Items);

                var packingSlip = CreatePackingSlip(request, slipNumber, visitor);
                packingSlip.CreatedBy = requestCommand.CurrentUserId;
                packingSlip.CreatedByUserName   = requestCommand.CurrentUserName;
                packingSlip.UserId = requestCommand.CurrentUserId;

                CalculateAndAddItems(request.Items, stocks, packingSlip);

                CalculateTotals(packingSlip);

                await SavePackingSlip(packingSlip);

                await transaction.CommitAsync();

                return packingSlip.Id;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // ================= METHODS =================

        private void ValidateRequest(PackingSlipRequest request)
        {
            if (request.Items == null || !request.Items.Any())
                throw new Exception("Packing slip must contain at least one item");
        }

        private async Task<string> GenerateSlipNumber(int financeYearId)
        {
            var command = new GenerateVoucherNumberCommand(VoucherTypeEnum.PackingSlip, financeYearId);
            return await _mediator.Send(command);
        }

        private async Task<Visitor?> GetVisitor(int? visitorId)
        {
            if (visitorId == null) return null;

            var visitor = await _context.Visitors
                .Include(x => x.Customer)
                .FirstOrDefaultAsync(x => x.Id == visitorId);

            if (visitor == null)
                throw new Exception("Visitor not found");

            return visitor;
        }

        private async Task<Dictionary<Guid, Stock>> GetStocks(List<PackingSlipItemRequest> items)
        {
            var stockIds = items.Select(x => x.StockId).ToList();

            return await _context.Stocks
                .Where(x => stockIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id);
        }

        private Textile.Core.Entities.DbEnitites.Sales.PackingSlip CreatePackingSlip(PackingSlipRequest request, string slipNumber, Visitor? visitor)
        {
            return new Textile.Core.Entities.DbEnitites.Sales.PackingSlip
            {
                SlipNumber = slipNumber,
                Date = DateTime.Now,
                FinanceYearId = request.FinanceYearId.Value,
                VisitorId = request.VisitorId,
                CustomerId = visitor==null? request.CustomerId: visitor?.CustomerId,
                UserId = request.UserId,
                SalesPersonId = request.SalesPersonId,
                IsDeleted = false,
                CreatedOn = DateTime.Now,
                DiscountPercent = request.DiscountPercent
            };
        }

        private void CalculateAndAddItems(
            List<PackingSlipItemRequest> items,
            Dictionary<Guid, Stock> stocks,
            Textile.Core.Entities.DbEnitites.Sales.PackingSlip packingSlip)
        {
            foreach (var item in items)
            {
                if (!stocks.TryGetValue(item.StockId, out var stock))
                    throw new Exception($"Stock not found for Id {item.StockId}");

                if (stock.AvailableQty < item.Qty)
                    throw new Exception($"Insufficient stock for {stock.Id}");

                stock.ReservedQty += item.Qty;

                var calculatedItem = CalculateItem(item);

                packingSlip.Items.Add(calculatedItem);
            }
        }

        private PackingSlipItem CalculateItem(PackingSlipItemRequest item)
        {
            var taxable = item.SaleRate * item.Qty;

            var discountPercent = item.DiscountPercent ?? 0;
            var discountAmount = taxable * discountPercent / 100;

            var netAmount = taxable - discountAmount;

            var gstAmount = netAmount * item.GstPercent / 100;

            var total = Math.Round(netAmount + gstAmount, 2, MidpointRounding.AwayFromZero);

            return new PackingSlipItem
            {
                StockId = item.StockId,
                SaleRate = item.SaleRate,
                Qty = item.Qty,
                GstPercent = item.GstPercent,
                DiscountPercent = discountPercent,

                TaxableAmount = taxable,
                DiscountAmount = discountAmount,
                NetAmount = netAmount,
                GstAmount = gstAmount,
                TotalAmount = total
            };
        }

        private void CalculateTotals(Textile.Core.Entities.DbEnitites.Sales.PackingSlip packingSlip)
        {
            packingSlip.TotalQuantity = packingSlip.Items.Sum(x => x.Qty);
            packingSlip.TotalDiscount = Math.Round(packingSlip.Items.Sum(x => x.DiscountAmount), 2);
            packingSlip.TotalGst = Math.Round(packingSlip.Items.Sum(x => x.GstAmount), 2);
            packingSlip.TotalAmount = Math.Round(packingSlip.Items.Sum(x => x.TotalAmount), 2);
        }

        private async Task SavePackingSlip(Textile.Core.Entities.DbEnitites.Sales.PackingSlip packingSlip)
        {
            await _context.PackingSlips.AddAsync(packingSlip);
            await _context.SaveChangesAsync();
        }
    }

}
