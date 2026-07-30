using Textile.Core.Entities.Views;

namespace Textile.Core.Entities.Models.Response.Parcels
{
    public class ParcelResponse
    {
        public ParcelView? SaleVoucher { get; set; }
        public bool IsAvailable { get; set; }

        public string? Message { get; set; }
    }
}
