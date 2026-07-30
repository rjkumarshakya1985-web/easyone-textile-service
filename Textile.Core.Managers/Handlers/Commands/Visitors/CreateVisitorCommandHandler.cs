using MediatR;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Interfaces.Data;
using Textile.Core.Managers.Commands.Visitors;

namespace Textile.Core.Managers.Handlers.Commands.Visitors
{
    public class CreateVisitorCommandHandler
   : IRequestHandler<CreateVisitorCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateVisitorCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> Handle(CreateVisitorCommand request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.Repository<Visitor, int>();

            Visitor? visitor;

            // ----- UPDATE CASE -----
            if (request.VisitorRequest.Id != null)
            {
                visitor = await repo.GetByIdAsync(request.VisitorRequest.Id.Value);

                if (visitor == null)
                    return 0;

                visitor.Name = request.VisitorRequest.Name;
                visitor.CustomerId = request.VisitorRequest.CustomerId;
                visitor.Mobile = request.VisitorRequest.Mobile;
                visitor.CustomerType = request.VisitorRequest.CustomerType;
                visitor.Remarks = request.VisitorRequest.Remarks?.Trim();
                visitor.CityId = request.VisitorRequest.CityId;
                visitor.ModifiedOn = DateTime.UtcNow;
                visitor.ModifiedBy = request.UserId;
                visitor.ModifiedByUserName = request.UserName;

                await repo.UpdateAsync(visitor);
            }
            else
            {
                visitor = new Visitor
                {
                    Name = request.VisitorRequest.Name,
                    Mobile = request.VisitorRequest.Mobile,
                    CustomerId = request.VisitorRequest.CustomerId,
                    CustomerType = request.VisitorRequest.CustomerType,
                    Remarks = request.VisitorRequest.Remarks?.Trim(),
                    VisitDate = DateTime.UtcNow,
                    CityId = request.VisitorRequest.CityId,
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = request.UserId,
                    CreatedByUserName = request.UserName,
                };

                await repo.AddAsync(visitor);
            }

            return visitor.Id;   // ✔ correct
        }
    }
}
