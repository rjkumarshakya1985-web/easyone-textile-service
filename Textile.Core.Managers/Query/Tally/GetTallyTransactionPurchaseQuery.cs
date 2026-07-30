using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Textile.Core.Entities.Models.Response.Tally;

namespace Textile.Core.Managers.Query.Tally
{
    public class GetTallyTransactionPurchaseQuery : IRequest<TallyTransactionPurchaseResponse>
    {
        public int Id { get; set; }
    }
}
