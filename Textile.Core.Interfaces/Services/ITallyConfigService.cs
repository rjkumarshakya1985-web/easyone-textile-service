using Textile.Core.Entities.Data;
using Textile.Core.Entities.Models.Requests;
using Textile.Core.Entities.Models.Response.Tally;

namespace Textile.Core.Interfaces.Services
{
    public interface ITallyConfigService
    {     
        public Task<IEnumerable<TallyCompanyResponse>> GetAllCompanies();
    }
}
