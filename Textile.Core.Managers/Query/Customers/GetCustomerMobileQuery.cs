using MediatR;
using Textile.Core.Entities.Models.Response.Customers;

namespace Textile.Core.Managers.Query.Customers
{
    public class GetCustomerMobileQuery : IRequest<CustomerResponse>
    {
        public string Mobile { get; set; }

        public GetCustomerMobileQuery(string mobile)
        {
            Mobile = mobile;
        }
    }
}
