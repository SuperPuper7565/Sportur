using Microsoft.AspNetCore.Http;

namespace Sportur.ViewModels
{
    public class AdminBicycleColorFormViewModel
    {
        public int Id { get; set; }
        public int BicycleModelId { get; set; }
        public string Color { get; set; }

        public string? PhotoUrl { get; set; }

        public IFormFile PhotoFile { get; set; }

        public List<VariantOption> Models { get; set; } = new();
    }
}
