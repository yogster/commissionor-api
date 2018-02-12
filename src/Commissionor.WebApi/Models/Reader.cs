using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Commissionor.WebApi.Models
{
    public class Reader
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string Placement { get; set; }
        [Required]
        public string Description { get; set; }

        public List<Location> Locations { get; set; }
    }
}
