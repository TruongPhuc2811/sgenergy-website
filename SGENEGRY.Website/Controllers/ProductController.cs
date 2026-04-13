using Microsoft.AspNetCore.Mvc;
using SGENERGY.Website.Models;

namespace SGENERGY.Website.Controllers;

public class ProductController : Controller
{
    public IActionResult Index()
    {
        // Ảnh demo từ network (tạm thời)
        var img = new[]
        {
            "https://images.unsplash.com/photo-1509395176047-4a66953fd231?auto=format&fit=crop&w=800&q=70",
            "https://images.unsplash.com/photo-1508514177221-188b1cf16e9d?auto=format&fit=crop&w=800&q=70",
            "https://images.unsplash.com/photo-1473341304170-971dccb5ac1e?auto=format&fit=crop&w=800&q=70",
            "https://images.unsplash.com/photo-1466611653911-95081537e5b7?auto=format&fit=crop&w=800&q=70",
            "https://images.unsplash.com/photo-1520607162513-77705c0f0d4a?auto=format&fit=crop&w=800&q=70",
            "https://images.unsplash.com/photo-1584277261846-c6a1672ed979?auto=format&fit=crop&w=800&q=70"
        };

        var vm = new ProductListVm
        {
            Title = "Tấm pin năng lượng mặt trời",
            Categories = new()
            {
                new ProductCategoryVm { Name = "Tấm pin năng lượng mặt trời", Url = "/Products?cat=solar-panel", IsActive = true },
                new ProductCategoryVm { Name = "Bộ chuyển điện - Inverter", Url = "/Products?cat=inverter" }
            },
            NewProducts = new()
            {
                new ProductCardVm { Name = "Tấm Pin Năng Lượng Mặt Trời Longi 450 WP", ImageUrl = img[4], DetailUrl = "/Products/Detail/5" },
                new ProductCardVm { Name = "Tấm Pin Năng Lượng Mặt Trời Risen 530 - 555 WP", ImageUrl = img[2], DetailUrl = "/Products/Detail/3" }
            },
            Products = new()
            {
                new ProductCardVm { Name = "Tấm Pin Năng Lượng Mặt Trời Longi 570 WP (Hi-MO 6)", ImageUrl = img[0], DetailUrl = "/Products/Detail/1", BadgeText="G5.4" },
                new ProductCardVm { Name = "Tấm Pin Năng Lượng Mặt Trời Risen 645 - 670 WP", ImageUrl = img[1], DetailUrl = "/Products/Detail/2", BadgeText="G5.6" },
                new ProductCardVm { Name = "Tấm Pin Năng Lượng Mặt Trời Risen 530 - 555 WP", ImageUrl = img[2], DetailUrl = "/Products/Detail/3", BadgeText="G5.6" },
                new ProductCardVm { Name = "Tấm Pin Năng Lượng Mặt Trời Canadian 405 WP", ImageUrl = img[3], DetailUrl = "/Products/Detail/4" },
                new ProductCardVm { Name = "Tấm Pin Năng Lượng Mặt Trời Longi 450 WP", ImageUrl = img[4], DetailUrl = "/Products/Detail/5" },
                new ProductCardVm { Name = "Tấm Pin Năng Lượng Mặt Trời Longi 445 WP", ImageUrl = img[5], DetailUrl = "/Products/Detail/6" }
            },
            Page = 1,
            TotalPages = 3
        };

        return View(vm);
    }

    // prototype detail để click "Xem chi tiết" không bị 404
    public IActionResult Detail(int id) => Content($"Product detail prototype - id={id}");
}