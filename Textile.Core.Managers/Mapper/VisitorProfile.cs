using AutoMapper;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Models.Requests.Visitors;
using Textile.Core.Entities.Models.Response.Visitors;

namespace Textile.Core.Managers.Mapper
{
    public class VisitorProfile : Profile
    {
        public VisitorProfile()
        {
            // Entity → Response (GET / TABLE)
            CreateMap<Visitor, VisitorResponse>();

            // Request → Entity (CREATE / UPDATE)
            CreateMap<VisitorRequest, Visitor>()
                .ForMember(d => d.Id, o => o.Ignore())          // create ke time
                .ForMember(d => d.CreatedBy, o => o.Ignore())
                .ForMember(d => d.CreatedOn, o => o.Ignore())
                .ForMember(d => d.CreatedByUserName, o => o.Ignore())
                .ForMember(d => d.ModifiedBy, o => o.Ignore())
                .ForMember(d => d.ModifiedOn, o => o.Ignore());

            // (Optional) Entity → Request (Edit form load)
            CreateMap<Visitor, VisitorRequest>();
        }
    }
}
