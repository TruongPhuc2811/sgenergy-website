using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGENERGY.Admin.Models;
using SGENERGY.BusinessLayers;
using SGENERGY.DomainModels.Catalog;
using SGENERGY.DomainModels.Common;
using SGENERGY.DomainModels.Sales;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SGENERGY.Admin.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        /// <summary>
        /// Hiển thị thông tin dashboard của hệ thống
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            // lấy tất cả đơn hoàn thành để tính doanh thu
            var completedOrders = await SalesDataService.ListOrdersAsync(new OrderSearchInput() {
                Page = 1,
                PageSize = 0, 
                SearchValue = "",
                Status = OrderStatusEnum.Completed
            });
            //Doanh thu hôm nay
            ViewBag.TodayRevenue = completedOrders.DataItems.Where(o => o.FinishedTime.HasValue && o.FinishedTime.Value.Date == DateTime.Today).Sum(o => o.SumPrice);
            // Tổng đơn hàng hoàn thành
            ViewBag.CompletedOrders = completedOrders.RowCount;
            //Tổng khách hàng trên hệ thống
                var customers = await PartnerDataService.ListCustomersAsync(new PaginationSearchInput () { SearchValue = "" });
                ViewBag.TotalCustomers = customers.RowCount;
            // Tổng sản phẩm trên hệ thống
                var products = await CatalogDataService.ListProductsAsync(new ProductSearchInput() { SearchValue = "" });
                ViewBag.TotalProducts = products.RowCount;

            // Biểu đồ doanh thu theo tháng 
            var monthLabels = Enumerable.Range(1, 12).Select(m => $"Tháng {m}").ToList();
            var monthRevenues = Enumerable.Range(1, 12)
            .Select(m => completedOrders.DataItems
                .Where(o => o.FinishedTime.HasValue && o.FinishedTime.Value.Month == m &&o.FinishedTime.Value.Year == DateTime.Today.Year)
                .Sum(o => o.SumPrice))
            .ToList();

            ViewBag.MonthLabels = monthLabels;
            ViewBag.MonthRevenues = monthRevenues;

            // Top sản phẩm bán chạy
            // Lấy chi tiết của tất cả đơn hàng hoàn thành
            var detailTasks = completedOrders.DataItems
                .Select(o => SalesDataService.ListDetailsAsync(o.OrderID));
            // Chờ tất cả tác vụ hoàn thành và gộp kết quả về 1 danh sách chung
            var allDetails = (await Task.WhenAll(detailTasks)).SelectMany(x => x);
            // Nhóm theo tên sản phẩm, tính tổng số lượng đã bán, sắp xếp giảm dần và lấy 5 sản phẩm đầu tiên
            var topProducts = allDetails
                .GroupBy(d => d.ProductName)
                .Select(g => new KeyValuePair<string, int>(g.Key, g.Sum(x => x.Quantity)))
                .OrderByDescending(x => x.Value)
                .Take(5)
                .ToList();
            ViewBag.TopProducts = topProducts;
            // Hiển thị danh sách đơn hàng cần xử lý trạng thái New
            var input = new OrderSearchInput()
            {
                Page = 1,
                PageSize = ApplicationContext.PageSize,
                SearchValue = "",
                Status = OrderStatusEnum.New,
                DateFrom = null,
                DateTo = null
            };
            var model = await SalesDataService.ListOrdersAsync(input);
            if (model == null) return RedirectToAction("Index");
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
