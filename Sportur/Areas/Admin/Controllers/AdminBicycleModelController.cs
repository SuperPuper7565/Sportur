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
        public IActionResult Edit(BicycleModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _context.BicycleModels.Update(model);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
