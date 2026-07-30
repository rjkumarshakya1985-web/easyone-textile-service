using MediatR;
using Textile.Core.Entities.Models.Response.Visitors;

namespace Textile.Core.Managers.Query.Visitors
{
    public class GetVisitorByMobileQuery : IRequest<VisitorResponse>
    {
        public string Mobile { get; set; }
        public GetVisitorByMobileQuery(string mobile)
        {
            Mobile = mobile;
        }
    }
}
