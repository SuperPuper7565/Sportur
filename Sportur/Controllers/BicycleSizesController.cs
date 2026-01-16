using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Sportur.Context;

namespace Sportur.Controllers
{
    public class BicycleSizesController : Controller
    {
        private readonly SporturDbContext _context;

        public BicycleSizesController(SporturDbContext context)
        {
            _context = context;
        }

        // GET: BicycleSizes
        public async Task<IActionResult> Index()
        {
            var sporturDbContext = _context.BicycleSizes.Include(b => b.BicycleModel);
            return View(await sporturDbContext.ToListAsync());
        }

        // GET: BicycleSizes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bicycleSize = await _context.BicycleSizes
                .Include(b => b.BicycleModel)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bicycleSize == null)
            {
                return NotFound();
            }

            return View(bicycleSize);
        }

        // GET: BicycleSizes/Create
        public IActionResult Create()
        {
            ViewData["BicycleModelId"] = new SelectList(_context.BicycleModels, "Id", "Brakes");
            return View();
        }

        // POST: BicycleSizes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BicycleModelId,FrameSize,StockQuantity,Price")] BicycleSize bicycleSize)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bicycleSize);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BicycleModelId"] = new SelectList(_context.BicycleModels, "Id", "Brakes", bicycleSize.BicycleModelId);
            return View(bicycleSize);
        }

        // GET: BicycleSizes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bicycleSize = await _context.BicycleSizes.FindAsync(id);
            if (bicycleSize == null)
            {
                return NotFound();
            }
            ViewData["BicycleModelId"] = new SelectList(_context.BicycleModels, "Id", "Brakes", bicycleSize.BicycleModelId);
            return View(bicycleSize);
        }

        // POST: BicycleSizes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BicycleModelId,FrameSize,StockQuantity,Price")] BicycleSize bicycleSize)
        {
            if (id != bicycleSize.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bicycleSize);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BicycleSizeExists(bicycleSize.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BicycleModelId"] = new SelectList(_context.BicycleModels, "Id", "Brakes", bicycleSize.BicycleModelId);
            return View(bicycleSize);
        }

        // GET: BicycleSizes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bicycleSize = await _context.BicycleSizes
                .Include(b => b.BicycleModel)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bicycleSize == null)
            {
                return NotFound();
            }

            return View(bicycleSize);
        }

        // POST: BicycleSizes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bicycleSize = await _context.BicycleSizes.FindAsync(id);
            if (bicycleSize != null)
            {
                _context.BicycleSizes.Remove(bicycleSize);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BicycleSizeExists(int id)
        {
            return _context.BicycleSizes.Any(e => e.Id == id);
        }
    }
}
