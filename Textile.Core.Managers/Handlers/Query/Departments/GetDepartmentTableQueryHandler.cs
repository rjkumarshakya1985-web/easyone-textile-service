using MediatR;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Models.Response.Departments;
using Textile.Core.Interfaces.Data;
using Textile.Core.Managers.Query.Departments;

namespace Textile.Core.Managers.Handlers.Query.Departments
{
    public class GetDepartmentTableQueryHandler
    : IRequestHandler<GetDepartmentTableQuery, IEnumerable<SubDepartmentsResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDepartmentTableQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<IEnumerable<SubDepartmentsResponse>> Handle(
            GetDepartmentTableQuery request,
            CancellationToken cancellationToken)
        {
            var departmentRepo = _unitOfWork.Repository<Department, int>();
            var subDepartmentRepo = _unitOfWork.Repository<SubDepartment, int>();

            var departments = await departmentRepo.GetAllAsync();
            var subDepartments = await subDepartmentRepo.GetAllAsync();

            var result = from d in departments
                         join sd in subDepartments
                            on d.Id equals sd.DepartmentId
                         select new SubDepartmentsResponse
                         {
                             Id = d.Id,
                             DepartmentName = d.Name,
                             SubDepartmentId = sd.Id,
                             SubDepartmentName = sd.Name
                         };

            return result;
        }
    }

}
