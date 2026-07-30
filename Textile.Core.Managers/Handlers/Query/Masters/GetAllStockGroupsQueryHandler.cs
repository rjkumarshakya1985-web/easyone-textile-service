using AutoMapper;
using MediatR;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Models.Response.Masters;
using Textile.Core.Interfaces.Data;

namespace Textile.Core.Managers.Handlers.Query.Masters
{
    public class GetAllStockGroupsQuery : IRequest<List<StockGroupResponse>>
    {
    }

    public class GetAllStockGroupsQueryHandler : IRequestHandler<GetAllStockGroupsQuery, List<StockGroupResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetAllStockGroupsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<StockGroupResponse>> Handle(GetAllStockGroupsQuery request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.Repository<StockGroup, int>();

         
            var groups = await repo.GetAllAsync(x => x.IsDeleted == false);

          
            var stockGroupDto = _mapper.Map<List<StockGroupResponse>>(groups);

            return stockGroupDto
                .OrderByDescending(x => x.Name)
                .ToList();
        }
    }

}
