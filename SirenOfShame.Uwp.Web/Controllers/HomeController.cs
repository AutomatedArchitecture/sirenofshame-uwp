using Microsoft.AspNet.Mvc;

namespace SirenOfShame.Uwp.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return File("~/index.html", "text/html");
        }

        public IActionResult Vendors()
        {
            return File("~/dist/vendors.min.js", "text/html");
        }

        public IActionResult Boot()
        {
            return File("~/dist/boot.min.js", "text/html");
        }

        public IActionResult App()
        {
            return File("~/dist/app.min.js", "text/html");
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
