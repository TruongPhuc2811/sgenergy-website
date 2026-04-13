using Microsoft.AspNetCore.Mvc;

namespace SGENERGY.Website.Controllers
{
    public class SolutionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
