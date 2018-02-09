using System.Threading.Tasks;
using Commissionor.WebApi.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Commissionor.WebApi.Services
{
    /// <summary>
    /// An implementation of IEventSource using SignalR
    /// </summary>
    public class SignalREventSource : IEventSource
    {
        private readonly IHubContext<EventHub> hubContext;

        public SignalREventSource(IHubContext<EventHub> hubContext)
        {
            this.hubContext = hubContext;
        }

        public Task FireEvent(object evt)
        {
            return hubContext.Clients.All.InvokeAsync("event", evt);
        }
    }
}
