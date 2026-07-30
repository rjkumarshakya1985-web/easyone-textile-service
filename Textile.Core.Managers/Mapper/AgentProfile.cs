using AutoMapper;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Models.Response.Agents;

namespace Textile.Core.Managers.Mapper
{
    public class AgentProfile : Profile
    {
        public AgentProfile()
        {
            CreateMap<Agent, AgentDTO>().ReverseMap();
        }
    }
}
