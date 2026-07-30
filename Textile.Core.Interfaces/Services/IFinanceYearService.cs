using Textile.Core.Entities.Models.Requests.FinanceYears;
using Textile.Core.Entities.Models.Response.FinanceYears;

namespace Textile.Core.Interfaces.Services
{
    public interface IFinanceYearService
    {
        public Task<bool> AddFinanceYear(FinanceYearRequest financeYearRequest, Guid currentUserId, string currentUserName);
        public Task<bool> UpdateFinanceYear(FinanceYearRequest financeYearRequest, Guid currentUserId, string currentUserName);
        public Task<bool> ToggleFinanceYearStatus(int id, Guid currentUserId, string currentUserName);
       
        public Task<FinanceYearResponse> GetFinanceYearById(int id);
        public Task<IEnumerable<FinanceYearResponse>> GetFinanceYears();
        public Task<IEnumerable<FinanceYearResponse>> GetActiveFinanceYears();
    }
}
