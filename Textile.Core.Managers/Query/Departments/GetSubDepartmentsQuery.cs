using MediatR;
using Textile.Core.Entities.Models.Response.Departments;

namespace Textile.Core.Managers.Query.Departments
{
    
    public class GetSubDepartmentsQuery : IRequest<IEnumerable<SubDepartmentResponse>>
    {
        public int DepartmentId { get; set; }
        public GetSubDepartmentsQuery(int departmentId)
        {
            DepartmentId = departmentId;
        }
    }
}
