using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sportur.Context;
using Sportur.Models;
using Sportur.ViewModels;

namespace Sportur.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminBicycleSizeController : Controller
    {
        private readonly SporturDbContext _context;

        public AdminBicycleSizeController(SporturDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var sizes = _context.BicycleSizes
                .Include(s => s.BicycleModel)
                .OrderBy(s => s.BicycleModel.Brand)
                .ThenBy(s => s.BicycleModel.ModelName)
                .ThenBy(s => s.FrameSize)
                .ToList();

            return View(sizes);
        }

        public IActionResult Create()
        {
            return View(BuildFormViewModel());
        }

        [HttpPost]
        public IActionResult Create(AdminBicycleSizeFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(BuildFormViewModel(model));

            var size = new BicycleSize
            {
                BicycleModelId = model.BicycleModelId,
                FrameSize = model.FrameSize
            };

            _context.BicycleSizes.Add(size);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var size = _context.BicycleSizes.Find(id);
            if (size == null)
                return NotFound();

            var model = BuildFormViewModel(new AdminBicycleSizeFormViewModel
            {
                Id = size.Id,
                BicycleModelId = size.BicycleModelId,
                FrameSize = size.FrameSize
            });

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(AdminBicycleSizeFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(BuildFormViewModel(model));

            var size = _context.BicycleSizes.Find(model.Id);
            if (size == null)
                return NotFound();

            size.BicycleModelId = model.BicycleModelId;
            size.FrameSize = model.FrameSize;

            _context.BicycleSizes.Update(size);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var size = _context.BicycleSizes
                .Include(s => s.BicycleModel)
                .FirstOrDefault(s => s.Id == id);

            if (size == null)
                return NotFound();

            return View(size);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var size = _context.BicycleSizes.Find(id);
            if (size == null)
                return NotFound();

            _context.BicycleSizes.Remove(size);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        private AdminBicycleSizeFormViewModel BuildFormViewModel(AdminBicycleSizeFormViewModel? model = null)
        {
            model ??= new AdminBicycleSizeFormViewModel();

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
