using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Models.Requests.StockGroups;
using Textile.Core.Entities.Models.Response.StockGroups;
using Textile.Core.Interfaces.Data;
using Textile.Core.Interfaces.Services;

namespace Textile.Core.Managers.Services
{
    public class GstRuleService : IGstRuleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public GstRuleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        // -----------------------------
        // CREATE
        // -----------------------------
        public async Task<bool> CreateAsync(
            GstRuleRequest request,
            Guid currentUserId,
            string currentUserName)
        {
            var repository = _unitOfWork.Repository<GstRule, int>();
            var entity = new GstRule
            {
                StockGroupId = request.StockGroupId,
                GstValue = request.GstValue,
                StartRange = request.StartRange,
                EndRange = request.EndRange,

                IsDeleted = false,

                CreatedBy = currentUserId,
                CreatedByUserName = currentUserName,
                CreatedOn = DateTime.UtcNow
            };

            await repository.AddAsync(entity);


            return true;
        }

        // -----------------------------
        // UPDATE
        // -----------------------------
        public async Task<bool> UpdateAsync(
            GstRuleRequest request,
            Guid currentUserId,
            string currentUserName)
        {

            var repository = _unitOfWork.Repository<GstRule, int>();
            var entity = await repository.GetSingleAsync(x => x.Id == request.Id);


            if (entity == null || entity.IsDeleted)
                return false;

            entity.StockGroupId = request.StockGroupId;
            entity.GstValue = request.GstValue;
            entity.StartRange = request.StartRange;
            entity.EndRange = request.EndRange;

            entity.ModifiedBy = currentUserId;
            entity.ModifiedByUserName = currentUserName;
            entity.ModifiedOn = DateTime.UtcNow;

            await repository.UpdateAsync(entity);

            return true;
        }

        // -----------------------------
        // DELETE (SOFT DELETE)
        // -----------------------------
        public async Task<bool> DeleteAsync(int id)
        {
            var repository = _unitOfWork.Repository<GstRule, int>();
            var entity = await repository.GetSingleAsync(x => x.Id == id);


            if (entity == null)
                return false;

            entity.IsDeleted = true;
            entity.ModifiedOn = DateTime.UtcNow;

            await repository.UpdateAsync(entity);

            return true;
        }

        // -----------------------------
        // GET ALL
        // -----------------------------
        public async Task<IEnumerable<GstRuleDto>> GetAllAsync()
        {
            var repository = _unitOfWork.Repository<GstRule, int>();

           
            var list = await repository.GetAllAsync
                (x => !x.IsDeleted,x=>x.StockGroup);

            return list.Select(x => new GstRuleDto
            {
                Id = x.Id,
                StockGroupId = x.StockGroupId,
                StockGroupName = x.StockGroup.Name,
                GstValue = x.GstValue,
                ApplyOrder =x.ApplyOrder,
                StartRange = x.StartRange,
                EndRange = x.EndRange
            });
        }

        // -----------------------------
        // GET BY ID
        // -----------------------------
        public async Task<GstRuleDto?> GetByIdAsync(int id)
        {
            var repository = _unitOfWork.Repository<GstRule, int>();

            var entity = await repository.GetByIdAsync(id, x => !x.IsDeleted);

            if (entity == null)
                return null;

            return new GstRuleDto
            {
                Id = entity.Id,
                StockGroupId = entity.StockGroupId,
                GstValue = entity.GstValue,
                StartRange = entity.StartRange,
                EndRange = entity.EndRange,
                ApplyOrder = entity.ApplyOrder
            };
        }
    }

}
