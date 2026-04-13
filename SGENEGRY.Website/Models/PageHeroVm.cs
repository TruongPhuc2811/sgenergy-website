namespace SGENERGY.Website.Models;

public class PageHeroVm
{
    public string Title { get; set; } = default!;
    public string? BreadcrumbText { get; set; }
    public string BackgroundImageUrl { get; set; } =
        "https://images.unsplash.com/photo-1520607162513-77705c0f0d4a?auto=format&fit=crop&w=2000&q=70";
}