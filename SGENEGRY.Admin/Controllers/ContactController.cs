using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGENERGY.BusinessLayers;
using SGENERGY.DomainModels.Sgenergy;

namespace SGENERGY.Admin.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Administrator},{WebUserRoles.DataManager}")]
    public class ContactController : Controller
    {
        private const string CONTACTSEARCHINPUT = "ContactSearchInput";

        public IActionResult Index()
        {
            var input = ApplicationContext.GetSessionData<ContactSearchInput>(CONTACTSEARCHINPUT);
            if (input == null) input = new ContactSearchInput
            {
                Page = 1,
                PageSize = ApplicationContext.PageSize,
                SearchValue = ""
            };
            return View(input);
        }

        public async Task<IActionResult> Search(ContactSearchInput input)
        {
            var result = await SgEnergyDataService.ListContactsAsync(input);
            ApplicationContext.SetSessionData(CONTACTSEARCHINPUT, input);
            return View(result);
        }

        public async Task<IActionResult> Detail(long id)
        {
            var model = await SgEnergyDataService.GetContactAsync(id);
            if (model == null) return RedirectToAction("Index");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkHandled(long id)
        {
            var user = User.Identity?.Name ?? "Admin";
            await SgEnergyDataService.MarkContactHandledAsync(id, user);
            return RedirectToAction("Detail", new { id });
        }

        public async Task<IActionResult> Delete(long id)
        {
            if (Request.Method == "POST")
            {
                await SgEnergyDataService.DeleteContactAsync(id);
                return RedirectToAction("Index");
            }
            var model = await SgEnergyDataService.GetContactAsync(id);
            if (model == null) return RedirectToAction("Index");
            return View(model);
        }
    }
}
