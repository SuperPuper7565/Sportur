using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sportur.Context;
using Sportur.Models;
using System.Linq;

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
                .Include(p => p.BicycleVariant)
                    .ThenInclude(v => v.BicycleColor)
                .ToList();

            return View(prices);
        }

        // Создание
        public IActionResult Create()
        {
            ViewBag.Users = _context.Users
                .Where(u => u.Role == UserRole.Wholesale && u.IsWholesaleApproved)
                .ToList();

            ViewBag.Variants = _context.BicycleVariants
                .Include(v => v.BicycleModel)
                .Include(v => v.BicycleColor)
                .ToList();

            return View();
        }

        [HttpPost]
        public IActionResult Create(WholesalePrice model)
        {
            var user = _context.Users.Find(model.UserId);

            if (user == null ||
                user.Role != UserRole.Wholesale ||
                !user.IsWholesaleApproved)
            {
                ModelState.AddModelError("",
                    "Цена может быть назначена только одобренному оптовому покупателю");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Users = _context.Users
                    .Where(u => u.Role == UserRole.Wholesale && u.IsWholesaleApproved)
                    .ToList();

                ViewBag.Variants = _context.BicycleVariants
                    .Include(v => v.BicycleModel)
                    .Include(v => v.BicycleColor)
                    .ToList();

                return View(model);
            }

            var existingPrice = _context.WholesalePrices
                .FirstOrDefault(x =>
                    x.UserId == model.UserId &&
                    x.BicycleVariantId == model.BicycleVariantId);

            if (existingPrice == null)
            {
                _context.WholesalePrices.Add(model);
            }
            else
            {
                existingPrice.Price = model.Price;
                _context.WholesalePrices.Update(existingPrice);
            }

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
                .Include(v => v.BicycleColor)
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
                    .Include(v => v.BicycleColor)
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