
using Textile.Core.Entities.Data;
using Textile.Core.Entities.Models.Requests;
using Textile.Core.Entities.Models.Response.Billing.Invoices;
using Textile.Core.Entities.Models.Response.Invoices;
using Textile.Core.Entities.Views;

namespace Textile.Core.Interfaces.Services.Sales
{
    public interface IInvoiceService
    {
        Task<TableResult<InvoiceListResponse>> GetTableData(TableDataRequest tableDataRequest, int finYearId);
        Task<InvoiceResponse?> GetInvoice(string number, int finYearId);
        Task<InvoiceResponse?> GetInvoice(int id);

        Task<List<StatusCountView>> GetInvoiceStatusCountsAsync(int financialYearId);
    }
}
