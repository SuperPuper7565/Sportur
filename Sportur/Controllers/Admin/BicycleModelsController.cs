using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Sportur.Context;
using Sportur.Models;

namespace Sportur.Controllers.Admin
{
    public class BicycleModelsController : Controller
    {
        private readonly SporturDbContext _context;

        public BicycleModelsController(SporturDbContext context)
        {
            _context = context;
        }

        // GET: BicycleModels
        public async Task<IActionResult> Index()
        {
            var sporturDbContext = _context.BicycleModels.Include(b => b.Category);
            return View(await sporturDbContext.ToListAsync());
        }

        // GET: BicycleModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bicycleModel = await _context.BicycleModels
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bicycleModel == null)
            {
                return NotFound();
            }

            return View(bicycleModel);
        }

        // GET: BicycleModels/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: BicycleModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ModelName,CategoryId,Brand,Description,GearCount,WheelDiameter,FrameMaterial,Fork,Brakes")] BicycleModel bicycleModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bicycleModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", bicycleModel.CategoryId);
            return View(bicycleModel);
        }

        // GET: BicycleModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bicycleModel = await _context.BicycleModels.FindAsync(id);
            if (bicycleModel == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", bicycleModel.CategoryId);
            return View(bicycleModel);
        }

        // POST: BicycleModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ModelName,CategoryId,Brand,Description,GearCount,WheelDiameter,FrameMaterial,Fork,Brakes")] BicycleModel bicycleModel)
        {
            if (id != bicycleModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bicycleModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BicycleModelExists(bicycleModel.Id))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", bicycleModel.CategoryId);
            return View(bicycleModel);
        }

        // GET: BicycleModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bicycleModel = await _context.BicycleModels
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bicycleModel == null)
            {
                return NotFound();
            }

            return View(bicycleModel);
        }

        // POST: BicycleModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bicycleModel = await _context.BicycleModels.FindAsync(id);
            if (bicycleModel != null)
            {
                _context.BicycleModels.Remove(bicycleModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BicycleModelExists(int id)
        {
            return _context.BicycleModels.Any(e => e.Id == id);
        }
    }
}
