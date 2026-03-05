using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sportur.Context;
using Sportur.Models;
using Sportur.Services;
using Sportur.ViewModels;

namespace Sportur.Controllers
{
    public class CatalogController : Controller
    {
        private readonly SporturDbContext _context;
        private readonly IPricingService _pricingService;

        public CatalogController(SporturDbContext context, IPricingService pricingService)
        {
            _context = context;
            _pricingService = pricingService;
        }

        public IActionResult Index(BicycleCategory? category)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            var models = _context.BicycleModels
                .Include(m => m.Colors)
                    .ThenInclude(c => c.Variants)
                .Where(m => category == null || m.Category == category)
                .ToList()
                .Select(m =>
                {
                    var activeColors = m.Colors.Where(c => c.IsActive).ToList();

                    return new BicycleCatalogItemViewModel
                    {
                        Id = m.Id,
                        Brand = m.Brand,
                        ModelName = m.ModelName,
                        Category = m.Category,
                        PreviewImageUrl = activeColors
                            .Select(c => c.PhotoUrl)
                            .FirstOrDefault(),
                        MinPrice = _pricingService.GetPrice(m, userId),
                        ColorsCount = activeColors.Count
                    };
                })
                .ToList();

            return View(models);
        }

        public IActionResult Details(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            var model = _context.BicycleModels
                .Include(m => m.Colors)
                    .ThenInclude(c => c.Variants)
                .FirstOrDefault(m => m.Id == id);

            if (model == null)
                return NotFound();

            var modelPrice = _pricingService.GetPrice(model, userId);
            var activeColors = model.Colors.Where(c => c.IsActive).ToList();
            var allVariants = activeColors.SelectMany(c => c.Variants).ToList();

            var vm = new BicycleDetailsViewModel
            {
                ModelId = model.Id,
                Brand = model.Brand,
                ModelName = model.ModelName,
                WheelDiameter = model.WheelDiameter,
                Category = model.Category,
                Description = model.Description,
                GearCount = model.GearCount,
                FrameMaterial = model.FrameMaterial,
                Fork = model.Fork,
                Headset = model.Headset,
                Shifters = model.Shifters,
                FrontDerailleur = model.FrontDerailleur,
                RearDerailleur = model.RearDerailleur,
                BottomBracket = model.BottomBracket,
                Crankset = model.Crankset,
                Cassette = model.Cassette,
                Chain = model.Chain,
                Brakes = model.Brakes,
                Hubs = model.Hubs,
                Rims = model.Rims,
                Tires = model.Tires,
                Accessories = model.Accessories,

                Colors = activeColors
                    .Select(c => new BicycleColorViewModel
                    {
                        Id = c.Id,
                        Color = c.Color,
                        PhotoUrl = c.PhotoUrl
                    })
                    .ToList(),

                Sizes = allVariants
                    .Where(v => !string.IsNullOrEmpty(v.FrameSize))
                    .Select(v => v.FrameSize)
                    .Distinct()
                    .OrderBy(s => s)
                    .ToList(),

                Variants = allVariants
                    .Select(v => new BicycleVariantViewModel
                    {
                        Id = v.Id,
                        ColorId = v.BicycleColorId,
                        ColorName = v.BicycleColor.Color,
                        SizeName = v.FrameSize,
                        Price = modelPrice,
                        StockQuantity = v.StockQuantity,
                        IsAvailable = v.IsAvailable
                    })
                    .ToList()
            };

            return View(vm);
        }
    }
}