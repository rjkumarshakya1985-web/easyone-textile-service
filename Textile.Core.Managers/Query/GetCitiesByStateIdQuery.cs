using MediatR;
using Textile.Core.Entities.DbEnitites;

namespace Textile.Core.Managers.Query
{
    public class GetCitiesByStateIdQuery : IRequest<IEnumerable<City>>
    {
        public int StateId { get; }

        public GetCitiesByStateIdQuery(int stateId)
        {
            StateId = stateId;
        }
    }
}
