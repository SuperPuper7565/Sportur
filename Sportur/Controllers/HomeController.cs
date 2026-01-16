using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Sportur.Models;

namespace Sportur.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

    }
}
