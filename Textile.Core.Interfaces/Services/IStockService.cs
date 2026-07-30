using Textile.Core.Entities.Data;
using Textile.Core.Entities.Models.Requests;
using Textile.Core.Entities.Models.Response.Stocks;
using Textile.Core.Entities.Views;

namespace Textile.Core.Interfaces.Services
{
    public interface IStockService
    {
        Task<TableResult<StockTableResponse>> GetTableData(
     TableDataRequest dataRequest);

        Task<TableResult<StockLedgerViews>> GetStockLedgerTableData(
       TableDataRequest dataRequest);

        Task<IEnumerable<CurrentStockView>> GetStockItemsByBarcode(string barcode);

        Task<StockTableResponse> GetStockByIdAsync(
            Guid id);

    }
}
