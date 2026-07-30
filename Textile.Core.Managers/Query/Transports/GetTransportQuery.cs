using MediatR;
using Textile.Core.Entities.Models.Response;

namespace Textile.Core.Managers.Query.Transports
{
    public class GetTransportQuery : IRequest<TransportResponse>
    {
        public int Id { get; }

        public GetTransportQuery(int id)
        {
            Id = id;
        }
    }
}
