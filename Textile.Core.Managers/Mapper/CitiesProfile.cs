using AutoMapper;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Models;


namespace Textile.Core.Managers.Mapper
{

    public class CitiesProfile : Profile
    {
        public CitiesProfile()
        {
            // Entity → Response (GET / TABLE)
            CreateMap<City, CitiesResponse>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Name));

        }
    }
}
