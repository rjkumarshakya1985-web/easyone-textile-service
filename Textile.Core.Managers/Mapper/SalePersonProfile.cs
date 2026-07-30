using AutoMapper;
using Textile.Core.Entities.DbEnitites.Sales;
using Textile.Core.Entities.Models.Requests;
using Textile.Core.Entities.Models.Response.SalePersons;

namespace Textile.Core.Managers.Mapper
{
    public class SalePersonProfile : Profile
    {
        public SalePersonProfile()
        {
            // ✅ Entity → Response
            CreateMap<SalePerson, SalePersonResponse>()
                .ForMember(d => d.CityName, o => o.MapFrom(s => s.City.Name))
                .ForMember(d => d.StateId, o => o.MapFrom(s => s.City.State.Id))
                .ForMember(d => d.StateName, o => o.MapFrom(s => s.City.State.Name));

            // ✅ Response → Entity (Edit case - optional but allowed)
            CreateMap<SalePersonResponse, SalePerson>()
                .ForMember(d => d.City, o => o.Ignore());

            // ✅ Request → Entity (Create/Update - MAIN mapping)
            CreateMap<SalePersonRequest, SalePerson>()
                .ForMember(d => d.City, o => o.Ignore());

            // ✅ Entity → Request (optional - edit form prefill)
            CreateMap<SalePerson, SalePersonRequest>();
        }
    }
}
