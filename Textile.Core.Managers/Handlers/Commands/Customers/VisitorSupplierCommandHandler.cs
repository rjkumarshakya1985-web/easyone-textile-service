using Azure.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Enums;
using Textile.Core.Entities.Models.Response.Customers;
using Textile.Core.Entities.Models.Response.Visitors;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Interfaces.Data;
using Textile.Core.Interfaces.Services;
using Textile.Core.Managers.Commands.Customers;
using Textile.Core.Managers.Commands.Users;

namespace Textile.Core.Managers.Handlers.Commands.Customers
{
   
    public class VisitorSupplierCommandHandler : IRequestHandler<VisitorSupplierCommand, VisitorResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly TextileDbContext _context;
        private readonly ICustomerService _customerService;
        private readonly IVisitorService _visitorService;

        public VisitorSupplierCommandHandler(IUnitOfWork unitOfWork, 
            TextileDbContext context,ICustomerService customerService,
            IVisitorService visitorService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork)); ;
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            _visitorService = visitorService ?? throw new ArgumentNullException(nameof(visitorService));
        }

        public async Task<VisitorResponse> Handle(VisitorSupplierCommand command, CancellationToken cancellationToken)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var request = command.Request ?? throw new ArgumentException("Request cannot be null");

            if (command.VisitorId <= 0)
                throw new ArgumentException("Invalid Visitor Id");

            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                // ✅ Create Customer
                var customerId = await _customerService.CreateAsync(
                    request,
                    command.CurrentUserId,
                    command.CurrentUserName);

                // ✅ Fetch Visitor
                var visitor = await _context.Visitors
                    .FirstOrDefaultAsync(x => x.Id == command.VisitorId, cancellationToken);

                if (visitor == null)
                    throw new Exception("Visitor not found");

                // ✅ Update Visitor
                visitor.CustomerId = customerId;

                await _context.SaveChangesAsync(cancellationToken);

                // ✅ Fetch updated visitor with relations
                var updatedVisitor = await _context.Visitors
                    .Include(x => x.Customer)
                    .Include(x => x.City)
                    .FirstOrDefaultAsync(x => x.Id == command.VisitorId, cancellationToken);

                if (updatedVisitor == null)
                    throw new Exception("Visitor not found after update");

                await transaction.CommitAsync(cancellationToken);

                // ✅ Map to response (NO Select here)
                return new VisitorResponse
                {
                    Id = updatedVisitor.Id,
                    Name = updatedVisitor.Name,
                    Mobile = updatedVisitor.Mobile,
                    CustomerType = updatedVisitor.CustomerType,
                    VisitDate = updatedVisitor.VisitDate,
                    Remarks = updatedVisitor.Remarks,
                    CityId = updatedVisitor.CityId,
                    StateId = updatedVisitor.City?.StateId,
                    CreatedBy = updatedVisitor.CreatedBy,
                    CreatedByUserName = updatedVisitor.CreatedByUserName,
                    CreatedOn = updatedVisitor.CreatedOn,
                    ModifiedBy = updatedVisitor.ModifiedBy,
                    ModifiedByUserName = updatedVisitor.ModifiedByUserName,
                    ModifiedOn = updatedVisitor.ModifiedOn,

                    // ✅ Nested Customer Response
                    CustomerResponse = updatedVisitor.Customer == null ? null : new CustomerResponse
                    {
                        Id = updatedVisitor.Customer.Id,
                        Name = updatedVisitor.Customer.Name,
                        Mobile = updatedVisitor.Customer.Mobile
                    }
                };
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
