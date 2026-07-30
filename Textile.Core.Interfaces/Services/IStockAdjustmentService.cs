using Textile.Core.Entities.Models.Requests.Stock;
using Textile.Core.Entities.Models.Response.Stocks;

namespace Textile.Core.Interfaces.Services
{
    public interface IStockAdjustmentService
    {
        Task<bool> AdjustStockAsync(StockAdjustmentRequest request,Guid userid,string username);

        Task<StockAdjustmentResponse> GetStockAdjustments(Guid stockId);
    }
}
