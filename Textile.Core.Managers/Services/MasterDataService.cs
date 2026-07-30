using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Dto;
using Textile.Core.Entities.Enums;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Interfaces.Data;
using Textile.Core.Interfaces.Services;

namespace Textile.Core.Managers.Services
{
    public class MasterDataService : IMasterDataService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly TextileDbContext _context;
        public MasterDataService(IUnitOfWork unitOfWork, TextileDbContext context)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Gst>> GetGsts()
        {
            var repository = _unitOfWork.Repository<Gst, int>();

            return await repository.GetAllAsync();


        }

        public async Task<List<LookupDto<int>>> GetTransportLookUp(int? transportType = null)
        {
            var repository = _unitOfWork.Repository<Transport, int>();

            var response = await repository.GetAllAsync();
            var transports = response.Where(t => t.IsActive && !t.IsDeleted);

            if (transportType.HasValue)
            {
                transports = transports.Where(t => t.TransportType == transportType.Value ||
                    t.TransportType == (int)TransportTypeEnum.Both);
            }

            return transports
                .OrderBy(t => t.Name)   // 👈 Order here
                .Select(t => new LookupDto<int>
                {
                    Id = t.Id,
                    Name = t.Name
                })
                .ToList();
        }

        public async Task<List<LookupDto<Guid>>> GetHsnCodeLookUp()
        {
            var repository = _unitOfWork.Repository<ProductHsnCode, Guid>();

            var response = await repository.GetAllAsync();

            return response
                .OrderBy(t => t.Name)   // 👈 Order here
                .Select(t => new LookupDto<Guid>
                {
                    Id = t.Id,
                    Name = t.Name
                })
                .ToList();
        }

    }
}
