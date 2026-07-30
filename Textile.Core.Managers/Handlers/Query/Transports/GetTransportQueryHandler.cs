using MediatR;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Enums;
using Textile.Core.Entities.Models.Response;
using Textile.Core.Interfaces.Data;
using Textile.Core.Managers.Query.Transports;

namespace Textile.Core.Managers.Handlers.Query.Transports
{

    public class GetTransportQueryHandler : IRequestHandler<GetTransportQuery, TransportResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetTransportQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<TransportResponse> Handle(GetTransportQuery request, CancellationToken cancellationToken)
        {

            var _transportRepository = _unitOfWork.Repository<Transport, int>();

            var transport = await _transportRepository.GetSingleAsync(t => t.Id == request.Id, t => t.City.State);


            if (transport == null)
                throw new Exception("Transport not found");

            var response = new TransportResponse
            {
                Id = transport.Id,
                Name = transport.Name,
                CityId = transport.CityId,
                StateId = transport.City.State.Id,
                GstIn = transport.GstIn,
                RegistrationType = transport.RegistrationType,
                TransportType = (TransportTypeEnum)transport.TransportType,
                Address = transport.Address,
                PinCode = transport.Pincode,
                Mobile = transport.Mobile,
                Email = transport.Email,
                Remarks = transport.Remarks
            };

            return response;
        }


    }
}
