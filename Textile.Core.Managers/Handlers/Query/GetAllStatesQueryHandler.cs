using MediatR;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Interfaces.Data;
using Textile.Core.Managers.Query;

namespace Textile.Core.Managers.Handlers.Query
{
    public class GetAllStatesQueryHandler : IRequestHandler<GetAllStatesQuery, IEnumerable<State>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllStatesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<IEnumerable<State>> Handle(GetAllStatesQuery request, CancellationToken cancellationToken)
        {
            var _stateRepository = _unitOfWork.Repository<State, int>();
            var states = await _stateRepository.GetAllAsync();
            return states;
        }
    }
}



