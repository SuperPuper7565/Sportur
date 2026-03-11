using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sportur.Context;
using Sportur.Models;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Sportur.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminOrderController : Controller
    {
        private readonly SporturDbContext _context;

        private static readonly HashSet<string> AllowedStatuses = new()
        {
            "Новый",
            "В обработке",
            "Отправлен",
            "Выполнен",
            "Отменён"
        };

        public AdminOrderController(SporturDbContext context)
        {
            _context = context;
        }

        // =========================
        // Список заказов
        // =========================
        public IActionResult Index()
        {
            var orders = BuildOrdersQuery()
                .OrderByDescending(o => o.CreatedAt)
                .ToList();

            return View(orders);
        }

        // =========================
        // Детали конкретного заказа
        // =========================
        public IActionResult Details(int id)
        {
            var order = BuildOrdersQuery()
                .FirstOrDefault(o => o.Id == id);

            if (order == null)
                return NotFound();

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cancel(int id)
        {
            var order = _context.Orders.FirstOrDefault(o => o.Id == id);

            if (order == null)
                return NotFound();

            if (order.Status == "Выполнен")
            {
                TempData["OrderError"] = $"Нельзя отменить заказ #{order.Id}, так как он уже выполнен.";
                return RedirectToAction(nameof(Index));
            }

            order.Status = "Отменён";
            _context.SaveChanges();
            TempData["OrderMessage"] = $"Заказ #{order.Id} отменён.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Complete(int id)
        {
            var order = _context.Orders.FirstOrDefault(o => o.Id == id);

            if (order == null)
                return NotFound();

            if (order.Status == "Отменён")
            {
                TempData["OrderError"] = $"Нельзя завершить заказ #{order.Id}, так как он отменён.";
                return RedirectToAction(nameof(Index));
            }

            order.Status = "Выполнен";
            _context.SaveChanges();
            TempData["OrderMessage"] = $"Заказ #{order.Id} отмечен как выполненный.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateStatus(int id, string status)
        {
            var order = _context.Orders.FirstOrDefault(o => o.Id == id);

            if (order == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(status) || !AllowedStatuses.Contains(status))
            {
                TempData["OrderError"] = "Передан недопустимый статус заказа.";
                return RedirectToAction(nameof(Index));
            }

            if (order.Status == "Отменён" && status != "Отменён")
            {
                TempData["OrderError"] = $"Нельзя изменить статус заказа #{order.Id}, так как он отменён.";
                return RedirectToAction(nameof(Index));
            }

            order.Status = status;
            _context.SaveChanges();
            TempData["OrderMessage"] = $"Статус заказа #{order.Id} изменён на «{status}».";

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // Экспорт заказов в CSV
        // =========================
        public IActionResult ExportCsv()
        {
            var orders = BuildOrdersQuery()
                .OrderBy(o => o.CreatedAt)
                .ToList();

            var sb = new StringBuilder();
            sb.AppendLine("OrderId;CreatedAt;UserEmail;Model;Color;Size;Price;Quantity;Total");

            foreach (var order in orders)
            {
                var userEmail = order.User?.Email ?? "Guest";

                foreach (var item in order.Items)
                {
                    var variant = item.BicycleVariant;
                    var total = item.Quantity * item.Price;

                    sb.AppendLine($"{order.Id};{order.CreatedAt:yyyy-MM-dd HH:mm};{userEmail};" +
                        $"{variant.BicycleColor.BicycleModel.Brand} {variant.BicycleColor.BicycleModel.ModelName};" +
                        $"{variant.BicycleColor.Color};" +
                        $"{variant.FrameSize};" +
                        $"{item.Price};{item.Quantity};{total}");
                }
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", $"orders_{DateTime.Now:yyyyMMdd_HHmm}.csv");
        }

        // =========================
        // Экспорт заказов в JSON
        // =========================
        public IActionResult ExportJson()
        {
            var orders = BuildOrdersQuery()
                .OrderBy(o => o.CreatedAt)
                .Select(o => new
                {
                    o.Id,
                    o.CreatedAt,
                    UserEmail = o.User != null ? o.User.Email : "Guest",
                    Items = o.Items.Select(i => new
                    {
                        Model = i.BicycleVariant.BicycleColor.BicycleModel.Brand + " " + i.BicycleVariant.BicycleColor.BicycleModel.ModelName,
                        Color = i.BicycleVariant.BicycleColor.Color,
                        Size = i.BicycleVariant.FrameSize,
                        i.Price,
                        i.Quantity,
                        Total = i.Price * i.Quantity
                    })
                })
                .ToList();

            var json = JsonSerializer.Serialize(orders, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            return File(Encoding.UTF8.GetBytes(json), "application/json", $"orders_{DateTime.Now:yyyyMMdd_HHmm}.json");
        }

        // =========================
        // Приватный метод построения запроса
        // =========================
        private IQueryable<Order> BuildOrdersQuery()
        {
            return _context.Orders
                .Include(o => o.User)
                .Include(o => o.Items)
                    .ThenInclude(i => i.BicycleVariant)
                        .ThenInclude(v => v.BicycleColor)
                            .ThenInclude(c => c.BicycleModel);
        }
    }
}