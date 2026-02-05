using Sportur.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sportur.Models
{
    public class BicycleSize
    {
        [Key]
        public int Id { get; set; }

        public int BicycleModelId { get; set; }
        public BicycleModel BicycleModel { get; set; }

        [Required]
        [StringLength(20)]
        public string FrameSize { get; set; }
    }
}
