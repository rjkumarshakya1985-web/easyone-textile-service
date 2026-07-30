namespace Textile.Core.Entities.DbEnitites
{
    public class SaleVoucherStatus : BaseAuditDbEntity<Guid>
    {
        public int SaleVoucherId { get; set; }

        public DateTime Date { get; set; }
        public int Status { get; set; }
        public string? Reasons { get; set; }

        public SaleVoucher SaleVoucher { get; set; }
    }
}
