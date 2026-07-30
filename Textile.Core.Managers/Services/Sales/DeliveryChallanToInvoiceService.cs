using MediatR;
using Microsoft.EntityFrameworkCore;
using Textile.Core.Entities.DbEnitites.Sales;
using Textile.Core.Entities.Enums;
using Textile.Core.Entities.Models.Requests.Billing.Invoices;
using Textile.Core.Entities.Models.Response.Billing.DeliveryChallans;
using Textile.Core.Entities.Models.Response.Customers;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Interfaces.Data;
using Textile.Core.Interfaces.Services.Sales;
using Textile.Core.Managers.Commands.Sales;


namespace Textile.Core.Managers.Services.Sales
{
    public class DeliveryChallanToInvoiceService : IDeliveryChallanToInvoiceService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly TextileDbContext _context;
        private readonly IMediator _mediator;

        public DeliveryChallanToInvoiceService(IUnitOfWork unitOfWork,
            TextileDbContext context, IMediator mediator)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

      

        public async Task<DeliverChallanToInvoiceResponse?> GetDeliveryChallanForInvoiceByNumber(string number,
              int finYearId)
        {
            var challan = await _context.DeliveryChallans
                .Include(x => x.Customer)
                .Include(x => x.DeliveryChallanItems)
                .FirstOrDefaultAsync(x =>
                    x.DeliveryChallanNumber == number &&
                    x.FinanceYearId == finYearId);

            if (challan == null)
                return null;

            var totalQty = challan.DeliveryChallanItems.Sum(x => x.Qty);
            var totalReturnQty = challan.DeliveryChallanItems.Sum(x => x.ReturnQty);

            var availableInvoiceQty = totalQty - totalReturnQty;

            var totalAmount = challan.TotalAmount;

            return new DeliverChallanToInvoiceResponse
            {
                DeliveryChallanId = challan.Id,
                DeiliverChallanNo = challan.DeliveryChallanNumber,
                Quantity = totalQty,
                ReturnQty = totalReturnQty,
                AvailableInvoiceQty = availableInvoiceQty,
                TotalAmount = totalAmount,
                DeliveryChallanStatusEnum = (DeliveryChallanStatusEnum)challan.Status,
                Customer = challan.Customer == null ? null : new CustomerResponse
                {
                    Id = challan.Customer.Id,
                    Name = challan.Customer.Name,
                    Mobile = challan.Customer.Mobile
                }
            };
        }
    }
}
