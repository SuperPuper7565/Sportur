using Microsoft.AspNetCore.Mvc;
using Sportur.Context;
using Sportur.Models;

namespace Sportur.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminBicycleModelController : Controller
    {
        private readonly SporturDbContext _context;

        public AdminBicycleModelController(SporturDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View(_context.BicycleModels.ToList());
        }

        public IActionResult Create()
        {
            return View(new BicycleModel());
        }

        [HttpPost]
        public IActionResult Create(BicycleModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _context.BicycleModels.Add(model);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var model = _context.BicycleModels.Find(id);
            if (model == null) return NotFound();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(BicycleModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var existing = _context.BicycleModels.Find(model.Id);
            if (existing == null)
                return NotFound();

            existing.Brand = model.Brand;
            existing.ModelName = model.ModelName;
            existing.Category = model.Category;
            existing.Description = model.Description;
            existing.WheelDiameter = model.WheelDiameter;
            existing.GearCount = model.GearCount;
            existing.FrameMaterial = model.FrameMaterial;
            existing.Fork = model.Fork;
            existing.Headset = model.Headset;
            existing.Shifters = model.Shifters;
            existing.FrontDerailleur = model.FrontDerailleur;
            existing.RearDerailleur = model.RearDerailleur;
            existing.BottomBracket = model.BottomBracket;
            existing.Crankset = model.Crankset;
            existing.Cassette = model.Cassette;
            existing.Chain = model.Chain;
            existing.Brakes = model.Brakes;
            existing.Hubs = model.Hubs;
            existing.Rims = model.Rims;
            existing.Tires = model.Tires;
            existing.Accessories = model.Accessories;
            existing.BasePrice = model.BasePrice;

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var model = _context.BicycleModels.Find(id);
            if (model == null) return NotFound();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var model = _context.BicycleModels.Find(id);
            if (model == null) return NotFound();

            // Soft delete
            model.IsDeleted = true;
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
