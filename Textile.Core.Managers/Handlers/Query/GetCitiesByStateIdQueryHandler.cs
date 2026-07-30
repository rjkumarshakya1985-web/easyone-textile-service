using MediatR;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Interfaces.Data;
using Textile.Core.Managers.Query;

namespace Textile.Core.Managers.Handlers.Query
{
    public class GetCitiesByStateIdQueryHandler : IRequestHandler<GetCitiesByStateIdQuery, IEnumerable<City>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCitiesByStateIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<IEnumerable<City>> Handle(GetCitiesByStateIdQuery request, CancellationToken cancellationToken)
        {
            var cityRepository = _unitOfWork.Repository<City, int>();

            var cities = await cityRepository.GetAllAsync(
                 c => c.StateId == request.StateId
            );

            return cities;
        }
    }
}
