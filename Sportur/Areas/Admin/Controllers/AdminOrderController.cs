using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sportur.Context;
using Sportur.Models;
using System.Text;
using System.Text.Json;

namespace Sportur.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminOrderController : Controller
    {
        private readonly SporturDbContext _context;

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
                        $"{variant.BicycleModel.Brand} {variant.BicycleModel.ModelName};" +
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
                        Model = i.BicycleVariant.BicycleModel.Brand + " " + i.BicycleVariant.BicycleModel.ModelName,
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
                        .ThenInclude(v => v.BicycleModel)
                .Include(o => o.Items)
                    .ThenInclude(i => i.BicycleVariant)
                        .ThenInclude(v => v.BicycleColor);
        }
    }
}