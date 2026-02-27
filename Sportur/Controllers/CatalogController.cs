using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sportur.Context;
using Sportur.ViewModels;
using Sportur.Models;

namespace Sportur.Controllers
{
    public class CatalogController : Controller
    {
        private readonly SporturDbContext _context;

        public CatalogController(SporturDbContext context)
        {
            _context = context;
        }

        // =========================
        // Каталог
        // =========================
        public IActionResult Index(BicycleCategory? category)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("UserRole");

            var wholesalePrices = new Dictionary<int, decimal>();

            if (userId.HasValue && role == UserRole.Wholesale.ToString())
            {
                wholesalePrices = _context.WholesalePrices
                    .Where(w => w.UserId == userId.Value)
                    .GroupBy(w => w.BicycleVariantId)
                    .ToDictionary(
                        g => g.Key,
                        g => g.OrderByDescending(x => x.Id).First().Price
                    );
            }

            var models = _context.BicycleModels
                .Include(m => m.Colors)
                    .ThenInclude(c => c.Variants)
                .Where(m => category == null || m.Category == category)
                .ToList()
                .Select(m =>
                {
                    // фильтруем только активные цвета
                    var activeColors = m.Colors.Where(c => c.IsActive).ToList();

                    var allVariants = activeColors
                        .SelectMany(c => c.Variants)
                        .ToList();

                    decimal GetVariantPrice(BicycleVariant v)
                    {
                        if (wholesalePrices.TryGetValue(v.Id, out var wholesale))
                            return wholesale;

                        return v.PriceOverride ?? m.BasePrice;
                    }

                    return new BicycleCatalogItemViewModel
                    {
                        Id = m.Id,
                        Brand = m.Brand,
                        ModelName = m.ModelName,
                        Category = m.Category,

                        PreviewImageUrl = activeColors
                            .Select(c => c.PhotoUrl)
                            .FirstOrDefault(),

                        MinPrice = allVariants.Any()
                            ? allVariants.Min(v => GetVariantPrice(v))
                            : m.BasePrice,

                        ColorsCount = activeColors.Count
                    };
                })
                .ToList();

            return View(models);
        }

        // =========================
        // Детали модели
        // =========================
        public IActionResult Details(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("UserRole");

            var wholesalePrices = new Dictionary<int, decimal>();

            if (userId.HasValue && role == UserRole.Wholesale.ToString())
            {
                wholesalePrices = _context.WholesalePrices
                    .Where(w => w.UserId == userId.Value)
                    .GroupBy(w => w.BicycleVariantId)
                    .ToDictionary(
                        g => g.Key,
                        g => g.OrderByDescending(x => x.Id).First().Price
                    );
            }

            var model = _context.BicycleModels
                .Include(m => m.Colors)
                    .ThenInclude(c => c.Variants)
                .FirstOrDefault(m => m.Id == id);

            if (model == null)
                return NotFound();

            decimal GetVariantPrice(BicycleVariant v)
            {
                if (wholesalePrices.TryGetValue(v.Id, out var wholesale))
                    return wholesale;

                return v.PriceOverride ?? model.BasePrice;
            }

            var activeColors = model.Colors.Where(c => c.IsActive).ToList();

            var allVariants = activeColors
                .SelectMany(c => c.Variants)
                .ToList();

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
                         Price = GetVariantPrice(v),
                         StockQuantity = v.StockQuantity,
                         IsAvailable = v.IsAvailable
                     })
                     .ToList()
            };

            return View(vm);
        }
    }
}