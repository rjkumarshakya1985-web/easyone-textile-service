using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Textile.Core.Entities.Models.Requests.Agents;
using Textile.Core.Entities.Models.Requests.Suppliers;

namespace Textile.Core.Managers.Commands.Agents
{
    public class UpdateAgentStatusCommand : IRequest<bool>
    {
        public UpdateAgentStatusRequest Request { get; }

        public UpdateAgentStatusCommand(UpdateAgentStatusRequest request)
        {
            Request = request;
        }
    }
}
