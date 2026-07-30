namespace Textile.Core.Entities.Models.Response.Dashboard
{
    public class AdminDashboardResponse
    {
        public int SupplierCount { get; set; }
        public int CustomerCount { get; set; }
        public int InParcel { get; set; }
        public int OpenParcel { get; set; }

        public int Transport { get; set; }

        public List<DashboardParcel> InTransitLatestSaleVouchers { get; set; }
        public List<DashboardParcel> InHouseLatestSaleVouchers { get; set; }
    }
}
