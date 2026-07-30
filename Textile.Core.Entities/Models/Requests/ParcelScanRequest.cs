using Textile.Core.Entities.Enums;

namespace Textile.Core.Entities.Models.Requests
{
    public class ParcelScanRequest
    {
        public List<int> SaleVoucherId { get; set; }

        public ParcelStatusEnum StatusEnum { get; set; }
    }
}
