using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sportur.Context;
using Sportur.Models;
using Sportur.Models.Cart;
using Sportur.Services;
using Sportur.ViewModels;

namespace Sportur.Controllers
{
    public class OrderController : Controller
    {
        private readonly SporturDbContext _context;
        private readonly IPricingService _pricingService;
        private const string CartKey = "cart";
        private const int WholesalePackSize = 5;

        public OrderController(SporturDbContext context, IPricingService pricingService)
        {
            _context = context;
            _pricingService = pricingService;
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

            var variantIds = cart.Select(c => c.VariantId).ToList();
            var variants = _context.BicycleVariants
                .Include(v => v.BicycleColor)
                    .ThenInclude(c => c.BicycleModel)
                .Where(v => variantIds.Contains(v.Id))
                .ToList();

            var isWholesale = _pricingService.IsApprovedWholesaleUser(userId);

            foreach (var item in cart)
            {
                var variant = variants.First(v => v.Id == item.VariantId);

                if (!variant.IsAvailable || variant.StockQuantity < item.Quantity)
                {
                    TempData["CheckoutError"] = "Недостаточно товара на складе";
                    return RedirectToAction("Index", "Cart");
                }

                if (isWholesale && (item.Quantity < WholesalePackSize || item.Quantity % WholesalePackSize != 0))
                {
                    TempData["CheckoutError"] = "Для оптовых покупателей количество должно быть не менее 5 и кратно 5.";
                    return RedirectToAction("Index", "Cart");
                }
            }

            var order = new Order { UserId = userId };

            foreach (var item in cart)
            {
                var variant = variants.First(v => v.Id == item.VariantId);
                var actualPrice = _pricingService.GetPrice(variant.BicycleColor.BicycleModel, userId);

                order.Items.Add(new OrderItem
                {
                    BicycleVariantId = variant.Id,
                    Price = actualPrice,
                    Quantity = item.Quantity
                });

                variant.StockQuantity -= item.Quantity;
                if (variant.StockQuantity == 0)
                    variant.IsAvailable = false;
            }

            order.TotalPrice = order.Items.Sum(x => x.Price * x.Quantity);

            _context.Orders.Add(order);
            _context.SaveChanges();

            HttpContext.Session.Remove(CartKey);

            return RedirectToAction("Success");
        }

        public IActionResult Success()
        {
            return View();
        }

        public IActionResult MyOrders()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var orders = _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.Items)
                    .ThenInclude(i => i.BicycleVariant)
                        .ThenInclude(v => v.BicycleColor)
                            .ThenInclude(c => c.BicycleModel)
                .OrderByDescending(o => o.CreatedAt)
                .ToList()
                .Select(o => new OrderViewModel
                {
                    OrderId = o.Id,
                    CreatedAt = o.CreatedAt,
                    TotalPrice = o.TotalPrice,
                    Items = o.Items.Select(i => new OrderItemViewModel
                    {
                        ModelName = i.BicycleVariant.BicycleColor.BicycleModel.Brand + " " + i.BicycleVariant.BicycleColor.BicycleModel.ModelName,
                        Color = i.BicycleVariant.BicycleColor.Color,
                        FrameSize = i.BicycleVariant.FrameSize,
                        Price = i.Price,
                        Quantity = i.Quantity
                    }).ToList()
                }).ToList();

            return View(orders);
        }
    }
}