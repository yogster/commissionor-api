using System.ComponentModel.DataAnnotations;

namespace Commissionor.WebApi.Models
{
    public class Location
    {
        public int Id { get; set; }
        [Required]
        public string ReaderId { get; set; }
        [Required]
        public string Site { get; set; }
        [Required]
        public string Room { get; set; }
        [Required]
        public string Door { get; set; }
    }
}
