using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sportur.Context;
using Sportur.Models;
using Sportur.Services;
using Sportur.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

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

        public IActionResult Index(
            string? search,
            BicycleCategory? category,
            string? wheelDiameter,
            int? gearCount,
            string? frameMaterial,
            BrakeType? brakeType,
            string? brand)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            var query = _context.BicycleModels
                .Include(m => m.Colors)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(m => m.ModelName.Contains(search));

            if (category.HasValue)
                query = query.Where(m => m.Category == category.Value);

            if (!string.IsNullOrWhiteSpace(wheelDiameter))
                query = query.Where(m => m.WheelDiameter == wheelDiameter);

            if (gearCount.HasValue)
                query = query.Where(m => m.GearCount == gearCount.Value);

            if (!string.IsNullOrWhiteSpace(frameMaterial))
                query = query.Where(m => m.FrameMaterial == frameMaterial);

            if (brakeType.HasValue)
                query = query.Where(m => m.BrakeType == brakeType.Value);

            if (!string.IsNullOrWhiteSpace(brand))
                query = query.Where(m => m.Brand == brand);

            var models = query
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
                        CategoryDisplayName = GetEnumDisplayName(m.Category),
                        PreviewImageUrl = activeColors
                            .Select(c => c.PhotoUrl)
                            .FirstOrDefault(),
                        MinPrice = _pricingService.GetPrice(m, userId),
                        ColorsCount = activeColors.Count
                    };
                })
                .ToList();

            var vm = new CatalogIndexViewModel
            {
                Items = models,
                Search = search,
                Category = category,
                WheelDiameter = wheelDiameter,
                GearCount = gearCount,
                FrameMaterial = frameMaterial,
                BrakeType = brakeType,
                Brand = brand,
                AvailableWheelDiameters = _context.BicycleModels
                    .Select(m => m.WheelDiameter)
                    .Distinct()
                    .OrderBy(v => v)
                    .ToList(),
                AvailableGearCounts = _context.BicycleModels
                    .Select(m => m.GearCount)
                    .Distinct()
                    .OrderBy(v => v)
                    .ToList(),
                AvailableFrameMaterials = _context.BicycleModels
                    .Select(m => m.FrameMaterial)
                    .Distinct()
                    .OrderBy(v => v)
                    .ToList(),
                AvailableBrands = _context.BicycleModels
                    .Select(m => m.Brand)
                    .Distinct()
                    .OrderBy(v => v)
                    .ToList()
            };

            return View(vm);
        }

        private static string GetEnumDisplayName<TEnum>(TEnum value) where TEnum : Enum
        {
            var member = typeof(TEnum).GetMember(value.ToString()).FirstOrDefault();
            var displayAttribute = member?.GetCustomAttribute<DisplayAttribute>();
            return displayAttribute?.GetName() ?? value.ToString();
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
                BrakeType = model.BrakeType,
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