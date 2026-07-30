using AutoMapper;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Models.Requests.Customers;
using Textile.Core.Entities.Models.Response.Customers;

namespace Textile.Core.Managers.Mapper
{

    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            // Entity → Response (GET / TABLE)
            CreateMap<Customer, CustomerResponse>()
                .ForMember(d => d.CityName, o => o.MapFrom(s => s.City.Name))
                .ForMember(d => d.StateName, o => o.MapFrom(s => s.City.State.Name))
                .ForMember(d => d.StateCode, o => o.MapFrom(s => s.City.State.Code));

            // Request → Entity (CREATE / UPDATE)
            CreateMap<CustomerRequest, Customer>()
                .ForMember(d => d.Id, o => o.Ignore())          // create ke time
                .ForMember(d => d.CreatedBy, o => o.Ignore())
                .ForMember(d => d.CreatedOn, o => o.Ignore())
                .ForMember(d => d.CreatedByUserName, o => o.Ignore())
                .ForMember(d => d.ModifiedBy, o => o.Ignore())
                .ForMember(d => d.ModifiedOn, o => o.Ignore())
                .ForMember(d => d.IsDeleted, o => o.Ignore());

            // (Optional) Entity → Request (Edit form load)
            CreateMap<Customer, CustomerRequest>();
        }
    }
}
