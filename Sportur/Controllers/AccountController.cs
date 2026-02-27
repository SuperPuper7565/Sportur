using Microsoft.AspNetCore.Mvc;
using Sportur.Context;
using Sportur.Models;
using Sportur.ViewModels;

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
    public IActionResult Register(RegisterViewModel model)
    {
        CreatePasswordHash(model.Password, out var hash, out var salt);

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

        return RedirectToAction("Index", "Catalog");
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public IActionResult Login(string email, string password)
    {
        var user = _context.Users.FirstOrDefault(u => u.Email == email);

        if (user == null ||
            !VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
        {
            ModelState.AddModelError("", "Неверный email или пароль");
            return View();
        }

        HttpContext.Session.SetInt32("UserId", user.Id);
        HttpContext.Session.SetString("UserRole", GetEffectiveRole(user).ToString());
        HttpContext.Session.SetString("UserName", user.Name);

        if (user.Role == UserRole.Wholesale && !user.IsWholesaleApproved)
        {
            TempData["WholesalePendingMessage"] =
                "Ваша заявка на оптовый доступ ещё рассматривается. Пока вам доступна розничная версия кабинета.";
        }

        return RedirectToAction("Index", "Catalog");
    }

    private static UserRole GetEffectiveRole(User user)
    {
        if (user.Role == UserRole.Wholesale && !user.IsWholesaleApproved)
        {
            return UserRole.Retail;
        }

        return user.Role;
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Catalog");
    }

    private static void CreatePasswordHash(string password, out string hash, out byte[] salt)
    {
        using var hmac = new System.Security.Cryptography.HMACSHA512();
        salt = hmac.Key;
        hash = Convert.ToBase64String(
            hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password))
        );
    }

    private static bool VerifyPassword(
        string password,
        string storedHash,
        byte[] storedSalt)
    {
        using var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt);
        var computedHash = Convert.ToBase64String(
            hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password))
        );

        return computedHash == storedHash;
    }
}