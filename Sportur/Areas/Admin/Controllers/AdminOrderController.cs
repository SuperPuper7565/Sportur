using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sportur.Context;
using System.Text;

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

        // Список заказов
        public IActionResult Index()
        {
            var orders = _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.BicycleVariant)
                        .ThenInclude(v => v.BicycleModel)
                .Include(o => o.Items)
                    .ThenInclude(i => i.BicycleVariant)
                        .ThenInclude(v => v.BicycleColor)
                .OrderByDescending(o => o.CreatedAt)
                .ToList();

            return View(orders);
        }

        // Детали конкретного заказа
        public IActionResult Details(int id)
        {
            var order = _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.BicycleVariant)
                        .ThenInclude(v => v.BicycleModel)
                .Include(o => o.Items)
                    .ThenInclude(i => i.BicycleVariant)
                        .ThenInclude(v => v.BicycleColor)
                .FirstOrDefault(o => o.Id == id);

            if (order == null)
                return NotFound();

            return View(order);
        }

        // Экспорт заказов в CSV
        public IActionResult ExportCsv()
        {
            var orders = _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.BicycleVariant)
                        .ThenInclude(v => v.BicycleModel)
                .Include(o => o.Items)
                    .ThenInclude(i => i.BicycleVariant)
                        .ThenInclude(v => v.BicycleColor)
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
    }
}