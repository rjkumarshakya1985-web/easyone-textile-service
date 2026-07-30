using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Models.Response.Departments;
using Textile.Core.Interfaces.Data;
using Textile.Core.Managers.Query.Departments;

namespace Textile.Core.Managers.Handlers.Query.Departments
{
    public class GetSubDepartmentsQueryHandler
     : IRequestHandler<GetSubDepartmentsQuery, IEnumerable<SubDepartmentResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetSubDepartmentsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<SubDepartmentResponse>> Handle(GetSubDepartmentsQuery query, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.Repository<SubDepartment, int>();

            var departments = await repo.GetAllAsync(x => x.DepartmentId == query.DepartmentId);

            if (departments == null || !departments.Any())
                return Enumerable.Empty<SubDepartmentResponse>();

            return departments.Select(d => new SubDepartmentResponse
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description,
                IsActive = d.IsActive
            });
        }
    }

}
