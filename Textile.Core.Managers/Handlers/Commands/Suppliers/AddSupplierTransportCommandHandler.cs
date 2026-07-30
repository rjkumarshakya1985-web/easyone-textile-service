using MediatR;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Enums;
using Textile.Core.Interfaces.Data;
using Textile.Core.Managers.Commands.Suppliers;

namespace Textile.Core.Managers.Handlers.Commands.Suppliers
{
    public class AddSupplierTransportCommandHandler : IRequestHandler<AddSupplierTransportCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddSupplierTransportCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(AddSupplierTransportCommand command, CancellationToken cancellationToken)
        {
            var request = command.AddSupplierTransportRequest;

            var supplierTransportRepository = _unitOfWork.Repository<SupplierTransport, Guid>();
            var supplierRepository = _unitOfWork.Repository<Supplier, Guid>();
            var transportRepository = _unitOfWork.Repository<Transport, int>();

            // ---------------------------
            //  Validate Supplier
            // ---------------------------
            var supplier = await supplierRepository.GetSingleAsync(x => x.Id == request.SupplierId);
            if (supplier == null)
                throw new Exception("Supplier not found");

            // ---------------------------
            //  Validate Transport
            // ---------------------------
            var transport = await transportRepository.GetSingleAsync(x => x.Id == request.TransportId);
            if (transport == null)
                throw new Exception("Transport not found");
            if (!transport.IsActive || transport.IsDeleted ||
                transport.TransportType == (int)TransportTypeEnum.Sales)
                throw new Exception("Only Purchase or Both type transport can be assigned to a supplier.");

            // ---------------------------
            //  Check if mapping already exists
            // ---------------------------
            var existingMap = await supplierTransportRepository.GetSingleAsync(x =>
                x.SupplierId == request.SupplierId &&
                x.TransportId == request.TransportId);

            if (existingMap != null)
                throw new Exception("This transport is already mapped to the supplier");

            // ---------------------------
            //  Add new mapping
            // ---------------------------
            var newMapping = new SupplierTransport
            {
                SupplierId = request.SupplierId,
                TransportId = request.TransportId,
                IsActive = true
            };

            await supplierTransportRepository.AddAsync(newMapping);

            return true;
        }
    }

}
