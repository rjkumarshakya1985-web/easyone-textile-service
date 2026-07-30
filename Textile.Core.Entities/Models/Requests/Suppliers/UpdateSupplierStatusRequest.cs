using Textile.Core.Entities.Enums;

namespace Textile.Core.Entities.Models.Requests.Suppliers
{
    public class UpdateSupplierStatusRequest
    {
        public Guid SupplierId { get; set; }
        public SupplierStatusActionType ActionType { get; set; }
    }
}
