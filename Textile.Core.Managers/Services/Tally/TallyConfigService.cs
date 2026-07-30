using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Models.Response.Tally;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Interfaces.Data;
using Textile.Core.Interfaces.Services;

namespace Textile.Core.Managers.Services.Tally
{
    public class TallyConfigService:ITallyConfigService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly TextileDbContext _context;
        public TallyConfigService(IUnitOfWork unitOfWork, TextileDbContext context)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<IEnumerable<TallyCompanyResponse>> GetAllCompanies()
        {
            var repository = _unitOfWork.Repository<TallyCompanies, int>();
            var companies = await repository.GetAllAsync();
            return companies.Select(comp => new TallyCompanyResponse
            {
                Id = comp.Id,
                Name = comp.Name,
                StateId=comp.StateId,
                GSTIN=comp.GSTIN,
                StateName=comp.StateName,
                GSTRegistrationType=comp.GSTRegistrationType,
                Consignee=comp.Consignee,
                ConsigneeAddress=comp.ConsigneeAddress,
                PINCode=comp.PINCode,
                Email=comp.Email,
                IsActive=comp.IsActive,
                CreatedOn=comp.CreatedOn                
            }).OrderByDescending(x => x.IsActive).ToList();

        }
    }
}
