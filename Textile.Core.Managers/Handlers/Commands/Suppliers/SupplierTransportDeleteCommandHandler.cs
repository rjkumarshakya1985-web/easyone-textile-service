using MediatR;
using Microsoft.EntityFrameworkCore;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Enums;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Interfaces.Data;
using Textile.Core.Managers.Commands.Suppliers;

namespace Textile.Core.Managers.Handlers.Commands.Suppliers
{
    public class SupplierTransportDeleteCommandHandler
     : IRequestHandler<SupplierTransportDeleteCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly TextileDbContext _context;

        public SupplierTransportDeleteCommandHandler(IUnitOfWork unitOfWork, TextileDbContext context)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> Handle(
            SupplierTransportDeleteCommand command,
            CancellationToken cancellationToken)
        {
            var request = command.SupplierTransportDeleteRequest;


            bool hasInTransitVoucher = await _context.SaleVouchers.AsNoTracking().AnyAsync(x =>
                 x.SupplierId == request.SupplierId &&
                 x.TransportId == request.TransportId && !x.IsDeleted && x.Status == (int)ParcelStatusEnum.InTransit,cancellationToken);

            if (hasInTransitVoucher)
            {
                throw new Exception(
                    "Transport cannot be removed while sale vouchers are in transit for this supplier."
                );
            }

            var repo = _unitOfWork.Repository<SupplierTransport, Guid>();

            

            // Check if mapping exists
            var mapping = await repo.GetSingleAsync(x =>
                x.SupplierId == request.SupplierId &&
                x.TransportId == request.TransportId);

            if (mapping == null)
                throw new Exception("Supplier transport mapping not found.");

            // Delete the mapping row
            await repo.DeleteAsync(mapping);

            return true;
        }
    }

}
