using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Models.Response.Tally;
using Textile.Core.Infrastructure.Context;
using Textile.Core.Interfaces.Data;
using Textile.Core.Managers.Query.Tally;

namespace Textile.Core.Managers.Handlers.Query.Tally
{
    public class GetTallyProcessQueryHandler : IRequestHandler<GetTallyProcessQuery, List<TallyProcessResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly TextileDbContext _TextileDbContext;
        public GetTallyProcessQueryHandler(
       IUnitOfWork unitOfWork,
       IMapper mapper, TextileDbContext textileDbContext)
        {
            _unitOfWork = unitOfWork ??
              throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ??
              throw new ArgumentNullException(nameof(mapper));
            _TextileDbContext = textileDbContext ??
              throw new ArgumentNullException(nameof(textileDbContext));

        }
        public async Task<List<TallyProcessResponse>> Handle(
    GetTallyProcessQuery request,
    CancellationToken cancellationToken)     
        {
            var repo = _unitOfWork.Repository<TallyProcessLogs, int>();

            var data = (await repo.GetAllAsync(x => x.CompanyId == request.CompanyId)).ToList();

            if (!data.Any())
                return new List<TallyProcessResponse>();

            return data.Select(x => new TallyProcessResponse
            {
                Id = x.Id,
                CompanyId = x.CompanyId,
                FinanceYearId = x.FinanceYearId,
                ReferenceNo = x.ReferenceNo,
                ProcessType = x.ProcessType,
                Step = x.Step,
                IsSuccess = x.IsSuccess,
                RequestData = x.RequestData,
                ResponseData = x.ResponseData,
                ErrorMessage = x.ErrorMessage,
                CreatedOn = x.CreatedOn
            }).ToList();

        }
    }
}
