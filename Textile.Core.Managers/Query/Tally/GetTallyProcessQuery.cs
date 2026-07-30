using MediatR;
using Textile.Core.Entities.Models.Response.Tally;

namespace Textile.Core.Managers.Query.Tally
{
    public class GetTallyProcessQuery : IRequest<List<TallyProcessResponse>>
    {
        public int CompanyId { get; set; }
    }
}
