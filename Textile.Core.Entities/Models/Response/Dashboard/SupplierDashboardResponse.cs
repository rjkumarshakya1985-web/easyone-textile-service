namespace Textile.Core.Entities.Models.Response.Dashboard
{
    public class SupplierDashboardResponse
    {
        public int ProductCount { get; set; }
        public int InTransitParcelCount { get; set; }
        public int TransportParcelCount { get; set; }
        public int SaleVoucherCount { get; set; }

        public List<DashboardParcel> LatestSaleVouchers { get; set; }
      
    }

   
}
