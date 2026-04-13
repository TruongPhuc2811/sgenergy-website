using Microsoft.AspNetCore.Mvc;

namespace SGENERGY.Website.Controllers
{
    public class ServiceController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
