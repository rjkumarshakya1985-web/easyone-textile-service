using Microsoft.EntityFrameworkCore;
using Textile.Core.Entities.Data;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Models.Requests;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Interfaces.Data;
using Textile.Core.Interfaces.Services;

namespace Textile.Core.Managers.Services
{
    public class HsnCodeService : IHsnCodeService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly TextileDbContext _context;
        public HsnCodeService(IUnitOfWork unitOfWork, TextileDbContext context)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> CreateAsync(HsnCodeRequest request, Guid currentUserId, string currentUserName)
        {
            var repository = _unitOfWork.Repository<ProductHsnCode, Guid>();

            var entity = new ProductHsnCode
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                CreatedBy = currentUserId,
                CreatedByUserName = currentUserName,
                CreatedOn = DateTime.UtcNow
            };

            await repository.AddAsync(entity);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var repository = _unitOfWork.Repository<ProductHsnCode, Guid>();
            var product = await repository.GetByIdAsync(id);
            product.IsDeleted = true;
            await repository.UpdateAsync(product);
            return true;
        }

        public Task<IEnumerable<ProductHsnCode>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ProductHsnCode?> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<TableResult<ProductHsnCode>> GetTableData(TableDataRequest req)
        {
            var query = _context.HsnCodes.AsNoTracking().Where(x => !x.IsDeleted);

            // 🔍 SEARCH
            if (!string.IsNullOrWhiteSpace(req.Search))
            {
                var s = req.Search.Trim().ToLower();

                query = query.Where(x =>
                    x.Name.ToLower().Contains(s));
            }

            int total = await query.CountAsync();

            var data = await query
                .OrderBy(x => x.Name)
                .Skip(req.PageIndex * req.PageSize)
                .Take(req.PageSize)
                .ToListAsync();

            return new TableResult<ProductHsnCode>
            {
                TotalRows = total,
                Result = data
            };
        }

        public Task<bool> UpdateAsync(HsnCodeRequest request, Guid currentUserId, string currentUserName)
        {
            throw new NotImplementedException();
        }
    }
}
