using Sportur.Models;

namespace Sportur.ViewModels
{
    public class BicycleDetailsViewModel
    {
        public int ModelId { get; set; }

        public string Brand { get; set; }
        public string ModelName { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }

        // Общие характеристики
        public int GearCount { get; set; }
        public string WheelDiameter { get; set; }
        public string FrameMaterial { get; set; }
        public string Fork { get; set; }
        public string Brakes { get; set; }

        // Данные для выбора
        public List<BicycleColorViewModel> Colors { get; set; }
        public List<BicycleSizeViewModel> Sizes { get; set; }
    }

}
