using MediatR;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Enums;
using Textile.Core.Entities.Models.Response;
using Textile.Core.Interfaces.Data;
using Textile.Core.Managers.Query.Suppliers;

namespace Textile.Core.Managers.Handlers.Query.Suppliers
{

    public class GetAllSupplierTransportsQueryHandler : IRequestHandler<GetAllSupplierTransportsQuery, IEnumerable<TransportResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllSupplierTransportsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<IEnumerable<TransportResponse>> Handle(
            GetAllSupplierTransportsQuery request,
            CancellationToken cancellationToken)
        {
            var repository = _unitOfWork.Repository<SupplierTransport, Guid>();

            var supplierTransports = await repository.GetAllAsync(
                x => x.SupplierId == request.SupplierId && x.IsActive,
                x => x.Transport,
                x => x.Transport.City,
                x => x.Transport.City.State
            );

            var result = supplierTransports.Select(x => new TransportResponse
            {
                Id = x.Transport.Id,
                Name = x.Transport.Name,

                CityId = x.Transport.CityId,
                City = x.Transport.City.Name,

                StateId = x.Transport.City.State.Id,
                State = x.Transport.City.State.Name,

                GstIn = x.Transport.GstIn,
                RegistrationType = x.Transport.RegistrationType,
                TransportType = (TransportTypeEnum)x.Transport.TransportType,

                Address = x.Transport.Address,
                PinCode = x.Transport.Pincode,
                Mobile = x.Transport.Mobile,
                Email = x.Transport.Email,
                Remarks = x.Transport.Remarks
            });

            return result;
        }

    }
}
