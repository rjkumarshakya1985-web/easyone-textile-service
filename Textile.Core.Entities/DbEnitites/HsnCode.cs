namespace Textile.Core.Entities.DbEnitites
{
    public class ProductHsnCode : BaseAuditDbEntity<Guid>
    {
        public string Name { get; set; }
        public string? Description { get; set; }

        public bool IsDeleted { get; set; }

        public ICollection<SupplierHsnCode> SupplierHsnCodes { get; set; }
    }
}
