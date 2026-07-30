using AutoMapper;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Models.Response.Suppliers;

namespace Textile.Core.Managers.Mapper
{
    public class SupplierProfile : Profile
    {
        public SupplierProfile()
        {
            CreateMap<Supplier, SupplierDTO>().ReverseMap();
        }
    }

}
