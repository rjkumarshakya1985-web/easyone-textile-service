using MediatR;
using System.Threading.Tasks;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Enums;
using Textile.Core.Interfaces.Data;
using Textile.Core.Managers.Commands.Suppliers;

namespace Textile.Core.Managers.Handlers.Query.Suppliers
{


    public class UpdateSupplierStatusCommandHandler
    : IRequestHandler<UpdateSupplierStatusCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateSupplierStatusCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateSupplierStatusCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            var repository = _unitOfWork.Repository<Supplier, Guid>();

            // ---------------------------
            // Find existing mapping
            // ---------------------------
            var entity = await repository.GetSingleAsync(x =>
                x.Id == request.SupplierId);

            if (entity == null)
                throw new Exception("Supplier not found");

            // ---------------------------
            // Perform Action
            // ---------------------------
            switch (request.ActionType)
            {
                case SupplierStatusActionType.Delete:
                    entity.IsDeleted = true;
                    entity.IsActive = false;
                    await UpdateSupplierUserStatus(entity, false);
                    break;

                case SupplierStatusActionType.Activate:
                    entity.IsActive = true;
                    entity.IsDeleted = false;
                    await UpdateSupplierUserStatus(entity, true);
                    break;

                case SupplierStatusActionType.Deactivate:
                    entity.IsActive = false;
                    await UpdateSupplierUserStatus(entity, false);
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


        private async Task UpdateSupplierUserStatus(Supplier supplier,bool status)
        {
            var repository = _unitOfWork.Repository<User, Guid>();
            var user = await repository.GetByIdAsync(supplier.UserId);
            if(user!=null)
            {
                user.IsActive = status;

               await repository.UpdateAsync(user);

            }
        }
    }

}
