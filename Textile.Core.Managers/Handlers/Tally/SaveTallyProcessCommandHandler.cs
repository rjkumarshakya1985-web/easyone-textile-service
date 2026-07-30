using MediatR;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Interfaces.Data;
using Textile.Core.Managers.Commands.Tally;

namespace Textile.Core.Managers.Handlers.Tally
{
    public class SaveTallyProcessCommandHandler
   : IRequestHandler<SaveTallyProcessCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public SaveTallyProcessCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(SaveTallyProcessCommand request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.Repository<TallyProcessLogs, int>();

            var entities = request.TallyProcessRequests.Select(item => new TallyProcessLogs
            {
                CompanyId = item.CompanyId,
                FinanceYearId=item.FinanceYearId,
                ReferenceNo = item.ReferenceNo,
                ProcessType = item.ProcessType,
                Step = item.Step,
                IsSuccess = item.IsSuccess,
                RequestData = item.RequestData,
                ResponseData = item.ResponseData,
                ErrorMessage = item.ErrorMessage
            }).ToList();
           
            foreach (var entity in entities)
            {
                await repo.AddAsync(entity);
            }

            await _unitOfWork.SaveChangesAsync();

            return true;

        }

    }
}
