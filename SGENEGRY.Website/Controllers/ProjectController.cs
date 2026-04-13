using Microsoft.AspNetCore.Mvc;
using SGENERGY.Website.Models;

namespace SGENERGY.Website.Controllers;

public class ProjectController : Controller
{
    public IActionResult Index()
    {
        // Ảnh demo từ network (tạm thời)
        var imgs = new[]
        {
            "https://images.unsplash.com/photo-1509395176047-4a66953fd231?auto=format&fit=crop&w=1200&q=70",
            "https://images.unsplash.com/photo-1508514177221-188b1cf16e9d?auto=format&fit=crop&w=1200&q=70",
            "https://images.unsplash.com/photo-1473341304170-971dccb5ac1e?auto=format&fit=crop&w=1200&q=70",
            "https://images.unsplash.com/photo-1466611653911-95081537e5b7?auto=format&fit=crop&w=1200&q=70",
            "https://images.unsplash.com/photo-1520607162513-77705c0f0d4a?auto=format&fit=crop&w=1200&q=70",
            "https://images.unsplash.com/photo-1584277261846-c6a1672ed979?auto=format&fit=crop&w=1200&q=70"
        };

        var vm = new ProjectListVm
        {
            HeadingSmall = "CÁC DỰ ÁN NỔI BẬT MÀ DOANH NGHIỆP ĐÃ TRIỂN KHAI", // bạn sửa text theo ý
            HeadingLarge = "DỰ ÁN ĐÃ TRIỂN KHAI",
            Page = 1,
            TotalPages = 5,
            Projects = new()
            {
                new ProjectCardVm { Title = "Nhà máy MIDA Mold", ImageUrl = imgs[0], DetailUrl="/Project/Detail/1" },
                new ProjectCardVm { Title = "Nhà máy Tài Châu Duy Tân", ImageUrl = imgs[1], DetailUrl="/Project/Detail/2" },
                new ProjectCardVm { Title = "Nhà máy Ningbo Changya", ImageUrl = imgs[2], DetailUrl="/Project/Detail/3" },
                new ProjectCardVm { Title = "Nhà xưởng B15 giai đoạn 2", ImageUrl = imgs[3], DetailUrl="/Project/Detail/4" },
                new ProjectCardVm { Title = "Nhà máy Bel Ga", ImageUrl = imgs[4], DetailUrl="/Project/Detail/5" },
                new ProjectCardVm { Title = "Nhà máy First Team", ImageUrl = imgs[5], DetailUrl="/Project/Detail/6" },
                new ProjectCardVm { Title = "Nhà xưởng B15 giai đoạn 1", ImageUrl = imgs[3], DetailUrl="/Project/Detail/7" },
                new ProjectCardVm { Title = "Nhà xe Khu du lịch Đốc Lết", ImageUrl = imgs[2], DetailUrl="/Project/Detail/8" },
                new ProjectCardVm { Title = "Nhà máy Riso Sun", ImageUrl = imgs[1], DetailUrl="/Project/Detail/9" },
                new ProjectCardVm { Title = "Nhà máy Kuo Yuan", ImageUrl = imgs[4], DetailUrl="/Project/Detail/10" },
                new ProjectCardVm { Title = "Nhà công ty Tòa Hải Vân", ImageUrl = imgs[0], DetailUrl="/Project/Detail/11" },
                new ProjectCardVm { Title = "Công ty Nhựa Duy Tân", ImageUrl = imgs[5], DetailUrl="/Project/Detail/12" }
            }
        };

        return View(vm);
    }

    // prototype detail để click card không bị 404
    public IActionResult Detail(int id) => Content($"Project detail prototype - id={id}");
}