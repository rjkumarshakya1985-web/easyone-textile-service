using Textile.Core.Entities.Models.Response.Suppliers.Print;

namespace Textile.Core.Interfaces.Services
{
    public interface IPrintService
    {
        Task<StickerPrint> GetStickerByProduct(Guid id, bool isSaleVoucher = false);

        Task<StickerPrint> GetStickerBySaleVoucherDetail(Guid id);
    }
}
