using Sportur.Models;

namespace Sportur.ViewModels
{
    public class CatalogIndexViewModel
    {
        public List<BicycleCatalogItemViewModel> Items { get; set; } = new();

        public string? Search { get; set; }
        public BicycleCategory? Category { get; set; }
        public string? WheelDiameter { get; set; }
        public int? GearCount { get; set; }
        public string? FrameMaterial { get; set; }
        public BrakeType? BrakeType { get; set; }
        public string? Brand { get; set; }

        public List<string> AvailableWheelDiameters { get; set; } = new();
        public List<int> AvailableGearCounts { get; set; } = new();
        public List<string> AvailableFrameMaterials { get; set; } = new();
        public List<string> AvailableBrands { get; set; } = new();
    }
}
