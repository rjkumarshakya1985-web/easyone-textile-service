using MediatR;
using Microsoft.EntityFrameworkCore;
using Textile.Core.Entities.DbEnitites.Sales;
using Textile.Core.Entities.Enums;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Managers.Commands.Sales;
using Textile.Core.Managers.Commands.Sales.Invoices;

namespace Textile.Core.Managers.Handlers.Commands.Sales.Invoice
{
    public class CreateDeliveryChallansToInvoiceCommandHandler
    : IRequestHandler<CreateDeliveryChallansToInvoiceCommand, int>
    {
        private readonly TextileDbContext _context;
        private readonly IMediator _mediator;

        public CreateDeliveryChallansToInvoiceCommandHandler(
            TextileDbContext context,
            IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<int> Handle(
            CreateDeliveryChallansToInvoiceCommand  deliveryChallansToInvoiceCommand,
            CancellationToken cancellationToken)
        {

            var request = deliveryChallansToInvoiceCommand.InvoiceRequest;
            if (request == null || request.DeliveryChallanIds == null || !request.DeliveryChallanIds.Any())
                throw new Exception("No delivery challan selected");

            var invoiceNumber = await _mediator.Send(
                new GenerateVoucherNumberCommand(VoucherTypeEnum.SaleInvoice, request.FinYearId),
                cancellationToken);

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var challans = await _context.DeliveryChallans
                    .Include(x => x.Customer)
                    .Include(x => x.DeliveryChallanItems)
                    .Where(x => request.DeliveryChallanIds.Contains(x.Id)
                                && x.FinanceYearId == request.FinYearId)
                    .ToListAsync(cancellationToken);

                if (!challans.Any())
                    throw new Exception("No valid challans found");

                var customerId = challans.First().CustomerId;

                if (challans.Any(x => x.CustomerId != customerId))
                    throw new Exception("Different customer challans not allowed");

                var invoice = new Textile.Core.Entities.DbEnitites.Sales.Invoice
                {
                    UserId = deliveryChallansToInvoiceCommand.CurrentUserId,
                    InvoiceNumber = invoiceNumber,
                    Date = DateTime.Now,
                    FinanceYearId = request.FinYearId,
                    CustomerId = customerId.Value,
                    BillDiscount = request.BillDiscount,
                    TotalQuantity = challans.Sum(x=>x.TotalEffectiveQty),
                    TotalDiscount = challans.Sum(x => x.TotalDiscount),
                    TotalGst = challans.Sum(x => x.TotalGst),
                    TotalAmount = challans.Sum(x => x.TotalAmount),
                    CreatedBy = deliveryChallansToInvoiceCommand.CurrentUserId,
                    CreatedByUserName = deliveryChallansToInvoiceCommand.CurrentUserName,
                    CreatedOn = DateTime.Now,
                    Status = (int)InvoiceStatusEnum.DeliveryChallanInvoice,
                    IsDeleted = false
                };

                _context.Invoices.Add(invoice);
                await _context.SaveChangesAsync(cancellationToken);

                var invoiceItems = new List<InvoiceItem>();

                foreach (var challan in challans)
                {
                    _context.InvoiceDeliveryChallanMaps.Add(new InvoiceDeliveryChallanMap
                    {
                        InvoiceId = invoice.Id,
                        DeliveryChallanId = challan.Id
                    });

                    foreach (var item in challan.DeliveryChallanItems)
                    {
                       
                        invoiceItems.Add(new InvoiceItem
                        {
                            InvoiceId = invoice.Id,
                            DeliveryChallanItemId = item.Id,
                            PackingSlipItemId = item.PackingSlipItemId,
                            SalesPersonId = item.SalesPersonId,
                            StockId = item.StockId,
                            SaleRate = item.SaleRate,
                            Qty = item.EffectiveQty,
                            DiscountPercent = item.DiscountPercent,
                            GstPercent = item.GstPercent,
                            IsDeleted =false
                            
                        });

                        
                    }

                    challan.Status = (int)DeliveryChallanStatusEnum.Invoiced;
                }

                _context.InvoiceItems.AddRange(invoiceItems);

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return invoice.Id;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
