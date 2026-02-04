using System.ComponentModel.DataAnnotations;

namespace Sportur.Models
{
    public enum BicycleCategory
    {
        [Display(Name = "Горный")]
        Mountain,

        [Display(Name = "Городской")]
        City,

        [Display(Name = "Подростковый")]
        Teen,

        [Display(Name = "Женский")]
        Female,

        [Display(Name = "Электровелосипед")]
        Electric
    }

    public class BicycleModel
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(50)]
        [Display(Name = "Название модели")]
        public string ModelName { get; set; }

        [Required]
        [Display(Name = "Категория")]
        public BicycleCategory Category { get; set; }

        [Required, StringLength(50)]
        [Display(Name = "Бренд")]
        public string Brand { get; set; }

        [StringLength(500)]
        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Количество передач")]
        public int GearCount { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Диаметр колес")]
        public string WheelDiameter { get; set; }

        [Required, StringLength(200)]
        [Display(Name = "Материал рамы")]
        public string FrameMaterial { get; set; }

        [Required, StringLength(200)]
        [Display(Name = "Вилка")]
        public string Fork { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Тормоза")]
        public string Brakes { get; set; }

        public ICollection<BicycleColor> Colors { get; set; } = new List<BicycleColor>();
        public ICollection<BicycleSize> Sizes { get; set; } = new List<BicycleSize>();
        public ICollection<BicycleVariant> Variants { get; set; } = new List<BicycleVariant>();
    }
}
