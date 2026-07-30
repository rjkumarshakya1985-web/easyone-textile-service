using MediatR;
using Textile.Core.Entities.Models.Requests.Visitors;

namespace Textile.Core.Managers.Commands.Visitors
{
    public class CreateVisitorCommand : IRequest<int>
    {
        public VisitorRequest VisitorRequest;
        public Guid UserId { get; set; }
        public string UserName { get; set; }

        public CreateVisitorCommand(VisitorRequest visitorRequest, Guid userId, string userName)
        {
            VisitorRequest = visitorRequest;
            UserId = userId;
            UserName = userName;
        }
    }
}
