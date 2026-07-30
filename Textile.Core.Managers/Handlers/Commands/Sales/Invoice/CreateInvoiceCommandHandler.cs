using MediatR;
using Microsoft.EntityFrameworkCore;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.DbEnitites.Sales;
using Textile.Core.Entities.Enums;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Managers.Commands.Sales;
using Textile.Core.Managers.Commands.Sales.Invoices;

namespace Textile.Core.Managers.Handlers.Commands.Sales.Invoice
{


    public class CreateInvoiceCommandHandler
    : IRequestHandler<CreateInvoiceCommand, int>
    {
        private readonly TextileDbContext _context;
        private readonly IMediator _mediator;

        public CreateInvoiceCommandHandler(
            TextileDbContext context,
            IMediator mediator)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<int> Handle(CreateInvoiceCommand cmd, CancellationToken ct)
        {
            if (cmd?.BillingRequest == null)
                throw new ArgumentNullException(nameof(cmd));

            var request = cmd.BillingRequest;
            var now = DateTime.UtcNow;

            if (request.VisitorId == 0)
                throw new ArgumentException("Invalid VisitorId");

            if (request.PackingSlipIds == null || !request.PackingSlipIds.Any())
                throw new ArgumentException("Packing slips required");

            // 🔢 Generate Invoice Number
            var number = await _mediator.Send(
                new GenerateVoucherNumberCommand(
                    VoucherTypeEnum.SaleInvoice,
                    request.FinanceYearId), ct);

            await using var transaction = await _context.Database.BeginTransactionAsync(ct);

            try
            {
                

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

                // 🧾 STEP 1: Create Invoice FIRST
                var invoice = new Textile.Core.Entities.DbEnitites.Sales.Invoice
                {
                    InvoiceNumber = number,
                    UserId = cmd.CurrentUserId,
                    FinanceYearId = request.FinanceYearId,
                    CustomerId = request.CustomerId.Value,
                    BillDiscount = request.BillDiscount,
                    VisitorId = request.VisitorId,
                    CreatedBy = cmd.CurrentUserId,
                    CreatedByUserName = cmd.CurrentUserName,
                    CreatedOn = now,
                    Date = now,
                    Status = (int)InvoiceStatusEnum.Created,
                    IsDeleted = false
                };

                _context.Invoices.Add(invoice);
                await _context.SaveChangesAsync(ct); // ✅ Id generated

                var items = new List<InvoiceItem>();
                var stockTransactions = new List<StockTransaction>();
                var maps = new List<InvoicePackingSlipMap>();

                // 🔁 Process Slips
                foreach (var slip in packingSlips)
                {
                    foreach (var item in slip.Items ?? Enumerable.Empty<PackingSlipItem>())
                    {
                        // ✅ Validation
                        if (item.Qty <= 0)
                            throw new Exception("Invalid quantity");

                        if (!stocks.TryGetValue(item.StockId, out var stock))
                            throw new Exception($"Stock not found for {item.StockId}");

                        if (stock.ReservedQty < item.Qty)
                            throw new Exception("Insufficient reserved stock");

                        // 🧾 Invoice Item
                        var invoiceItem = new InvoiceItem
                        {
                            InvoiceId = invoice.Id,
                            PackingSlipItemId = item.Id,
                            SalesPersonId = slip.SalesPersonId,
                            StockId = item.StockId,
                            SaleRate = item.SaleRate,
                            Qty = item.Qty,
                            DiscountPercent = item.DiscountPercent,
                            GstPercent = item.GstPercent,
                        };

                        items.Add(invoiceItem);

                        // 📉 Stock Transaction
                        stockTransactions.Add(new StockTransaction
                        {
                            ProductId = stock.ProductId,
                            VoucherId = invoice.Id,
                            VoucherType = (int)VoucherTypeEnum.SaleInvoice,
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
                    maps.Add(new InvoicePackingSlipMap
                    {
                        InvoiceId = invoice.Id,
                        PackingSlipId = slip.Id
                    });

                    slip.Status = (int)PackingSlipStatusEnum.Invoice;
                }

                // 📥 Bulk Insert
                _context.InvoiceItems.AddRange(items);
                _context.StockTransactions.AddRange(stockTransactions);
                _context.InvoicePackingSlipMaps.AddRange(maps);

                await _context.SaveChangesAsync(ct);

                // 🔄 Reload Items (if computed columns exist)
                var invoiceItems = await _context.InvoiceItems
                    .Where(x => x.InvoiceId == invoice.Id)
                    .ToListAsync(ct);

                // 🧮 Calculate totals
                invoice.TotalQuantity = invoiceItems.Sum(x => x.Qty);
                invoice.TotalDiscount = invoiceItems.Sum(x => x.DiscountAmount);
                invoice.TotalGst = invoiceItems.Sum(x => x.GstAmount);
                invoice.TotalAmount = invoiceItems.Sum(x => x.TotalAmount);
               

                // 👉 Add if needed

                await _context.SaveChangesAsync(ct);

                await transaction.CommitAsync(ct);

                return invoice.Id;
            }
            catch
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        }

        // 👤 Create Customer (Aligned with Challan)
      
    }
}
