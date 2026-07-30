using AutoMapper;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Views;
using Textile.Core.Interfaces.Data;
using Textile.Core.Interfaces.Services;

namespace Textile.Core.Managers.Services
{
    public class SaleVoucherStatusService : ISaleVoucherStatusService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SaleVoucherStatusService(IUnitOfWork unitOfWork, IMapper mapper)
        {

            this._unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<bool> AddAsync(SaleVoucherStatusView saleVoucherStatus)
        {
            var repository = _unitOfWork.Repository<SaleVoucherStatus, Guid>();

            // DTO → Entity
            var entity = _mapper.Map<SaleVoucherStatus>(saleVoucherStatus);

            await repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<List<SaleVoucherStatusView>> GetAll(int saleVoucherId)
        {
            var repository = _unitOfWork.Repository<SaleVoucherStatus, Guid>();

            var entities = await repository.GetAllAsync(
                x => x.SaleVoucherId == saleVoucherId
            );
            return _mapper.Map<List<SaleVoucherStatusView>>(entities);
        }
    }
}
