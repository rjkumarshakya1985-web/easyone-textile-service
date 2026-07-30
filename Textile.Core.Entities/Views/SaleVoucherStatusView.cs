namespace Textile.Core.Entities.Views
{
    public class SaleVoucherStatusView
    {
        public Guid  Id { get; set; }
        public int SaleVoucherId { get; set; }

        public DateTime Date { get; set; }
        public int Status { get; set; }
        public string? Reasons { get; set; }

        public Guid CreatedBy { get; set; }
        public string CreatedByUserName { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
