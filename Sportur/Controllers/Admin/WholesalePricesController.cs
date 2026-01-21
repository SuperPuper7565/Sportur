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
    public class WholesalePricesController : Controller
    {
        private readonly SporturDbContext _context;

        public WholesalePricesController(SporturDbContext context)
        {
            _context = context;
        }

        // GET: WholesalePrices
        public async Task<IActionResult> Index()
        {
            var sporturDbContext = _context.WholesalePrices.Include(w => w.BicycleSize).Include(w => w.User);
            return View(await sporturDbContext.ToListAsync());
        }

        // GET: WholesalePrices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wholesalePrice = await _context.WholesalePrices
                .Include(w => w.BicycleSize)
                .Include(w => w.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (wholesalePrice == null)
            {
                return NotFound();
            }

            return View(wholesalePrice);
        }

        // GET: WholesalePrices/Create
        public IActionResult Create()
        {
            ViewData["BicycleSizeId"] = new SelectList(_context.BicycleSizes, "Id", "FrameSize");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Name");
            return View();
        }

        // POST: WholesalePrices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BicycleSizeId,UserId,Price")] WholesalePrice wholesalePrice)
        {
            if (ModelState.IsValid)
            {
                _context.Add(wholesalePrice);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BicycleSizeId"] = new SelectList(_context.BicycleSizes, "Id", "FrameSize", wholesalePrice.BicycleSizeId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Name", wholesalePrice.UserId);
            return View(wholesalePrice);
        }

        // GET: WholesalePrices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wholesalePrice = await _context.WholesalePrices.FindAsync(id);
            if (wholesalePrice == null)
            {
                return NotFound();
            }
            ViewData["BicycleSizeId"] = new SelectList(_context.BicycleSizes, "Id", "FrameSize", wholesalePrice.BicycleSizeId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Name", wholesalePrice.UserId);
            return View(wholesalePrice);
        }

        // POST: WholesalePrices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BicycleSizeId,UserId,Price")] WholesalePrice wholesalePrice)
        {
            if (id != wholesalePrice.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(wholesalePrice);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WholesalePriceExists(wholesalePrice.Id))
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
            ViewData["BicycleSizeId"] = new SelectList(_context.BicycleSizes, "Id", "FrameSize", wholesalePrice.BicycleSizeId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Name", wholesalePrice.UserId);
            return View(wholesalePrice);
        }

        // GET: WholesalePrices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wholesalePrice = await _context.WholesalePrices
                .Include(w => w.BicycleSize)
                .Include(w => w.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (wholesalePrice == null)
            {
                return NotFound();
            }

            return View(wholesalePrice);
        }

        // POST: WholesalePrices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var wholesalePrice = await _context.WholesalePrices.FindAsync(id);
            if (wholesalePrice != null)
            {
                _context.WholesalePrices.Remove(wholesalePrice);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WholesalePriceExists(int id)
        {
            return _context.WholesalePrices.Any(e => e.Id == id);
        }
    }
}
