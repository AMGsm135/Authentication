using Microsoft.AspNetCore.Mvc;

namespace Amg.Authentication.Host.Controllers.MVC
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
