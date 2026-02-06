using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sportur.Context;
using Sportur.Models;
using Sportur.Models.Cart;
using Sportur.ViewModels;

namespace Sportur.Controllers
{
    public class OrderController : Controller
    {
        private readonly SporturDbContext _context;
        private const string CartKey = "cart";

        public OrderController(SporturDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Checkout()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["CheckoutAuthMessage"] = "Для оформления заказа необходимо авторизоваться.";
                return RedirectToAction("Index", "Cart");
            }

            var cart = HttpContext.Session.GetObject<List<CartItem>>(CartKey);

            if (cart == null || !cart.Any())
                return RedirectToAction("Index", "Cart");

            // 1️ Получаем все варианты из БД
            var variantIds = cart.Select(c => c.VariantId).ToList();

            var variants = _context.BicycleVariants
                .Where(v => variantIds.Contains(v.Id))
                .ToList();

            // 2️ ВАЛИДАЦИЯ остатков
            foreach (var item in cart)
            {
                var variant = variants.First(v => v.Id == item.VariantId);

                if (!variant.IsAvailable || variant.StockQuantity < item.Quantity)
                {
                    ModelState.AddModelError("", "Недостаточно товара на складе");
                    return RedirectToAction("Index", "Cart");
                }
            }

            // 3️ Создаём заказ
            var order = new Order
            {
                UserId = userId,
                TotalPrice = cart.Sum(x => x.Price * x.Quantity)
            };

            foreach (var item in cart)
            {
                var variant = variants.First(v => v.Id == item.VariantId);

                order.Items.Add(new OrderItem
                {
                    BicycleVariantId = variant.Id,
                    Price = variant.Price,
                    Quantity = item.Quantity
                });

                // 4️⃣ Списание остатков
                variant.StockQuantity -= item.Quantity;

                if (variant.StockQuantity == 0)
                    variant.IsAvailable = false;
            }

            _context.Orders.Add(order);
            _context.SaveChanges();

            // 5️⃣ Очистка корзины
            HttpContext.Session.Remove(CartKey);

            return RedirectToAction("Success");
        }

        public IActionResult Success()
        {
            return View();
        }

        public IActionResult MyOrders()
        {
            // Получаем текущего пользователя из сессии или Claims
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account"); // если не авторизован

            var orders = _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.Items)
                    .ThenInclude(i => i.BicycleVariant)
                        .ThenInclude(v => v.BicycleModel)
                .Include(o => o.Items)
                    .ThenInclude(i => i.BicycleVariant)
                        .ThenInclude(v => v.BicycleColor)
                .Include(o => o.Items)
                    .ThenInclude(i => i.BicycleVariant)
                        .ThenInclude(v => v.BicycleSize)
                .OrderByDescending(o => o.OrderDate)
                .ToList()
                .Select(o => new OrderViewModel
                {
                    OrderId = o.Id,
                    CreatedAt = o.OrderDate,
                    TotalPrice = o.Items.Sum(i => i.BicycleVariant.Price * i.Quantity),
                    Items = o.Items.Select(i => new OrderItemViewModel
                    {
                        ModelName = i.BicycleVariant.BicycleModel.Brand + " " + i.BicycleVariant.BicycleModel.ModelName,
                        Color = i.BicycleVariant.BicycleColor.Color,
                        FrameSize = i.BicycleVariant.BicycleSize.FrameSize,
                        Price = i.BicycleVariant.Price,
                        Quantity = i.Quantity
                    }).ToList()
                }).ToList();

            return View(orders);
        }
    }
}
