using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Textile.Core.Entities.Enums;

namespace Textile.Core.Entities.Models.Requests.Customers
{
    public class UpdateCustomerStatusRequest
    {
        public Guid CustomerId { get; set; }
        public CustomerStatusActionType ActionType { get; set; }
    }
}
