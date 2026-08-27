namespace Textile.Core.Entities.Models.Requests.SaleVouchers
{
    public class ChangeSaleVoucherStatusRequest
    {
        public int SaleVoucherId { get; set; }
        public int Status { get; set; }
        public string? Reason { get; set; }
    }
}
