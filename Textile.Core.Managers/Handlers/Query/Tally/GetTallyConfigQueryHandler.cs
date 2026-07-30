using MediatR;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Models.Response.Tally;
using Textile.Core.Interfaces.Data;
using Textile.Core.Managers.Query.Tally;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Textile.Core.Managers.Handlers.Query.Tally
{
    public class GetTallyConfigQueryHandler
        : IRequestHandler<GetTallyConfigQuery, TallyConfigResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetTallyConfigQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<TallyConfigResponse> Handle(
            GetTallyConfigQuery request,
            CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.Repository<TallyConfigs, int>();
            var companyRepo = _unitOfWork.Repository<TallyCompanies, int>();

            var data = (await repo.GetAllAsync(x => x.CompanyId == request.CompanyId)).ToList();

            // ✅ Get Company
            var company = await companyRepo.GetSingleAsync(x => x.Id == request.CompanyId);

            if (!data.Any())
                return new TallyConfigResponse();
            return new TallyConfigResponse
            {
                CompanyId = request.CompanyId,

                // ✅ MAP COMPANY HERE
                Company = company == null ? null : new CompanyDto
                {
                    Name = company.Name,
                    StateId = company.StateId,
                    GSTIN = company.GSTIN,
                    StateName=company.StateName,
                    GSTRegistrationType=company.GSTRegistrationType,
                    Consignee=company.Consignee,
                    ConsigneeAddress=company.ConsigneeAddress,
                    PINCode=company.PINCode,
                    Email=company.Email,
                    IsActive=company.IsActive,
                    CreatedOn=company.CreatedOn ?? DateTime.MinValue
                },

                Purchase = MapGroup(data, "PURCHASE"),
                Sale = MapGroup(data, "SALE")
            };

            
        }

        private LedgerGroupDto MapGroup(
            List<TallyConfigs> data,
            string transactionType)
        {
            var group = data
                .Where(x => x.TransactionType == transactionType)
                .ToList();

            return new LedgerGroupDto
            {
                MainLedger = group
                    .FirstOrDefault(x => x.TaxType == "MAIN")?.LedgerName,

                CGST = group
                    .FirstOrDefault(x => x.TaxType == "CGST")?.LedgerName,

                SGST = group
                    .FirstOrDefault(x => x.TaxType == "SGST")?.LedgerName,

                IGST = group
                    .FirstOrDefault(x => x.TaxType == "IGST")?.LedgerName
            };
        }
    }
}
