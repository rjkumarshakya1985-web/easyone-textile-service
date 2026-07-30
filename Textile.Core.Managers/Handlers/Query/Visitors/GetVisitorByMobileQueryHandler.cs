using MediatR;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Models.Response.Visitors;
using Textile.Core.Interfaces.Data;
using Textile.Core.Managers.Query.Visitors;


namespace Textile.Core.Managers.Handlers.Query.Visitors
{
    public class GetVisitorByMobileQueryHandler
     : IRequestHandler<GetVisitorByMobileQuery, VisitorResponse?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetVisitorByMobileQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<VisitorResponse?> Handle(GetVisitorByMobileQuery request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.Repository<Visitor, int>();

            var visitor = await repo.GetSingleAsync(x => x.Mobile == request.Mobile);

            if (visitor == null)
                return null;

            return new VisitorResponse
            {
                Id = visitor.Id,
                Name = visitor.Name,
                Mobile = visitor.Mobile,
                CustomerType = visitor.CustomerType,
                VisitDate = visitor.VisitDate,
                Remarks = visitor.Remarks,
                CityId = visitor.CityId,
                CreatedBy = visitor.CreatedBy,
                CreatedByUserName = visitor.CreatedByUserName,
                CreatedOn = visitor.CreatedOn,
                ModifiedBy = visitor.ModifiedBy,
                ModifiedByUserName = visitor.ModifiedByUserName,
                ModifiedOn = visitor.ModifiedOn
            };
        }
    }
}
