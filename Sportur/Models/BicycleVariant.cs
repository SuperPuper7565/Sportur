using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sportur.Models
{
    public class BicycleVariant
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BicycleModelId { get; set; }
        public BicycleModel BicycleModel { get; set; }

        [Required]
        public int BicycleColorId { get; set; }
        public BicycleColor BicycleColor { get; set; }

        [Required]
        public int BicycleSizeId { get; set; }
        public BicycleSize BicycleSize { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        public int StockQuantity { get; set; }

        public bool IsAvailable { get; set; } = true;
    }

}
