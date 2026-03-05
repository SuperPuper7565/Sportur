using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sportur.Context;
using Sportur.Models.Cart;
using Sportur.Services;

namespace Sportur.Controllers
{
    public class CartController : Controller
    {
        private readonly SporturDbContext _context;
        private readonly IPricingService _pricingService;

        private const string CartKey = "cart";
        private const int WholesalePackSize = 5;

        public CartController(SporturDbContext context, IPricingService pricingService)
        {
            _context = context;
            _pricingService = pricingService;
        }

        [HttpPost]
        public IActionResult Add(int variantId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var isWholesale = _pricingService.IsApprovedWholesaleUser(userId);
            var quantityStep = isWholesale ? WholesalePackSize : 1;

            var variant = _context.BicycleVariants
                .Include(v => v.BicycleColor)
                    .ThenInclude(c => c.BicycleModel)
                .FirstOrDefault(v => v.Id == variantId && v.IsAvailable && v.StockQuantity > 0);

            if (variant == null)
                return BadRequest("Вариант недоступен");

            var cart = HttpContext.Session.GetObject<List<CartItem>>(CartKey) ?? new List<CartItem>();
            var item = cart.FirstOrDefault(x => x.VariantId == variantId);

            var price = _pricingService.GetPrice(variant.BicycleColor.BicycleModel, userId);

            if (item != null)
            {
                if (item.Quantity + quantityStep > variant.StockQuantity)
                    return BadRequest("Недостаточно товара на складе");

                item.Quantity += quantityStep;
            }
            else
            {
                if (variant.StockQuantity < quantityStep)
                    return BadRequest("Недостаточно товара на складе");

                cart.Add(new CartItem
                {
                    VariantId = variant.Id,
                    ModelName = variant.BicycleColor.BicycleModel.Brand + " " +
                                variant.BicycleColor.BicycleModel.ModelName,
                    Color = variant.BicycleColor.Color,
                    FrameSize = variant.FrameSize,
                    Price = price,
                    Quantity = quantityStep,
                    StockQuantity = variant.StockQuantity,
                    PhotoUrl = variant.BicycleColor.PhotoUrl
                });
            }

            HttpContext.Session.SetObject(CartKey, cart);
            return RedirectToAction("Index");
        }

        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(CartKey) ?? new List<CartItem>();
            return View(cart);
        }

        public IActionResult Remove(int variantId)
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(CartKey) ?? new();
            var item = cart.FirstOrDefault(x => x.VariantId == variantId);

            if (item != null)
                cart.Remove(item);

            HttpContext.Session.SetObject(CartKey, cart);
            return RedirectToAction("Index");
        }

        public IActionResult Increase(int variantId)
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(CartKey) ?? new List<CartItem>();
            var item = cart.FirstOrDefault(x => x.VariantId == variantId);

            if (item == null)
                return RedirectToAction("Index");

            var userId = HttpContext.Session.GetInt32("UserId");
            var step = _pricingService.IsApprovedWholesaleUser(userId) ? WholesalePackSize : 1;

            var stock = _context.BicycleVariants
                .Where(v => v.Id == variantId && v.IsAvailable)
                .Select(v => v.StockQuantity)
                .FirstOrDefault();

            if (item.Quantity + step <= stock)
                item.Quantity += step;

            HttpContext.Session.SetObject(CartKey, cart);
            return RedirectToAction("Index");
        }

        public IActionResult Decrease(int variantId)
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(CartKey) ?? new List<CartItem>();
            var item = cart.FirstOrDefault(x => x.VariantId == variantId);

            if (item == null)
                return RedirectToAction("Index");

            var userId = HttpContext.Session.GetInt32("UserId");
            var step = _pricingService.IsApprovedWholesaleUser(userId) ? WholesalePackSize : 1;

            item.Quantity -= step;

            if (item.Quantity <= 0)
                cart.Remove(item);

            HttpContext.Session.SetObject(CartKey, cart);
            return RedirectToAction("Index");
        }
    }
}