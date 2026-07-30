
using Microsoft.EntityFrameworkCore;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Models.Requests.FinanceYears;
using Textile.Core.Entities.Models.Response.FinanceYears;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Interfaces.Data;
using Textile.Core.Interfaces.Services;

namespace Textile.Core.Managers.Services
{
    public class FinanceYearService : IFinanceYearService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly TextileDbContext _context;
        public FinanceYearService(IUnitOfWork unitOfWork,TextileDbContext context)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<bool> AddFinanceYear(FinanceYearRequest request, Guid currentUserId, string currentUserName)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var repository = _unitOfWork.Repository<FinanceYear, int>();

                // ✅ Only active FY ko close karo
                var activeFinanceYear = await _context.FinanceYears
                    .FirstOrDefaultAsync(x => x.IsActive && !x.IsClosed);

                if (activeFinanceYear != null)
                { 
                    activeFinanceYear.IsClosed = true;
                }

                // ✅ New Finance Year (always active)
                var entity = new FinanceYear
                {
                    Name = request.Name,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    IsActive = true,
                    IsClosed = false,
                    CreatedBy = currentUserId,
                    CreatedByUserName = currentUserName,
                    CreatedOn = DateTime.UtcNow // ✅ FIXED
                };

                await repository.AddAsync(entity);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> ToggleFinanceYearStatus(int id, Guid currentUserId, string currentUserName)
        {
            var repository = _unitOfWork.Repository<FinanceYear, int>();

            var financeYear = await repository.GetByIdAsync(id);

            if (financeYear == null)
            {
                throw new KeyNotFoundException($"Finance Year with ID {id} not found.");
            }

           
            financeYear.IsActive = !financeYear.IsActive;
            financeYear.CreatedBy = currentUserId;
            financeYear.CreatedByUserName = currentUserName;
            financeYear.ModifiedOn = DateTime.Now; 

            await repository.UpdateAsync(financeYear);
            await _context.SaveChangesAsync();

            return true;
        }
        public Task<FinanceYearResponse> GetFinanceYearById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<FinanceYearResponse>> GetFinanceYears()
        {
            var repository = _unitOfWork.Repository<FinanceYear, int>();
            var financeYears = await repository.GetAllAsync();
            return financeYears.Select(fy => new FinanceYearResponse
            {
                Id = fy.Id,
                Name = fy.Name,
                StartDate = fy.StartDate,
                EndDate = fy.EndDate,
                IsActive = fy.IsActive,
                IsClosed = fy.IsClosed
            }).OrderByDescending(x=>x.StartDate).ToList();

        }

        public Task<bool> UpdateFinanceYear(FinanceYearRequest financeYearRequest, Guid currentUserId, string currentUserName)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<FinanceYearResponse>> GetActiveFinanceYears()
        {
            var repository = _unitOfWork.Repository<FinanceYear, int>();
            var financeYears = await repository.GetAllAsync();
            return financeYears.Select(fy => new FinanceYearResponse
            {
                Id = fy.Id,
                Name = fy.Name,
                StartDate = fy.StartDate,
                EndDate = fy.EndDate,
                IsActive = fy.IsActive,
                IsClosed = fy.IsClosed
            }).OrderByDescending(x => x.StartDate).ToList();

        }
    }
}
