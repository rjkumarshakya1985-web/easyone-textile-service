using Textile.Core.Entities.Enums;
using Textile.Core.Entities.Models.Requests;
using Textile.Core.Entities.Models.Response.Parcels;

namespace Textile.Core.Interfaces.Services
{
    public interface IParcelService
    {
        Task<ParcelResponse> GetParcelScanInfoAsync(int parcelId, ParcelStatusEnum parcelStatusEnum);
        Task<bool> ChangeSaleVouchersStatus(ParcelScanRequest parcelScanRequest,Guid createdBy,string currentUser);
        Task<bool> MoveSaleVoucherProductsToStockAsync(ParcelScanRequest parcelScanRequest, Guid createdBy, string currentUser);

    }
    
}
