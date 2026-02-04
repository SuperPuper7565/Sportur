using Microsoft.AspNetCore.Mvc;
using Sportur.Context;

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
    public IActionResult Register(string name, string surname, string email, string password)
    {
        if (_context.Users.Any(u => u.Email == email))
        {
            ModelState.AddModelError("", "Пользователь с таким email уже существует");
            return View();
        }

        CreatePasswordHash(password, out string hash, out byte[] salt);

        var user = new User
        {
            Name = name,
            Surname = surname,
            Email = email,
            PasswordHash = hash,
            PasswordSalt = salt,
            Role = UserRole.Retail // по умолчанию
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        // логиним
        HttpContext.Session.SetInt32("UserId", user.Id);
        HttpContext.Session.SetString("UserRole", user.Role.ToString());

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
        HttpContext.Session.SetString("UserRole", user.Role.ToString());

        return RedirectToAction("Index", "Catalog");
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
