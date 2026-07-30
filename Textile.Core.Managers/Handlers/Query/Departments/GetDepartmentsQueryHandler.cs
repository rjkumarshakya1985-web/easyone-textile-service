using MediatR;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Models.Response.Departments;
using Textile.Core.Interfaces.Data;
using Textile.Core.Managers.Query.Departments;

namespace Textile.Core.Managers.Handlers.Query.Departments
{
    public class GetDepartmentsQueryHandler
    : IRequestHandler<GetDepartmentsQuery, IEnumerable<DepartmentResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDepartmentsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<DepartmentResponse>> Handle(GetDepartmentsQuery request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.Repository<Department, int>();

            var departments = await repo.GetAllAsync();

            if (departments == null || !departments.Any())
                return Enumerable.Empty<DepartmentResponse>();

            return departments.Select(d => new DepartmentResponse
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description,
                IsActive = d.IsActive
            });
        }
    }

}
