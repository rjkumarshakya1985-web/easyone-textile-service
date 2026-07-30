using MediatR;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Interfaces.Data;
using Textile.Core.Managers.Commands.Transports;

namespace Textile.Core.Managers.Handlers.Commands.Transports
{
    public class AddTransportCommandHandler : IRequestHandler<AddTransportCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddTransportCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(AddTransportCommand command, CancellationToken cancellationToken)
        {
            var request = command.TransportRequest;
            var transportRepository = _unitOfWork.Repository<Transport, int>();
            var cityRepository = _unitOfWork.Repository<City, int>();

            // Validate city
            var city = await cityRepository.GetSingleAsync(x => x.Id == request.CityId);
            if (city == null)
                throw new Exception("City not found");

            if (request.Id == 0)
            {
                // Create new transport
                var newTransport = new Transport
                {
                    Name = request.Name,
                    CityId = request.CityId,
                    GstIn = request.GstIn,
                    RegistrationType = request.RegistrationType,
                    TransportType = request.TransportType,
                    Address = request.Address,
                    Pincode = request.Pincode,
                    Mobile = request.Mobile,
                    Email = request.Email,
                    Remarks = request.Remarks,
                    IsActive = true,
                    IsDeleted = false
                };

                await transportRepository.AddAsync(newTransport);
            }
            else
            {
                // Update existing transport
                var existingTransport = await transportRepository.GetSingleAsync(x => x.Id == request.Id);
                if (existingTransport == null)
                    throw new Exception("Transport not found");

                // Map properties
                existingTransport.Name = request.Name;
                existingTransport.CityId = request.CityId;
                existingTransport.GstIn = request.GstIn;
                existingTransport.RegistrationType = request.RegistrationType;
                existingTransport.TransportType = request.TransportType;
                existingTransport.Address = request.Address;
                existingTransport.Pincode = request.Pincode;
                existingTransport.Mobile = request.Mobile;
                existingTransport.Email = request.Email;
                existingTransport.Remarks = request.Remarks;

                await transportRepository.UpdateAsync(existingTransport);
            }

            return true;
        }

    }
}
