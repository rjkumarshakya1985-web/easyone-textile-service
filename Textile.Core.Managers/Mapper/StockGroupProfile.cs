using AutoMapper;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Models.Response.Masters;

namespace Textile.Core.Managers.Mapper
{
    public class StockGroupProfile : Profile
    {
        public StockGroupProfile()
        {
            CreateMap<StockGroup, StockGroupResponse>().ReverseMap();
        }
    }

   
}
