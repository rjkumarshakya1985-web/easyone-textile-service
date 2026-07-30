using MediatR;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Interfaces.Data;
using Textile.Core.Managers.Commands.Departments;

namespace Textile.Core.Managers.Handlers.Commands.Departments
{
    public class SaveDepartmentCommandHandler
    : IRequestHandler<SaveDepartmentCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public SaveDepartmentCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(SaveDepartmentCommand request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.Repository<Department, int>();

            Department? department;

            // ----- UPDATE CASE -----
            if (request.DepartmentRequest.DepartmentId != null)
            {
                department = await repo.GetByIdAsync(request.DepartmentRequest.DepartmentId.Value);

                if (department == null)
                    return false; // Or throw exception

                department.Name = request.DepartmentRequest.Name;
                department.Description = request.DepartmentRequest.Description;

                await repo.UpdateAsync(department);
            }
            // ----- ADD CASE -----
            else
            {
                department = new Department
                {
                    Name = request.DepartmentRequest.Name,
                    Description = request.DepartmentRequest.Description,
                    IsActive = true
                };

                await repo.AddAsync(department);
            }

            return true;
        }

    }

}
