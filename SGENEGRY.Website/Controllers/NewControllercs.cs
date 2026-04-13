using Microsoft.AspNetCore.Mvc;

namespace SGENERGY.Website.Controllers
{
    public class NewControllercs : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
