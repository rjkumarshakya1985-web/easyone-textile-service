using MediatR;
using Textile.Core.Entities.Models.Requests.Suppliers;

namespace Textile.Core.Managers.Commands.Suppliers
{
    public class CreateSupplierCommand : IRequest<Guid>
    {
        public Guid CreatedBy { get; }
        public string CreatedByUserName { get; }
        public SupplierRequest SupplierRequest { get;}

        public CreateSupplierCommand(SupplierRequest supplierRequest,Guid createBy,string createdByUserName)
        {
            CreatedBy = createBy;
            CreatedByUserName = createdByUserName;
            SupplierRequest = supplierRequest;
        }
    }
}
