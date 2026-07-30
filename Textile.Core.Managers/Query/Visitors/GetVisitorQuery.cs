using MediatR;
using Textile.Core.Entities.Models.Response.Visitors;

namespace Textile.Core.Managers.Query.Visitors
{
    public class GetVisitorQuery : IRequest<VisitorResponse?>
    {
        public int Id { get; set; }

        public GetVisitorQuery(int id)
        {
            Id = id;
        }
    }
}
