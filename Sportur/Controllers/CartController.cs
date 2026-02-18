using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sportur.Context;
using Sportur.Models.Cart;

namespace Sportur.Controllers
{
    public class CartController : Controller
    {
        private readonly SporturDbContext _context;
        private const string CartKey = "cart";

        public CartController(SporturDbContext context)
        {
            _context = context;
        }

        // Добавить в корзину
        [HttpPost]
        public IActionResult Add(int variantId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("UserRole");

            var variant = _context.BicycleVariants
                .Include(v => v.BicycleModel)
                .Include(v => v.BicycleColor)
                .Include(v => v.BicycleSize)
                .FirstOrDefault(v =>
                    v.Id == variantId &&
                    v.IsAvailable &&
                    v.StockQuantity > 0);

            if (variant == null)
                return BadRequest("Вариант недоступен");

            var cart = HttpContext.Session.GetObject<List<CartItem>>(CartKey)
                       ?? new List<CartItem>();

            var item = cart.FirstOrDefault(x => x.VariantId == variantId);

            var price = variant.Price;
            if (userId.HasValue && role == UserRole.Wholesale.ToString())
            {
                price = _context.WholesalePrices
                    .Where(w => w.UserId == userId.Value && w.BicycleVariantId == variant.Id)
                    .Select(w => w.Price)
                    .FirstOrDefault() switch
                {
                    0 => variant.Price,
                    var wholesalePrice => wholesalePrice
                };
            }

            if (item != null)
            {
                if (item.Quantity >= variant.StockQuantity)
                    return BadRequest("Недостаточно товара на складе");

                item.Quantity++;
            }
            else
            {
                cart.Add(new CartItem
                {
                    VariantId = variant.Id,

                    ModelName = variant.BicycleModel.Brand + " " + variant.BicycleModel.ModelName,
                    Color = variant.BicycleColor.Color,
                    FrameSize = variant.BicycleSize.FrameSize,

                    Price = price,
                    Quantity = 1,
                    StockQuantity = variant.StockQuantity,
                    PhotoUrl = variant.BicycleColor.PhotoUrl
                });
            }

            HttpContext.Session.SetObject(CartKey, cart);
            return RedirectToAction("Index");
        }

        // Корзина
        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(CartKey)
                       ?? new List<CartItem>();

            return View(cart);
        }

        // Удалить позицию
        public IActionResult Remove(int variantId)
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(CartKey) ?? new();

            var item = cart.FirstOrDefault(x => x.VariantId == variantId);
            if (item != null)
                cart.Remove(item);

            HttpContext.Session.SetObject(CartKey, cart);
            return RedirectToAction("Index");
        }

        // Увеличить количество
        public IActionResult Increase(int variantId)
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(CartKey)
                       ?? new List<CartItem>();

            var item = cart.FirstOrDefault(x => x.VariantId == variantId);
            if (item == null)
                return RedirectToAction("Index");

            var stock = _context.BicycleVariants
                .Where(v => v.Id == variantId && v.IsAvailable)
                .Select(v => v.StockQuantity)
                .FirstOrDefault();

            if (item.Quantity < stock)
                item.Quantity++;

            HttpContext.Session.SetObject(CartKey, cart);
            return RedirectToAction("Index");
        }

        // Уменьшить количество
        public IActionResult Decrease(int variantId)
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(CartKey)
                       ?? new List<CartItem>();

            var item = cart.FirstOrDefault(x => x.VariantId == variantId);
            if (item == null)
                return RedirectToAction("Index");

            item.Quantity--;

            if (item.Quantity <= 0)
                cart.Remove(item);

            HttpContext.Session.SetObject(CartKey, cart);
            return RedirectToAction("Index");
        }
    }
}
