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
    public class BicycleColorsController : Controller
    {
        private readonly SporturDbContext _context;

        public BicycleColorsController(SporturDbContext context)
        {
            _context = context;
        }

        // GET: BicycleColors
        public async Task<IActionResult> Index()
        {
            var sporturDbContext = _context.BicycleColors.Include(b => b.BicycleModel);
            return View(await sporturDbContext.ToListAsync());
        }

        // GET: BicycleColors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bicycleColor = await _context.BicycleColors
                .Include(b => b.BicycleModel)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bicycleColor == null)
            {
                return NotFound();
            }

            return View(bicycleColor);
        }

        // GET: BicycleColors/Create
        public IActionResult Create()
        {
            ViewData["BicycleModelId"] = new SelectList(_context.BicycleModels, "Id", "Brakes");
            return View();
        }

        // POST: BicycleColors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BicycleModelId,Color,PhotoUrl,IsAvailable")] BicycleColor bicycleColor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bicycleColor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BicycleModelId"] = new SelectList(_context.BicycleModels, "Id", "Brakes", bicycleColor.BicycleModelId);
            return View(bicycleColor);
        }

        // GET: BicycleColors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bicycleColor = await _context.BicycleColors.FindAsync(id);
            if (bicycleColor == null)
            {
                return NotFound();
            }
            ViewData["BicycleModelId"] = new SelectList(_context.BicycleModels, "Id", "Brakes", bicycleColor.BicycleModelId);
            return View(bicycleColor);
        }

        // POST: BicycleColors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BicycleModelId,Color,PhotoUrl,IsAvailable")] BicycleColor bicycleColor)
        {
            if (id != bicycleColor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bicycleColor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BicycleColorExists(bicycleColor.Id))
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
            ViewData["BicycleModelId"] = new SelectList(_context.BicycleModels, "Id", "Brakes", bicycleColor.BicycleModelId);
            return View(bicycleColor);
        }

        // GET: BicycleColors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bicycleColor = await _context.BicycleColors
                .Include(b => b.BicycleModel)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bicycleColor == null)
            {
                return NotFound();
            }

            return View(bicycleColor);
        }

        // POST: BicycleColors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bicycleColor = await _context.BicycleColors.FindAsync(id);
            if (bicycleColor != null)
            {
                _context.BicycleColors.Remove(bicycleColor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BicycleColorExists(int id)
        {
            return _context.BicycleColors.Any(e => e.Id == id);
        }
    }
}
