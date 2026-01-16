using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sportur.Models
{
    public class WholesalePrice
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Ростовка")]
        public int BicycleSizeId { get; set; }
        public BicycleSize BicycleSize { get; set; }

        [Required]
        [Display(Name = "Оптовый покупатель")]
        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Оптовая цена")]
        public decimal Price { get; set; }
    }
}
