using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sportur.Context;
using Sportur.Models;
using Sportur.ViewModels;
using System;
using System.Linq;

namespace Sportur.Controllers
{
    public class AccountController : Controller
    {
        private readonly SporturDbContext _context;

        public AccountController(SporturDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            CreatePasswordHash(model.Password, out string hash, out byte[] salt);

            var user = new User
            {
                Name = model.Name,
                Surname = model.Surname,
                Email = model.Email,
                PasswordHash = hash,
                PasswordSalt = salt,
                Role = model.RequestWholesale ? UserRole.Wholesale : UserRole.Retail,
                IsWholesaleApproved = !model.RequestWholesale
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            if (model.RequestWholesale)
            {
                TempData["WholesalePendingMessage"] =
                    "Заявка на оптовый доступ отправлена. Дождитесь подтверждения администратора.";

                return RedirectToAction("Login");
            }

            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserRole", user.Role.ToString());
            HttpContext.Session.SetString("UserName", user.Name);
            HttpContext.Session.SetInt32("WholesalePending", 0);

            return RedirectToAction("Index", "Catalog");
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user == null ||
                !VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
            {
                ModelState.AddModelError("", "Неверный email или пароль");
                return View();
            }

            if (user.IsBlocked)
            {
                ModelState.AddModelError("", "Ваш аккаунт заблокирован. Обратитесь к администратору.");
                return View();
            }

            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserRole", GetEffectiveRole(user).ToString());
            HttpContext.Session.SetString("UserName", user.Name);

            HttpContext.Session.SetInt32(
                "WholesalePending",
                user.Role == UserRole.Wholesale && !user.IsWholesaleApproved ? 1 : 0
            );

            if (user.Role == UserRole.Wholesale && !user.IsWholesaleApproved)
            {
                TempData["WholesalePendingMessage"] =
                    "Ваша заявка на оптовый доступ ещё рассматривается. Пока вам доступна розничная версия кабинета.";
            }

            return RedirectToAction("Index", "Catalog");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RequestWholesaleAccess()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login");

            var user = _context.Users.FirstOrDefault(u => u.Id == userId.Value);

            if (user == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login");
            }

            if (user.Role == UserRole.Retail)
            {
                user.Role = UserRole.Wholesale;
                user.IsWholesaleApproved = false;
                _context.SaveChanges();

                // Пока заявка не одобрена — пользователь остаётся в Retail
                HttpContext.Session.SetString("UserRole", UserRole.Retail.ToString());
                HttpContext.Session.SetInt32("WholesalePending", 1);

                TempData["WholesalePendingMessage"] =
                    "Заявка на оптовый доступ отправлена. Дождитесь подтверждения администратора.";
            }

            return RedirectToAction("Index", "Catalog");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Catalog");
        }

        private static UserRole GetEffectiveRole(User user)
        {
            if (user.Role == UserRole.Wholesale && !user.IsWholesaleApproved)
                return UserRole.Retail;

            return user.Role;
        }

        private static void CreatePasswordHash(string password, out string hash, out byte[] salt)
        {
            using var hmac = new System.Security.Cryptography.HMACSHA512();
            salt = hmac.Key;
            hash = Convert.ToBase64String(
                hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password))
            );
        }

        private static bool VerifyPassword(string password, string storedHash, byte[] storedSalt)
        {
            using var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt);
            var computedHash = Convert.ToBase64String(
                hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password))
            );

            return computedHash == storedHash;
        }
    }
}