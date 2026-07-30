using MediatR;
using Textile.Core.Entities.Models.Requests;

namespace Textile.Core.Managers.Commands.Transports
{
    public class AddTransportCommand : IRequest<bool>
    {
        public TransportRequest TransportRequest { get; set; }

        public AddTransportCommand(TransportRequest transportRequest)
        {
            TransportRequest = transportRequest;
        }
    }
}
