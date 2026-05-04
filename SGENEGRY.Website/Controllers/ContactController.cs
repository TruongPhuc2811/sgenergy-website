using Microsoft.AspNetCore.Mvc;
using SGENERGY.BusinessLayers;
using SGENERGY.DomainModels.Sgenergy;
using System.Globalization;

namespace SGENERGY.Website.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(
            string fullName, string email, string phone, string city,
            string roofArea, string monthlyBill, string region, string dayPercent)
        {
            // Format monthly bill value for display in Message
            string formattedBill = "0";
            if (long.TryParse(monthlyBill, out var billNum))
            {
                formattedBill = billNum.ToString("N0", new CultureInfo("vi-VN"));
            }

            // Build Message from the 4 solar-specific fields
            var message = $"Diện tích mái nhà: {roofArea ?? ""}m²\n" +
                          $"Hóa đơn tiền điện trung bình tháng: {formattedBill}\n" +
                          $"Khu vực sống: {region ?? ""}\n" +
                          $"Tỷ lệ sử dụng điện ban ngày: {dayPercent ?? ""}%";

            var contact = new ContactCustomer
            {
                FullName = fullName?.Trim() ?? "",
                Email = email?.Trim(),
                Phone = phone?.Trim(),
                Address = city?.Trim(),
                Subject = "Yêu cầu báo giá",
                Message = message,
                SourcePage = "/lien-he"
            };

            await SgEnergyDataService.AddContactAsync(contact);

            TempData["SubmitSuccess"] = true;
            return RedirectToAction("Index");
        }
    }
}
