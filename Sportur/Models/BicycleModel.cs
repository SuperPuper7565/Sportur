using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        Female
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

        // Характеристики
        [Required, StringLength(100)]
        [Display(Name = "Диаметр колес")]
        public string WheelDiameter { get; set; }

        [Required]
        [Display(Name = "Количество скоростей")]
        public int GearCount { get; set; }

        [Required, StringLength(200)]
        [Display(Name = "Материал рамы")]
        public string FrameMaterial { get; set; }

        [Required, StringLength(200)]
        [Display(Name = "Вилка")]
        public string Fork { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Рулевая колонка")]
        public string Headset { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Шифтеры")]
        public string Shifters { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Передний переключатель")]
        public string FrontDerailleur { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Задний переключатель")]
        public string RearDerailleur { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Каретка")]
        public string BottomBracket { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Система шатунов")]
        public string Crankset { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Кассета / Трещотка")]
        public string Cassette { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Цепь")]
        public string Chain { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Тормоза")]
        public string Brakes { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Втулки")]
        public string Hubs { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Обода")]
        public string Rims { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Покрышки")]
        public string Tires { get; set; }

        [StringLength(500)]
        [Display(Name = "Дополнительное оборудование")]
        public string Accessories { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Базовая цена")]
        public decimal BasePrice { get; set; }

        public bool IsDeleted { get; set; } = false;

        public ICollection<BicycleColor> Colors { get; set; } = new List<BicycleColor>();
    }
}