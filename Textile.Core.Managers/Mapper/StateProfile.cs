using AutoMapper;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Models;

namespace Textile.Core.Managers.Mapper
{

    public class StateProfile : Profile
    {
        public StateProfile()
        {
            CreateMap<State, StateRespose>()
                 .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                 .ForMember(d => d.Name, o => o.MapFrom(s => s.Name));

        }
    }
}
