using System.ComponentModel.DataAnnotations;

namespace Sportur.ViewModels
{
    public class AdminBicycleSizeFormViewModel
    {
        public int? Id { get; set; }

        [Required]
        [Display(Name = "Модель")]
        public int BicycleModelId { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Размер")]
        public string FrameSize { get; set; } = string.Empty;

        public List<VariantOption> Models { get; set; } = new();
    }
}
