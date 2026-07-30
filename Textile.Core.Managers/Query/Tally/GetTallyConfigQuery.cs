using MediatR;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Models.Response.Tally;

namespace Textile.Core.Managers.Query.Tally
{   
    public class GetTallyConfigQuery : IRequest<TallyConfigResponse>
    {
        public int CompanyId { get; set; }
    }
}
