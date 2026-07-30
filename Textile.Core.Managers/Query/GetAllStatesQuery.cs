using MediatR;
using Textile.Core.Entities.DbEnitites;

namespace Textile.Core.Managers.Query
{
    public class GetAllStatesQuery : IRequest<IEnumerable<State>>
    {
    }
}
