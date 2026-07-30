using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Textile.Core.Entities.Models.Response.Tally;

namespace Textile.Core.Managers.Query.Tally
{
    public class GetTallyProcessStepsQuery  :IRequest<List<TallyProcessResponse>>
    {
        public int CompanyId { get; set; }
        public int FinancialYearId { get; set; }
        public int processType { get; set; }
        public string? ReferenceNo { get; set; }
    }
   
}
