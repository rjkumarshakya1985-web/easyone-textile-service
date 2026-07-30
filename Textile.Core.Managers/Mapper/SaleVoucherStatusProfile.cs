using AutoMapper;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Views;

namespace Textile.Core.Managers.Mapper
{
    public class SaleVoucherStatusProfile : Profile
    {
        public SaleVoucherStatusProfile()
        {
            CreateMap<SaleVoucherStatus, SaleVoucherStatusView>().ReverseMap();
        }
    }
}
