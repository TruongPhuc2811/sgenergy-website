using Microsoft.AspNetCore.Mvc;
using SGENERGY.BusinessLayers;
using SGENERGY.DomainModels.Catalog;

namespace SGENERGY.Website.Controllers;

public class ProductController : Controller
{
    private const int PAGE_SIZE = 12;

    /// <summary>
    /// Trang danh sách sản phẩm — hỗ trợ tìm kiếm, lọc theo loại hàng / nhà cung cấp, phân trang.
    /// URL patterns:
    ///   /san-pham                          → tất cả
    ///   /san-pham/loai-hang/{categorySlug} → lọc theo loại hàng
    ///   /san-pham/hang/{supplierSlug}      → lọc theo nhà cung cấp
    ///   ?q=...&amp;page=...               → tìm kiếm và phân trang qua query string
    /// </summary>
    public async Task<IActionResult> Index(
        string? q,
        int page = 1,
        string? categorySlug = null,
        string? supplierSlug = null)
    {
        var input = new ProductSearchInput
        {
            Page = page,
            PageSize = PAGE_SIZE,
            SearchValue = q ?? ""
        };

        // Lọc theo loại hàng
        if (!string.IsNullOrWhiteSpace(categorySlug))
        {
            var cat = await CatalogDataService.GetCategoryBySlugAsync(categorySlug);
            input.CategoryID = cat?.CategoryID ?? -1; // -1 → không tìm thấy → trả về 0 kết quả
            ViewBag.CurrentCategory = cat;
        }

        // Lọc theo nhà cung cấp
        if (!string.IsNullOrWhiteSpace(supplierSlug))
        {
            var sup = await PartnerDataService.GetSupplierBySlugAsync(supplierSlug);
            input.SupplierID = sup?.SupplierID ?? -1;
            ViewBag.CurrentSupplier = sup;
        }

        var pagedResult = await CatalogDataService.ListProductsAsync(input);

        // Danh mục và nhà cung cấp cho sidebar
        ViewBag.AllCategories = await CatalogDataService.ListAllCategoriesAsync();
        ViewBag.AllSuppliers  = await PartnerDataService.ListAllSuppliersAsync();
        ViewBag.Q             = q;
        ViewBag.CategorySlug  = categorySlug;
        ViewBag.SupplierSlug  = supplierSlug;

        return View(pagedResult);
    }

    /// <summary>
    /// Trang chi tiết sản phẩm — URL: /san-pham/{slug}
    /// </summary>
    public async Task<IActionResult> Detail(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
            return NotFound();

        var product = await CatalogDataService.GetProductBySlugAsync(slug);
        if (product == null)
            return NotFound();

        ViewBag.Photos     = await CatalogDataService.ListPhotosAsync(product.ProductID);
        ViewBag.Attributes = await CatalogDataService.ListAttributesAsync(product.ProductID);

        if (product.CategoryID.HasValue)
            ViewBag.Category = await CatalogDataService.GetCategoryAsync(product.CategoryID.Value);

        if (product.SupplierID.HasValue)
            ViewBag.Supplier = await PartnerDataService.GetSupplierAsync(product.SupplierID.Value);

        return View(product);
    }
}
