using MediatR;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Interfaces.Data;
using Textile.Core.Managers.Commands.Departments;

namespace Textile.Core.Managers.Handlers.Commands.Departments
{
    public class SaveSubDepartmentCommandHandler
   : IRequestHandler<SaveSubDepartmentCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public SaveSubDepartmentCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(SaveSubDepartmentCommand request, CancellationToken cancellationToken)
        {
            var repoSubDepartment = _unitOfWork.Repository<SubDepartment, int>();

            SubDepartment? subDepartment;

            if (request.DepartmentRequest.SubDepartmentId>0 && request.DepartmentRequest.DepartmentId!=null)
            {
                subDepartment = await repoSubDepartment.GetByIdAsync(request.DepartmentRequest.SubDepartmentId.Value);

                if (subDepartment == null)
                    return false; // Or throw exception

                subDepartment.DepartmentId = request.DepartmentRequest.DepartmentId.Value;
                subDepartment.Name = request.DepartmentRequest.Name;
                subDepartment.Description = request.DepartmentRequest.Description;

                await repoSubDepartment.UpdateAsync(subDepartment);
            }
            else
            {
                var department = new SubDepartment
                {
                    Id = request.DepartmentRequest.SubDepartmentId.Value,
                    DepartmentId = request.DepartmentRequest.DepartmentId.Value,
                    Name = request.DepartmentRequest.Name,
                    Description = request.DepartmentRequest.Description,
                    IsActive = true
                };

                await repoSubDepartment.AddAsync(department);
            }
              
            return true;


        }
    }
}
