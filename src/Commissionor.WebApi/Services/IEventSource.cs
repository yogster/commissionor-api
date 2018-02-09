using System.Threading.Tasks;

namespace Commissionor.WebApi.Services
{
    /// <summary>
    /// Abstraction of an event source that sends events to clients
    /// </summary>
    public interface IEventSource
    {
        /// <summary>
        /// Sends an event to all clients
        /// </summary>
        /// <returns>A task that gets completed when all clients have been sent the event</returns>
        /// <param name="evt">The event data</param>
        Task FireEvent(object evt);
    }
}
