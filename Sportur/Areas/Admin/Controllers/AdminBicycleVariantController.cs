using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sportur.Context;
using Sportur.Models;
using Sportur.ViewModels;

namespace Sportur.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminBicycleVariantController : Controller
    {
        private readonly SporturDbContext _context;

        public AdminBicycleVariantController(SporturDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var variants = _context.BicycleVariants
                .Include(v => v.BicycleModel)
                .Include(v => v.BicycleColor)
                .Include(v => v.BicycleSize)
                .OrderBy(v => v.BicycleModel.Brand)
                .ThenBy(v => v.BicycleModel.ModelName)
                .ThenBy(v => v.BicycleColor.Color)
                .ThenBy(v => v.BicycleSize.FrameSize)
                .ToList();

            return View(variants);
        }

        public IActionResult Create()
        {
            return View(BuildFormViewModel());
        }

        [HttpPost]
        public IActionResult Create(AdminBicycleVariantFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(BuildFormViewModel(model));

            if (VariantExists(model.BicycleModelId, model.BicycleColorId, model.BicycleSizeId))
            {
                ModelState.AddModelError("", "Такой вариант уже существует.");
                return View(BuildFormViewModel(model));
            }

            var variant = new BicycleVariant
            {
                BicycleModelId = model.BicycleModelId,
                BicycleColorId = model.BicycleColorId,
                BicycleSizeId = model.BicycleSizeId,
                Price = model.Price,
                StockQuantity = model.StockQuantity,
                IsAvailable = model.IsAvailable
            };

            _context.BicycleVariants.Add(variant);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var variant = _context.BicycleVariants.Find(id);
            if (variant == null)
                return NotFound();

            var model = BuildFormViewModel(new AdminBicycleVariantFormViewModel
            {
                Id = variant.Id,
                BicycleModelId = variant.BicycleModelId,
                BicycleColorId = variant.BicycleColorId,
                BicycleSizeId = variant.BicycleSizeId,
                Price = variant.Price,
                StockQuantity = variant.StockQuantity,
                IsAvailable = variant.IsAvailable
            });

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(AdminBicycleVariantFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(BuildFormViewModel(model));

            var variant = _context.BicycleVariants.Find(model.Id);
            if (variant == null)
                return NotFound();

            if (VariantExists(model.BicycleModelId, model.BicycleColorId, model.BicycleSizeId, variant.Id))
            {
                ModelState.AddModelError("", "Такой вариант уже существует.");
                return View(BuildFormViewModel(model));
            }

            variant.BicycleModelId = model.BicycleModelId;
            variant.BicycleColorId = model.BicycleColorId;
            variant.BicycleSizeId = model.BicycleSizeId;
            variant.Price = model.Price;
            variant.StockQuantity = model.StockQuantity;
            variant.IsAvailable = model.IsAvailable;

            _context.BicycleVariants.Update(variant);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var variant = _context.BicycleVariants
                .Include(v => v.BicycleModel)
                .Include(v => v.BicycleColor)
                .Include(v => v.BicycleSize)
                .FirstOrDefault(v => v.Id == id);

            if (variant == null)
                return NotFound();

            return View(variant);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var variant = _context.BicycleVariants.Find(id);
            if (variant == null)
                return NotFound();

            _context.BicycleVariants.Remove(variant);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        private AdminBicycleVariantFormViewModel BuildFormViewModel(AdminBicycleVariantFormViewModel? model = null)
        {
            model ??= new AdminBicycleVariantFormViewModel();

            model.Models = _context.BicycleModels
                .AsNoTracking()
                .OrderBy(m => m.Brand)
                .ThenBy(m => m.ModelName)
                .Select(m => new VariantOption
                {
                    Id = m.Id,
                    Label = $"{m.Brand} {m.ModelName}"
                })
                .ToList();

            model.Colors = _context.BicycleColors
                .AsNoTracking()
                .Include(c => c.BicycleModel)
                .OrderBy(c => c.BicycleModel.Brand)
                .ThenBy(c => c.BicycleModel.ModelName)
                .ThenBy(c => c.Color)
                .Select(c => new VariantOption
                {
                    Id = c.Id,
                    BicycleModelId = c.BicycleModelId,
                    Label = $"{c.Color} — {c.BicycleModel.Brand} {c.BicycleModel.ModelName}"
                })
                .ToList();

            model.Sizes = _context.BicycleSizes
                .AsNoTracking()
                .Include(s => s.BicycleModel)
                .OrderBy(s => s.BicycleModel.Brand)
                .ThenBy(s => s.BicycleModel.ModelName)
                .ThenBy(s => s.FrameSize)
                .Select(s => new VariantOption
                {
                    Id = s.Id,
                    BicycleModelId = s.BicycleModelId,
                    Label = $"{s.FrameSize} — {s.BicycleModel.Brand} {s.BicycleModel.ModelName}"
                })
                .ToList();

            return model;
        }

        private bool VariantExists(int modelId, int colorId, int sizeId, int? excludeId = null)
        {
            return _context.BicycleVariants.Any(v =>
                v.BicycleModelId == modelId &&
                v.BicycleColorId == colorId &&
                v.BicycleSizeId == sizeId &&
                (!excludeId.HasValue || v.Id != excludeId.Value));
        }
    }
}
