using Textile.Core.Entities.Models.Requests.SaleVoucherPrintDetails;
using Textile.Core.Entities.Models.Response.SaleVoucherPrintDetails;

namespace Textile.Core.Interfaces.Services
{
    public interface ISaleVoucherPrintDetailService
    {
        Task<SaleVoucherPrintDetailResponse> GetAsync();
        Task<bool> SaveAsync(SaleVoucherPrintDetailRequest request, Guid currentUserId);
    }
}
