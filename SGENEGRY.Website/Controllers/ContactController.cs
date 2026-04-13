using Microsoft.AspNetCore.Mvc;

namespace SGENERGY.Website.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
