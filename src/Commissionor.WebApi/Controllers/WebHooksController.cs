using System.Threading.Tasks;
using Commissionor.WebApi.Models;
using Commissionor.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Commissionor.WebApi.Controllers
{
    /// <summary>
    /// Implements web hooks that are called by third-party services.
    /// </summary>
    [Route("api/[controller]")]
    public class WebHooksController : Controller
    {
        /// <summary>
        /// Called when somebody taps in a card reader with a device.
        /// </summary>
        /// <remarks>
        /// The data is forwarded to Commissionor clients.
        /// </remarks>
        [HttpPost("tap")]
        public async Task<IActionResult> OnTapEvent([FromServices] IEventSource eventSource, [FromBody] TapEvent tapEvent)
        {
            if (tapEvent != null && ModelState.IsValid)
            {
                await eventSource.FireEvent(tapEvent);
                return Ok();
            }
            return BadRequest(ModelState);
        }
    }
}
