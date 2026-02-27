using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sportur.Context;
using Sportur.Models;

namespace Sportur.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminUsersController : Controller
    {
        private readonly SporturDbContext _context;

        public AdminUsersController(SporturDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Users
                .OrderBy(u => u.Id)
                .ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleBuyerType(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            if (user.Role != UserRole.Admin)
            {
                var isWholesale =
                    user.Role == UserRole.Wholesale &&
                    user.IsWholesaleApproved;

                if (isWholesale)
                {
                    user.Role = UserRole.Retail;
                    user.IsWholesaleApproved = false;
                }
                else
                {
                    user.Role = UserRole.Wholesale;
                    user.IsWholesaleApproved = true;
                }

                _context.Update(user);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}