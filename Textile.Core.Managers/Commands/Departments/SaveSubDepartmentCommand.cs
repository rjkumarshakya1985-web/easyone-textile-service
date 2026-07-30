using MediatR;
using Textile.Core.Entities.Models.Requests.Departments;

namespace Textile.Core.Managers.Commands.Departments
{

    public class SaveSubDepartmentCommand : IRequest<bool>
    {
        public DepartmentRequest DepartmentRequest;

        public SaveSubDepartmentCommand(DepartmentRequest departmentRequest)
        {
            DepartmentRequest = departmentRequest;
        }
    }
}
