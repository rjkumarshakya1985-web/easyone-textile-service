using MediatR;
using Textile.Core.Entities.Models.Response.Departments;

namespace Textile.Core.Managers.Query.Departments
{
    public class GetDepartmentsQuery : IRequest<IEnumerable<DepartmentResponse>>
    {
    }

}
