using Sportur.Models;
using Sportur.ViewModels;

public class BicycleDetailsViewModel
{
    public int ModelId { get; set; }

    public string Brand { get; set; }
    public string ModelName { get; set; }
    public string WheelDiameter { get; set; }
    public BicycleCategory Category { get; set; }
    public string Description { get; set; }

    // UI-выбор
    public List<BicycleColorViewModel> Colors { get; set; } = new();
    public List<string> Sizes { get; set; } = new();

    // ВСЕ варианты
    public List<BicycleVariantViewModel> Variants { get; set; } = new();

    // ===== характеристики =====
    public int? GearCount { get; set; }
    public string FrameMaterial { get; set; }
    public string Fork { get; set; }
    public string Headset { get; set; }
    public string Shifters { get; set; }
    public string FrontDerailleur { get; set; }
    public string RearDerailleur { get; set; }
    public string BottomBracket { get; set; }
    public string Crankset { get; set; }
    public string Cassette { get; set; }
    public string Chain { get; set; }
    public string Brakes { get; set; }
    public string Hubs { get; set; }
    public string Rims { get; set; }
    public string Tires { get; set; }
    public string Accessories { get; set; }
}