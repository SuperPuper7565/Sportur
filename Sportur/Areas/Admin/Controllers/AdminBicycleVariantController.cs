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

        // =========================
        // Список вариантов
        // =========================
        public IActionResult Index()
        {
            var variants = _context.BicycleVariants
                .Include(v => v.BicycleModel)
                .Include(v => v.BicycleColor)
                .OrderBy(v => v.BicycleModel.Brand)
                .ThenBy(v => v.BicycleModel.ModelName)
                .ThenBy(v => v.BicycleColor.Color)
                .ToList();

            return View(variants);
        }

        // =========================
        // Создание нового варианта
        // =========================
        public IActionResult Create()
        {
            var modelVm = BuildFormViewModel(loadDependentData: false);

            // Если есть хотя бы одна модель, подставляем её для Price
            if (modelVm.Models.Any())
            {
                modelVm.BicycleModelId = modelVm.Models.First().Id;
                modelVm.Price = GetModelBasePrice(modelVm.BicycleModelId);
            }

            return View(modelVm);
        }

        [HttpPost]
        public IActionResult Create(AdminBicycleVariantFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(BuildFormViewModel(model, loadDependentData: false));

            if (VariantExists(model.BicycleModelId, model.BicycleColorId, model.FrameSize, model.Id))
            {
                ModelState.AddModelError("", "Такой вариант уже существует.");
                return View(BuildFormViewModel(model, loadDependentData: false));
            }

            var basePrice = GetModelBasePrice(model.BicycleModelId);

            var variant = new BicycleVariant
            {
                BicycleModelId = model.BicycleModelId,
                BicycleColorId = model.BicycleColorId,
                FrameSize = model.FrameSize,
                PriceOverride = GetOverridePrice(model.Price, basePrice),
                StockQuantity = model.StockQuantity,
                IsAvailable = model.IsAvailable
            };

            _context.BicycleVariants.Add(variant);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // Редактирование варианта
        // =========================
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
                FrameSize = variant.FrameSize,
                Price = variant.PriceOverride ?? GetModelBasePrice(variant.BicycleModelId),
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

            var variant = _context.BicycleVariants.Find(model.Id);
            if (variant == null)
                return NotFound();

            if (VariantExists(model.BicycleModelId, model.BicycleColorId, model.FrameSize, model.Id))
            {
                ModelState.AddModelError("", "Такой вариант уже существует.");
                return View(BuildFormViewModel(model, loadDependentData: false));
            }

            var basePrice = GetModelBasePrice(model.BicycleModelId);

            variant.BicycleModelId = model.BicycleModelId;
            variant.BicycleColorId = model.BicycleColorId;
            variant.FrameSize = model.FrameSize;
            variant.PriceOverride = GetOverridePrice(model.Price, basePrice);
            variant.StockQuantity = model.StockQuantity;
            variant.IsAvailable = model.IsAvailable;

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // Удаление варианта
        // =========================
        public IActionResult Delete(int id)
        {
            var variant = _context.BicycleVariants
                .Include(v => v.BicycleModel)
                .Include(v => v.BicycleColor)
                .FirstOrDefault(v => v.Id == id);

            if (variant == null)
                return NotFound();

            return View(variant);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var variant = _context.BicycleVariants.Find(id);
            if (variant == null)
                return NotFound();

            _context.BicycleVariants.Remove(variant);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // AJAX для подгрузки цветов и цены модели
        // =========================
        [HttpGet]
        public IActionResult GetModelOptions(int modelId)
        {
            var colors = GetActiveColors(modelId);
            var basePrice = GetModelBasePrice(modelId);

            return Json(new { colors, basePrice });
        }

        // =========================
        // Приватные помощники
        // =========================
        private decimal GetModelBasePrice(int modelId)
        {
            return _context.BicycleModels
                .Where(m => m.Id == modelId)
                .Select(m => m.BasePrice)
                .FirstOrDefault();
        }

        private List<VariantOption> GetActiveColors(int modelId)
        {
            return _context.BicycleColors
                .AsNoTracking()
                .Where(c => c.BicycleModelId == modelId && c.IsActive)
                .OrderBy(c => c.Color)
                .Select(c => new VariantOption { Id = c.Id, Label = c.Color })
                .ToList();
        }

        private decimal? GetOverridePrice(decimal selectedPrice, decimal basePrice)
        {
            return selectedPrice != basePrice ? selectedPrice : null;
        }

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

            if (model.BicycleModelId > 0 && model.Price <= 0)
            {
                model.Price = GetModelBasePrice(model.BicycleModelId);
            }

            if (!loadDependentData || model.BicycleModelId == 0)
            {
                model.Colors = new();
                return model;
            }

            model.Colors = GetActiveColors(model.BicycleModelId);

            return model;
        }

        private bool VariantExists(int modelId, int colorId, string frameSize, int? excludeId = null)
        {
            return _context.BicycleVariants.Any(v =>
                v.BicycleModelId == modelId &&
                v.BicycleColorId == colorId &&
                v.FrameSize == frameSize &&
                (!excludeId.HasValue || v.Id != excludeId.Value));
        }
    }
}