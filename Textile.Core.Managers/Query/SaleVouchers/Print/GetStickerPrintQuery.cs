using AutoMapper;
using MediatR;
using Textile.Core.Entities.DbEnitites;
using Textile.Core.Entities.Models.Response.Suppliers.Print;
using Textile.Core.Interfaces.Data;

namespace Textile.Core.Managers.Query.SaleVouchers.Print
{
   
    public class GetStickerPrintQuery : IRequest<IEnumerable<StickerPrint>>
    {
        public int SaleVoucherId { get; set; }
        public int ProductId { get; set; }
        public GetStickerPrintQuery(int id)
        {
            SaleVoucherId = id;
        }
    }

  
}
