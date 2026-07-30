using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Textile.Core.Entities.Models.Response.PackingSlip
{
    public class BillPackingSlipsResponse
    {
        public int TotalPcs { get; set; }

        public decimal GrandTotal { get; set; }

        public List<PackingSlipResponse> PackingSlips { get; set; } = new List<PackingSlipResponse>();
    }
}
