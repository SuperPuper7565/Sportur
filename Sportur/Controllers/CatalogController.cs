using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sportur.Context;
using Sportur.Models.ViewModels;
using Sportur.ViewModels;

namespace Sportur.Controllers
{
    public class CatalogController : Controller
    {

        private readonly SporturDbContext _context;

        public CatalogController(SporturDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int? categoryId)
        {
            var models = _context.BicycleModels
                .Include(m => m.Category)
                .Include(m => m.Colors)
                .Include(m => m.Sizes)
                .Where(m => categoryId == null || m.CategoryId == categoryId)
                .Select(m => new BicycleCatalogItemViewModel
                {
                    Id = m.Id,
                    Brand = m.Brand,
                    ModelName = m.ModelName,
                    CategoryName = m.Category.Name,

                    PreviewImageUrl = m.Colors
                        .Select(c => c.PhotoUrl)
                        .FirstOrDefault(),

                    MinPrice = m.Sizes.Any()
                        ? m.Sizes.Min(s => s.Price)
                        : 0,

                    ColorsCount = m.Colors.Count
                })
                .ToList();

            return View(models);
        }

        public IActionResult Details(int id)
        {
            var model = _context.BicycleModels
                .Include(m => m.Category)
                .Include(m => m.Colors)
                .Include(m => m.Sizes)
                .FirstOrDefault(m => m.Id == id);

            if (model == null)
                return NotFound();

            var vm = new BicycleDetailsViewModel
            {
                ModelId = model.Id,
                Brand = model.Brand,
                ModelName = model.ModelName,
                CategoryName = model.Category?.Name,
                Description = model.Description,
                Colors = model.Colors.Select(c => new BicycleColorViewModel
                {
                    Id = c.Id,
                    Color = c.Color,
                    PhotoUrl = c.PhotoUrl
                }).ToList(),
                Sizes = model.Sizes.Select(s => new BicycleSizeViewModel
                {
                    Id = s.Id,
                    FrameSize = s.FrameSize,
                    Price = s.Price,
                    StockQuantity = s.StockQuantity
                }).ToList()
            };

            return View(vm);
        }


    }

}
