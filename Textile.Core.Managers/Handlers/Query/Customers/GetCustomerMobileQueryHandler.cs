using MediatR;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Models.Response.Customers;
using Textile.Core.Interfaces.Data;
using Textile.Core.Managers.Query.Customers;

namespace Textile.Core.Managers.Handlers.Query.Customers
{
    public class GetCustomerMobileQueryHandler : IRequestHandler<GetCustomerMobileQuery, CustomerResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCustomerMobileQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }       
        public async Task<CustomerResponse> Handle(GetCustomerMobileQuery request, CancellationToken cancellationToken)
        {
            var customerRepository = _unitOfWork.Repository<Customer, Guid>();

            var customer = await customerRepository
                .GetSingleAsync(x => x.Mobile == request.Mobile || x.Phone == request.Mobile,x=>x.City.State);

            if (customer == null)
                return null;

            return new CustomerResponse
            {
                Id = customer.Id,
                Name = customer.Name,
                Mobile = customer.Mobile,
                Phone = customer.Phone,
                CustomerType = customer.CustomerType,
                StateId = customer.City != null ? customer.City.StateId : null, 
                CityId = customer.CityId,
                RegType = customer.RegType, 
                Discount = customer.Discount
            };
        }
    }
}
