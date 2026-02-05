using System.ComponentModel.DataAnnotations;

namespace Sportur.ViewModels
{
    public class AdminBicycleColorFormViewModel
    {
        public int? Id { get; set; }

        [Required]
        [Display(Name = "Модель")]
        public int BicycleModelId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Цвет")]
        public string Color { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Ссылка на фото")]
        public string? PhotoUrl { get; set; }

        public List<VariantOption> Models { get; set; } = new();
    }
}
