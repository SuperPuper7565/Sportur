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
            var models = _context.BicycleModels
                .Include(m => m.Variants)
                    .ThenInclude(v => v.BicycleColor)
                .Where(m => category == null || m.Category == category)
                .Select(m => new BicycleCatalogItemViewModel
                {
                    Id = m.Id,
                    Brand = m.Brand,
                    ModelName = m.ModelName,

                    Category = m.Category,

                    PreviewImageUrl = m.Variants
                        .Select(v => v.BicycleColor.PhotoUrl)
                        .FirstOrDefault(),

                    MinPrice = m.Variants.Any()
                        ? m.Variants.Min(v => v.Price)
                        : 0,

                    ColorsCount = m.Variants
                        .Select(v => v.BicycleColorId)
                        .Distinct()
                        .Count()
                })
                .ToList();

            return View(models);
        }

        // =========================
        // Детали модели
        // =========================
        public IActionResult Details(int id)
        {
            var model = _context.BicycleModels
                .Include(m => m.Variants)
                    .ThenInclude(v => v.BicycleColor)
                .Include(m => m.Variants)
                    .ThenInclude(v => v.BicycleSize)
                .FirstOrDefault(m => m.Id == id);

            if (model == null)
                return NotFound();

            var vm = new BicycleDetailsViewModel
            {
                ModelId = model.Id,
                Brand = model.Brand,
                ModelName = model.ModelName,
                Category = model.Category,
                Description = model.Description,

                Colors = model.Variants
                    .Select(v => v.BicycleColor)
                    .Distinct()
                    .Select(c => new BicycleColorViewModel
                    {
                        Id = c.Id,
                        Color = c.Color,
                        PhotoUrl = c.PhotoUrl
                    })
                    .ToList(),

                Sizes = model.Variants
                    .Select(v => v.BicycleSize)
                    .Distinct()
                    .Select(s => new BicycleSizeViewModel
                    {
                        Id = s.Id,
                        FrameSize = s.FrameSize
                    })
                    .ToList(),

                Variants = model.Variants.Select(v => new BicycleVariantViewModel
                {
                    Id = v.Id,
                    ColorId = v.BicycleColorId,
                    SizeId = v.BicycleSizeId,
                    Price = v.Price,
                    StockQuantity = v.StockQuantity,
                    IsAvailable = v.IsAvailable
                })
                .ToList()
            };

            return View(vm);
        }
    }
}
