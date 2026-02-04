using Sportur.Models;

namespace Sportur.ViewModels
{
    public class BicycleCatalogItemViewModel
    {
        public int Id { get; set; }

        public string Brand { get; set; }
        public string ModelName { get; set; }
        public BicycleCategory Category { get; set; }

        public string PreviewImageUrl { get; set; }

        public decimal MinPrice { get; set; }

        public int ColorsCount { get; set; }
    }
}
