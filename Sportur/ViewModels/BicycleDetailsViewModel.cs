using Sportur.Models;
using Sportur.ViewModels;

public class BicycleDetailsViewModel
{
    public int ModelId { get; set; }

    public string Brand { get; set; }
    public string ModelName { get; set; }
    public BicycleCategory Category { get; set; }
    public string Description { get; set; }

    // UI-выбор
    public List<BicycleColorViewModel> Colors { get; set; }
    public List<BicycleSizeViewModel> Sizes { get; set; }

    // ВСЕ варианты
    public List<BicycleVariantViewModel> Variants { get; set; }
}
