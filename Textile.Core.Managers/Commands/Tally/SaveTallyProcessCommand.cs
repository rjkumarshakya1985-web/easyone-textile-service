using MediatR;
using Textile.Core.Entities.Models.Requests.Tally;

namespace Textile.Core.Managers.Commands.Tally
{   
    public class SaveTallyProcessCommand : IRequest<bool>
    {       
        public List<TallyProcessRequest> TallyProcessRequests { get; }
        public SaveTallyProcessCommand(List<TallyProcessRequest> tallyprocessRequests)
        {
            TallyProcessRequests = tallyprocessRequests;
        }
    }
}
