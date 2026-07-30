using MediatR;
using Textile.Core.Entities.Models.Requests.Departments;

namespace Textile.Core.Managers.Commands.Departments
{
    public class SaveDepartmentCommand : IRequest<bool>
    {
        public DepartmentRequest DepartmentRequest;

        public SaveDepartmentCommand(DepartmentRequest departmentRequest)
        {
            DepartmentRequest = departmentRequest;
        }
    }

}
