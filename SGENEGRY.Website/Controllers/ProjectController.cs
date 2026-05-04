using Microsoft.AspNetCore.Mvc;
using SGENERGY.BusinessLayers;
using SGENERGY.DomainModels.Common;
using SGENERGY.DomainModels.Sgenergy;

namespace SGENERGY.Website.Controllers;

public class ProjectController : Controller
{
    private const int PageSize = 9;

    /// <summary>
    /// Trang danh sách dự án — URL: /du-an?page={page}
    /// </summary>
    public async Task<IActionResult> Index(int page = 1)
    {
        var input = new ProjectSearchInput
        {
            Page = page,
            PageSize = PageSize,
            IsActive = true,
            SearchValue = ""
        };

        var result = await SgEnergyDataService.ListProjectsAsync(input);
        return View(result);
    }

    /// <summary>
    /// Trang chi tiết dự án — URL: /du-an/{slug}
    /// </summary>
    public async Task<IActionResult> Detail(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
            return NotFound();

        var project = await SgEnergyDataService.GetProjectBySlugAsync(slug);
        if (project == null)
            return NotFound();

        ViewBag.Photos = await SgEnergyDataService.ListProjectPhotosAsync(project.ProjectID);

        return View(project);
    }
}
