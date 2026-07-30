using Textile.Core.Entities.Data;
using Textile.Core.Entities.Models.Requests;
using Textile.Core.Entities.Models.Requests.Customers;
using Textile.Core.Entities.Models.Response.Billing;
using Textile.Core.Entities.Models.Response.Customers;

namespace Textile.Core.Interfaces.Services
{

    public interface ICustomerService
    {
        Task<Guid> CreateAsync(CustomerRequest request, Guid currentUserId, string currentUserName);

        Task<bool> UpdateAsync(CustomerRequest request, Guid currentUserId, string currentUserName);

        Task<bool> DeleteAsync(Guid id);

        Task<CustomerResponse?> GetByIdAsync(Guid id);

        Task<TableResult<CustomerResponse>> GetTableData(TableDataRequest req);


        /// <summary>
        /// This method used for search billing screen
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        Task<List<BillingCustomerResponse>> GetBillingCustomers();
    }

}
