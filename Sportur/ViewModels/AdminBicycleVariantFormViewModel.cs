using System.ComponentModel.DataAnnotations;

namespace Sportur.ViewModels
{
    public class AdminBicycleVariantFormViewModel
    {
        public int? Id { get; set; }

        [Required]
        [Display(Name = "Модель")]
        public int BicycleModelId { get; set; }

        [Required]
        [Display(Name = "Цвет")]
        public int BicycleColorId { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Размер")]
        public string FrameSize { get; set; }

        [Required]
        [Range(0.01, 1000000)]
        [Display(Name = "Цена")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, 100000)]
        [Display(Name = "Остаток")]
        public int StockQuantity { get; set; }

        [Display(Name = "Доступен")]
        public bool IsAvailable { get; set; } = true;

        public List<VariantOption> Models { get; set; } = new();
        public List<VariantOption> Colors { get; set; } = new();
    }

    public class VariantOption
    {
        public int Id { get; set; }
        public int? BicycleModelId { get; set; }
        public string Label { get; set; } = string.Empty;
    }
}
