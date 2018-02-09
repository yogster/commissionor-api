using System.ComponentModel.DataAnnotations;

namespace Commissionor.WebApi.Models
{
    /// <summary>
    /// Defines the event data available when somebody taps in a card reader with a device.
    /// </summary>
    public class TapEvent
    {
        [Required]
        public string DeviceId { get; set; }
        [Required]
        public string ReaderId { get; set; }
    }
}
