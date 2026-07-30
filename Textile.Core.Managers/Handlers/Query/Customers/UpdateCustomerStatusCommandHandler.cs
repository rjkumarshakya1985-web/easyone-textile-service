using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Enums;
using Textile.Core.Interfaces.Data;
using Textile.Core.Managers.Commands.Customers;

namespace Textile.Core.Managers.Handlers.Query.Customers
{
    public class UpdateCustomerStatusCommandHandler
   : IRequestHandler<UpdateCustomerStatusCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCustomerStatusCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateCustomerStatusCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            var repository = _unitOfWork.Repository<Customer, Guid>();

            // ---------------------------
            // Find existing mapping
            // ---------------------------
            var entity = await repository.GetSingleAsync(x =>
                x.Id == request.CustomerId);

            if (entity == null)
                throw new Exception("Customer not found");

            // ---------------------------
            // Perform Action
            // ---------------------------
            switch (request.ActionType)
            {
                case CustomerStatusActionType.Delete:
                    entity.IsDeleted = true;
                    entity.IsActive = false;
                    await UpdateCustomerUserStatus(entity, false);
                    break;

                case CustomerStatusActionType.Activate:
                    entity.IsActive = true;
                    entity.IsDeleted = false;
                    await UpdateCustomerUserStatus(entity, true);
                    break;

                case CustomerStatusActionType.Deactivate:
                    entity.IsActive = false;
                    await UpdateCustomerUserStatus(entity, false);
                    break;

                default:
                    throw new Exception("Invalid action type");
            }

            // ---------------------------
            // Save changes
            // ---------------------------
            await repository.UpdateAsync(entity);

            return true;
        }


        private async Task UpdateCustomerUserStatus(Customer customer, bool status)
        {
            var repository = _unitOfWork.Repository<User, Guid>();
            var user = await repository.GetByIdAsync(customer.Id);
            if (user != null)
            {
                user.IsActive = status;

                await repository.UpdateAsync(user);

            }
        }
    }
}
