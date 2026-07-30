using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Models.Requests.Masters;
using Textile.Core.Entities.Models.Response.StockGroups;
using Textile.Core.Interfaces.Data;
using Textile.Core.Interfaces.Services;

namespace Textile.Core.Managers.Services
{
    public class StockGroupService : IStockGroupService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StockGroupService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<IEnumerable<StockGroupDto>> GetAllAsync()
        {
            var repository = _unitOfWork.Repository<StockGroup, int>();

            var stockGroups = await repository.GetAllAsync();

            return stockGroups.Select(x => new StockGroupDto
            {
                Id = x.Id,
                Name = x.Name,
                Description =x.Description,
                isGstRule =x.IsGstRule,
                GstValue =x.GstValue,
                IsActive = x.IsActive
            });
        }

        public async Task<StockGroupDto?> GetByIdAsync(int id)
        {
            var repository = _unitOfWork.Repository<StockGroup, int>();

            var stockGroup = await repository.GetByIdAsync(id);
            if (stockGroup == null)
                return null;

            return new StockGroupDto
            {
                Id = stockGroup.Id,
                Name = stockGroup.Name,
                IsActive = stockGroup.IsActive
            };
        }

        public async Task<bool> CreateAsync(StockGroupRequest request, Guid currentUserId, string currentUserName)
        {
            var repository = _unitOfWork.Repository<StockGroup, int>();

            var entity = new StockGroup
            {
                Name = request.Name,
                GstValue = request.GstValue,
                IsGstRule = request.IsGstRule,
                CreatedBy = currentUserId,
                CreatedByUserName = currentUserName,
                IsActive = true,
                IsDeleted = false,
                CreatedOn = DateTime.UtcNow
            };

            await repository.AddAsync(entity);
          
            return true;
        }

        public async Task<bool> UpdateAsync(StockGroupRequest request, Guid currentUserId, string currentUserName)
        {
            if (!request.Id.HasValue)
                return false;

            var repository = _unitOfWork.Repository<StockGroup, int>();

            var stockGroup = await repository.GetByIdAsync(request.Id.Value);
            if (stockGroup == null)
                return false;

          
            stockGroup.Name = request.Name;
            stockGroup.GstValue = request.GstValue;
            stockGroup.IsGstRule = request.IsGstRule;
            stockGroup.Description = request.Description;
            stockGroup.IsActive = request.IsActive;
            stockGroup.ModifiedBy = currentUserId;
            stockGroup.ModifiedByUserName = currentUserName;
            stockGroup.ModifiedOn = DateTime.UtcNow;

            await repository.UpdateAsync(stockGroup);

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var repository = _unitOfWork.Repository<StockGroup, int>();

            var stockGroup = await repository.GetByIdAsync(id);
            if (stockGroup == null)
                return false;

            // Soft delete
            stockGroup.IsDeleted = true;

            await  repository.UpdateAsync(stockGroup);
     
            return true;
        }

        public async Task<bool> ToggleActiveAsync(int id)
        {
            var repository = _unitOfWork.Repository<StockGroup, int>();

            var stockGroup = await repository.GetByIdAsync(id);
            if (stockGroup == null)
                return false;

            stockGroup.IsActive = !stockGroup.IsActive;

            await repository.UpdateAsync(stockGroup);
           
            return true;
        }

    }

}
