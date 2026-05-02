using Microsoft.AspNetCore.Mvc;
using SGENERGY.BusinessLayers;
using SGENERGY.DomainModels.Catalog;
using SGENERGY.Website.Models;

namespace SGENERGY.Website.Controllers;

public class ProductController : Controller
{
    private const int PAGE_SIZE = 12;

    /// <summary>
    /// Trang danh sach san pham: /san-pham hoac /Product/Index
    /// Query strings: searchValue, categoryId, supplierId, page
    /// </summary>
    public async Task<IActionResult> Index(
        string? searchValue,
        int categoryId = 0,
        int supplierId = 0,
        int page = 1)
    {
        var input = new ProductSearchInput
        {
            SearchValue = searchValue ?? "",
            CategoryID  = categoryId,
            SupplierID  = supplierId,
            Page        = page,
            PageSize    = PAGE_SIZE,
            IsActive    = true   // website chi hien san pham dang ban
        };

        // Load data in parallel
        var productsTask   = CatalogDataService.ListProductsAsync(input);
        var categoriesTask = CatalogDataService.ListAllCategoriesAsync();
        var suppliersTask  = PartnerDataService.ListSuppliersAsync(new SGENERGY.DomainModels.Common.PaginationSearchInput { PageSize = 0 });

        // Featured: top 4 products from same category (or overall if no filter)
        var featuredInput = new ProductSearchInput
        {
            PageSize = 4,
            Page     = 1,
            IsActive = true,
            CategoryID = categoryId > 0 ? categoryId : 0
        };
        var featuredTask = CatalogDataService.ListProductsAsync(featuredInput);

        await Task.WhenAll(productsTask, categoriesTask, suppliersTask, featuredTask);

        var vm = new ProductListPageVm
        {
            Products         = productsTask.Result,
            Categories       = categoriesTask.Result,
            Suppliers        = suppliersTask.Result.DataItems,
            FeaturedProducts = featuredTask.Result.DataItems,
            Filter           = input,
            ActiveCategory   = categoryId > 0
                ? categoriesTask.Result.FirstOrDefault(c => c.CategoryID == categoryId)
                : null,
            ActiveSupplier   = supplierId > 0
                ? suppliersTask.Result.DataItems.FirstOrDefault(s => s.SupplierID == supplierId)
                : null
        };

        return View(vm);
    }

    /// <summary>
    /// Trang chi tiet san pham: /san-pham/{slug}
    /// </summary>
    public async Task<IActionResult> Detail(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
            return NotFound();

        var product = await CatalogDataService.GetProductBySlugAsync(slug);
        if (product == null)
            return NotFound();

        // Load detail data in parallel
        var photosTask     = CatalogDataService.ListPhotosAsync(product.ProductID);
        var attributesTask = CatalogDataService.ListAttributesAsync(product.ProductID);
        var categoryTask   = product.CategoryID.HasValue
            ? CatalogDataService.GetCategoryAsync(product.CategoryID.Value)
            : Task.FromResult<SGENERGY.DomainModels.Catalog.Category?>(null);
        var supplierTask   = product.SupplierID.HasValue
            ? PartnerDataService.GetSupplierAsync(product.SupplierID.Value)
            : Task.FromResult<SGENERGY.DomainModels.Partner.Supplier?>(null);

        // Related products: same category, excluding current
        var relatedInput = new ProductSearchInput
        {
            PageSize   = 4,
            Page       = 1,
            IsActive   = true,
            CategoryID = product.CategoryID ?? 0
        };
        var relatedTask = CatalogDataService.ListProductsAsync(relatedInput);

        await Task.WhenAll(photosTask, attributesTask, categoryTask, supplierTask, relatedTask);

        var vm = new ProductDetailVm
        {
            Product         = product,
            Photos          = photosTask.Result,
            Attributes      = attributesTask.Result,
            Category        = categoryTask.Result,
            Supplier        = supplierTask.Result,
            RelatedProducts = relatedTask.Result.DataItems
                .Where(p => p.ProductID != product.ProductID)
                .ToList()
        };

        return View(vm);
    }
}
