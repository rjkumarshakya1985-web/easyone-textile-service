using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Textile.Core.Entities.Enums;

namespace Textile.Core.Entities.Models.Requests.Agents
{
    public class UpdateAgentStatusRequest
    {
        public Guid AgentId { get; set; }
        public AgentStatusActionType ActionType { get; set; }
    }
}
