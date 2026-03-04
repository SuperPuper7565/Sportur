using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [Column(TypeName = "decimal(18,2)")]
        public decimal? PriceOverride { get; set; }

        [Required]
        public int StockQuantity { get; set; }

        public bool IsAvailable { get; set; } = true;

        [NotMapped]
        public decimal EffectivePrice =>
            PriceOverride ?? BicycleColor?.BicycleModel?.BasePrice ?? 0m;

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
