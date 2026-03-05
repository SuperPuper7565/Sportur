using System.ComponentModel.DataAnnotations;

namespace Sportur.Models
{
    public class BicycleVariant
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BicycleColorId { get; set; }
        public BicycleColor BicycleColor { get; set; }

        [Required]
        [StringLength(20)]
        public string FrameSize { get; set; }

        [Required]
        public int StockQuantity { get; set; }

        public bool IsAvailable { get; set; } = true;

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
