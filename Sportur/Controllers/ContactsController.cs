using Microsoft.AspNetCore.Mvc;

namespace Sportur.Controllers
{
    public class ContactsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
