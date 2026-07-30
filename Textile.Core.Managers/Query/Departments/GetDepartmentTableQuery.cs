using MediatR;
using Textile.Core.Entities.Models.Response.Departments;

namespace Textile.Core.Managers.Query.Departments
{
    public class GetDepartmentTableQuery : IRequest<IEnumerable<SubDepartmentsResponse>>
    {
    }

}
