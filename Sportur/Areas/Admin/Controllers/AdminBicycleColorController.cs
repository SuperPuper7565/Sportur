using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sportur.Context;
using Sportur.Models;
using Sportur.ViewModels;

namespace Sportur.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminBicycleColorController : Controller
    {
        private readonly SporturDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AdminBicycleColorController(SporturDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index()
        {
            var colors = _context.BicycleColors
                .Include(c => c.BicycleModel)
                .Where(c => c.IsActive)
                .OrderBy(c => c.BicycleModel.Brand)
                .ThenBy(c => c.BicycleModel.ModelName)
                .ThenBy(c => c.Color)
                .ToList();

            return View(colors);
        }

        public IActionResult Create()
        {
            return View(BuildFormViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(AdminBicycleColorFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(BuildFormViewModel(model));

            var color = new BicycleColor
            {
                BicycleModelId = model.BicycleModelId,
                Color = model.Color
            };

            await SavePhotoFileAsync(model, color);

            _context.BicycleColors.Add(color);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var color = _context.BicycleColors.Find(id);
            if (color == null) return NotFound();

            var model = BuildFormViewModel(new AdminBicycleColorFormViewModel
            {
                Id = color.Id,
                BicycleModelId = color.BicycleModelId,
                Color = color.Color,
                PhotoUrl = color.PhotoUrl
            });

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AdminBicycleColorFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(BuildFormViewModel(model));

            var color = await _context.BicycleColors.FindAsync(model.Id);
            if (color == null) return NotFound();

            color.BicycleModelId = model.BicycleModelId;
            color.Color = model.Color;

            await SavePhotoFileAsync(model, color);

            _context.BicycleColors.Update(color);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task SavePhotoFileAsync(AdminBicycleColorFormViewModel model, BicycleColor color)
        {
            if (model.PhotoFile == null || model.PhotoFile.Length == 0) return;

            // Ограничение размера файла (например до 5 МБ)
            if (model.PhotoFile.Length > 5 * 1024 * 1024)
            {
                ModelState.AddModelError("PhotoFile", "Файл слишком большой (макс 5 МБ).");
                return;
            }

            // Получаем безопасное уникальное имя файла
            var fileName = Path.GetFileNameWithoutExtension(model.PhotoFile.FileName);
            var ext = Path.GetExtension(model.PhotoFile.FileName);
            fileName = $"{fileName}_{Guid.NewGuid()}{ext}";

            // Полный путь через WebRootPath
            var uploadPath = Path.Combine(_env.WebRootPath, "images", "bicycles");

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.PhotoFile.CopyToAsync(stream);
            }

            // Сохраняем относительный путь для отображения в браузере
            color.PhotoUrl = $"/images/bicycles/{fileName}";
        }

        public IActionResult Delete(int id)
        {
            var color = _context.BicycleColors
                .Include(c => c.BicycleModel)
                .FirstOrDefault(c => c.Id == id);

            if (color == null) return NotFound();

            return View(color);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var color = _context.BicycleColors.Find(id);
            if (color == null) return NotFound();

            color.IsActive = false;
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        private AdminBicycleColorFormViewModel BuildFormViewModel(AdminBicycleColorFormViewModel? model = null)
        {
            model ??= new AdminBicycleColorFormViewModel();

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

            return model;
        }
    }
}