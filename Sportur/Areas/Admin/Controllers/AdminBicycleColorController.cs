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

        public AdminBicycleColorController(SporturDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var colors = _context.BicycleColors
                .Include(c => c.BicycleModel)
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
        public IActionResult Create(AdminBicycleColorFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(BuildFormViewModel(model));

            var color = new BicycleColor
            {
                BicycleModelId = model.BicycleModelId,
                Color = model.Color
            };

            // Загружаем файл, если выбран
            if (model.PhotoFile != null && model.PhotoFile.Length > 0)
            {
                var fileName = Path.GetFileName(model.PhotoFile.FileName);
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "bicycles");

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    model.PhotoFile.CopyTo(stream);
                }

                color.PhotoUrl = $"/images/bicycles/{fileName}";
            }

            _context.BicycleColors.Add(color);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var color = _context.BicycleColors.Find(id);
            if (color == null)
                return NotFound();

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
        public IActionResult Edit(AdminBicycleColorFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(BuildFormViewModel(model));

            var color = _context.BicycleColors.Find(model.Id);
            if (color == null)
                return NotFound();

            color.BicycleModelId = model.BicycleModelId;
            color.Color = model.Color;

            // Загружаем новый файл, если выбран
            if (model.PhotoFile != null && model.PhotoFile.Length > 0)
            {
                var fileName = Path.GetFileName(model.PhotoFile.FileName);
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "bicycles");

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    model.PhotoFile.CopyTo(stream);
                }

                color.PhotoUrl = $"/images/bicycles/{fileName}";
            }

            _context.BicycleColors.Update(color);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var color = _context.BicycleColors
                .Include(c => c.BicycleModel)
                .FirstOrDefault(c => c.Id == id);

            if (color == null)
                return NotFound();

            return View(color);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var color = _context.BicycleColors.Find(id);
            if (color == null)
                return NotFound();

            _context.BicycleColors.Remove(color);
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
