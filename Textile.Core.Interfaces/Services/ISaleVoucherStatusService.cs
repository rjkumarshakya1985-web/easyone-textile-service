using Textile.Core.Entities.Views;

namespace Textile.Core.Interfaces.Services
{
    public interface ISaleVoucherStatusService
    {
        Task<List<SaleVoucherStatusView>> GetAll(int salevoucherId);
        Task<bool> AddAsync(SaleVoucherStatusView saleVoucherStatus);
    }
}
