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
            // В Create цвета и ростовки НЕ грузим
            return View(BuildFormViewModel(loadDependentData: false));
        }

        [HttpPost]
        public IActionResult Create(AdminBicycleVariantFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(BuildFormViewModel(model, loadDependentData: false));

            if (!IsColorAndSizeBelongToModel(
                model.BicycleModelId,
                model.BicycleColorId,
                model.BicycleSizeId))
            {
                ModelState.AddModelError("", "Цвет и ростовка должны принадлежать выбранной модели.");
                return View(BuildFormViewModel(model, loadDependentData: false));
            }

            if (VariantExists(model.BicycleModelId, model.BicycleColorId, model.BicycleSizeId))
            {
                ModelState.AddModelError("", "Такой вариант уже существует.");
                return View(BuildFormViewModel(model, loadDependentData: false));
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

            var vm = new AdminBicycleVariantFormViewModel
            {
                Id = variant.Id,
                BicycleModelId = variant.BicycleModelId,
                BicycleColorId = variant.BicycleColorId,
                BicycleSizeId = variant.BicycleSizeId,
                Price = variant.Price,
                StockQuantity = variant.StockQuantity,
                IsAvailable = variant.IsAvailable
            };

            return View(BuildFormViewModel(vm, loadDependentData: true));
        }

        [HttpPost]
        public IActionResult Edit(AdminBicycleVariantFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(BuildFormViewModel(model, loadDependentData: true));

            if (!IsColorAndSizeBelongToModel(
                model.BicycleModelId,
                model.BicycleColorId,
                model.BicycleSizeId))
            {
                ModelState.AddModelError("", "Цвет и ростовка должны принадлежать выбранной модели.");
                return View(BuildFormViewModel(model, loadDependentData: true));
            }

            var variant = _context.BicycleVariants.Find(model.Id);
            if (variant == null)
                return NotFound();

            if (VariantExists(model.BicycleModelId, model.BicycleColorId, model.BicycleSizeId, variant.Id))
            {
                ModelState.AddModelError("", "Такой вариант уже существует.");
                return View(BuildFormViewModel(model, loadDependentData: true));
            }

            variant.BicycleModelId = model.BicycleModelId;
            variant.BicycleColorId = model.BicycleColorId;
            variant.BicycleSizeId = model.BicycleSizeId;
            variant.Price = model.Price;
            variant.StockQuantity = model.StockQuantity;
            variant.IsAvailable = model.IsAvailable;

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult GetModelOptions(int modelId)
        {
            var colors = _context.BicycleColors
                .AsNoTracking()
                .Where(c => c.BicycleModelId == modelId)
                .OrderBy(c => c.Color)
                .Select(c => new { id = c.Id, label = c.Color })
                .ToList();

            var sizes = _context.BicycleSizes
                .AsNoTracking()
                .Where(s => s.BicycleModelId == modelId)
                .OrderBy(s => s.FrameSize)
                .Select(s => new { id = s.Id, label = s.FrameSize })
                .ToList();

            return Json(new { colors, sizes });
        }

        // ===== helpers =====

        private AdminBicycleVariantFormViewModel BuildFormViewModel(
            AdminBicycleVariantFormViewModel? model = null,
            bool loadDependentData = true)
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

            if (!loadDependentData || model.BicycleModelId == 0)
            {
                model.Colors = new();
                model.Sizes = new();
                return model;
            }

            model.Colors = _context.BicycleColors
                .AsNoTracking()
                .Where(c => c.BicycleModelId == model.BicycleModelId)
                .OrderBy(c => c.Color)
                .Select(c => new VariantOption
                {
                    Id = c.Id,
                    Label = c.Color
                })
                .ToList();

            model.Sizes = _context.BicycleSizes
                .AsNoTracking()
                .Where(s => s.BicycleModelId == model.BicycleModelId)
                .OrderBy(s => s.FrameSize)
                .Select(s => new VariantOption
                {
                    Id = s.Id,
                    Label = s.FrameSize
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

        private bool IsColorAndSizeBelongToModel(int modelId, int colorId, int sizeId)
        {
            return _context.BicycleColors.Any(c => c.Id == colorId && c.BicycleModelId == modelId)
                && _context.BicycleSizes.Any(s => s.Id == sizeId && s.BicycleModelId == modelId);
        }
    }
}
