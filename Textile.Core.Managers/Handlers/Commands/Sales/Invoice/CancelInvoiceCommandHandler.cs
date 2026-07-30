using MediatR;
using Microsoft.EntityFrameworkCore;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Enums;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Managers.Commands.Sales.Invoices;

namespace Textile.Core.Managers.Handlers.Commands.Sales.Invoice
{
    public class CancelInvoiceCommandHandler
     : IRequestHandler<CancelInvoiceCommand, bool>
    {
        private readonly TextileDbContext _context;

        public CancelInvoiceCommandHandler(TextileDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(CancelInvoiceCommand request, CancellationToken ct)
        {
            var now = DateTime.UtcNow;

            using var transaction = await _context.Database.BeginTransactionAsync(ct);

            try
            {
                var invoice = await _context.Invoices
                    .Include(x => x.InvoiceItems)
                    .FirstOrDefaultAsync(x => x.Id == request.InvoiceId && !x.IsDeleted, ct);

                if (invoice == null)
                    throw new Exception("Invoice not found");

                //  STATUS VALIDATION
                if (invoice.Status != (int)InvoiceStatusEnum.Created)
                    throw new Exception("Only created invoice can be cancelled");

               
                var stockIds = invoice.InvoiceItems
                    .Select(x => x.StockId)
                    .Distinct()
                    .ToList();

                var stocks = await _context.Stocks
                    .Where(x => stockIds.Contains(x.Id))
                    .ToDictionaryAsync(x => x.Id, ct);

                var stockTransactions = new List<StockTransaction>();

                // 🔥 Reverse stock
                foreach (var item in invoice.InvoiceItems)
                {
                    if (!stocks.TryGetValue(item.StockId, out var stock))
                        throw new Exception($"Stock not found: {item.StockId}");

                    // 🔁 Reverse OUT → IN
                    stock.OutwardQty -= item.Qty;
                    
                    stockTransactions.Add(new StockTransaction
                    {
                        ProductId = stock.ProductId,
                        VoucherId = invoice.Id,
                        VoucherType = (int)VoucherTypeEnum.SaleInvoice,
                        TransactionType = "IN", // reverse
                        Quantity = item.Qty,
                        TransactionDate = now,
                        CreatedAt = now
                    });
                }

                // 🔥 Reset PackingSlip Status
                //var mappings = await _context.InvoicePackingSlipMaps
                //    .Where(x => x.InvoiceId == invoice.Id)
                //    .ToListAsync(ct);

                //var packingSlipIds = mappings.Select(x => x.PackingSlipId).ToList();

                //var packingSlips = await _context.PackingSlips
                //    .Where(x => packingSlipIds.Contains(x.Id))
                //    .ToListAsync(ct);

                //foreach (var slip in packingSlips)
                //{
                //    slip.Status = (int)PackingSlipStatusEnum.Cancelled; // 🔁 revert back
                //}

                // 🔥 Update Invoice Status
                invoice.Status = (int)InvoiceStatusEnum.Cancelled;

                invoice.ModifiedBy = request.CurrentUserId;
                invoice.ModifiedByUserName = request.CurrentUserName;
                invoice.ModifiedOn = now;



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
