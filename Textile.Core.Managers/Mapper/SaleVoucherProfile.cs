using AutoMapper;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Models.Requests.SaleVouchers;

namespace Textile.Core.Managers.Mapper
{
    public class SaleVoucherProfile : Profile
    {
        public SaleVoucherProfile()
        {
            CreateMap<SaleVoucher, SaleVoucherRequest>().ReverseMap();
        }
    }

    public class SaleVoucherDetailProfile : Profile
    {
        public SaleVoucherDetailProfile()
        {
            CreateMap<SaleVoucherDetail, SaleVoucherDetailRequest>().ReverseMap();
        }
    }
}
