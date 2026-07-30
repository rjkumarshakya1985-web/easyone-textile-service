namespace Textile.Core.Entities.Models.Response.Dashboard
{
    public class DashboardParcel
    {
        public int SaleVoucherId { get; set; }
        public DateTime Date { get; set; }
        public string TransportName { get; set; }
        public int ProductQuantity { get; set; }

        public int Status { get; set; }
    }
}
