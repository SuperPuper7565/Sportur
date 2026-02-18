using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sportur.Context;
using Sportur.Models;

namespace Sportur.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminWholesaleController : Controller
    {
        private readonly SporturDbContext _context;

        public AdminWholesaleController(SporturDbContext context)
        {
            _context = context;
        }

        // Список всех персональных цен
        public IActionResult Index()
        {
            var prices = _context.WholesalePrices
                .Include(p => p.User)
                .Include(p => p.BicycleVariant)
                    .ThenInclude(v => v.BicycleModel)
                .ToList();

            return View(prices);
        }

        // Создание
        public IActionResult Create()
        {
            ViewBag.Users = _context.Users.ToList();
            ViewBag.Variants = _context.BicycleVariants
                .Include(v => v.BicycleModel)
                .ToList();

            return View();
        }

        [HttpPost]
        public IActionResult Create(WholesalePrice model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                ViewBag.Users = _context.Users.ToList();
                ViewBag.Variants = _context.BicycleVariants
                    .Include(v => v.BicycleModel)
                    .ToList();

                return View(model);
            }

            _context.WholesalePrices.Add(model);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // Редактирование
        public IActionResult Edit(int id)
        {
            var price = _context.WholesalePrices.Find(id);
            if (price == null) return NotFound();

            ViewBag.Users = _context.Users.ToList();
            ViewBag.Variants = _context.BicycleVariants
                .Include(v => v.BicycleModel)
                .ToList();

            return View(price);
        }

        [HttpPost]
        public IActionResult Edit(WholesalePrice model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Users = _context.Users.ToList();
                ViewBag.Variants = _context.BicycleVariants
                    .Include(v => v.BicycleModel)
                    .ToList();

                return View(model);
            }

            _context.Update(model);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // Удаление
        public IActionResult Delete(int id)
        {
            var price = _context.WholesalePrices.Find(id);
            if (price == null) return NotFound();

            _context.WholesalePrices.Remove(price);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
