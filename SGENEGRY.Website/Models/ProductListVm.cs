using SGENERGY.DomainModels.Catalog;
using SGENERGY.DomainModels.Common;
using SGENERGY.DomainModels.Partner;

namespace SGENERGY.Website.Models;

/// <summary>
/// ViewModel cho trang danh sách sản phẩm (kết nối DB thực)
/// </summary>
public class ProductListPageVm
{
    public PagedResult<Product> Products { get; set; } = new();
    public List<Category> Categories { get; set; } = [];
    public List<Supplier> Suppliers { get; set; } = [];
    /// <summary>Sản phẩm nổi bật dùng cho sidebar</summary>
    public List<Product> FeaturedProducts { get; set; } = [];
    public ProductSearchInput Filter { get; set; } = new();
    public Category? ActiveCategory { get; set; }
    public Supplier? ActiveSupplier { get; set; }
}

// --- Legacy prototype models ---
public class ProductListVm
{
    public string Title { get; set; } = "Sản phẩm";
    public List<ProductCategoryVm> Categories { get; set; } = [];
    public List<ProductFilterGroupVm> FilterGroups { get; set; } = [];
    public List<ProductCardVm> NewProducts { get; set; } = [];
    public List<ProductCardVm> Products { get; set; } = [];
    public int Page { get; set; } = 1;
    public int TotalPages { get; set; } = 3;
}

public class ProductCategoryVm
{
    public string Name { get; set; } = default!;
    public string Url { get; set; } = "#";
    public bool IsActive { get; set; }
}

public class ProductFilterGroupVm
{
    public string Title { get; set; } = default!;
    public List<string> Items { get; set; } = [];
}

public class ProductCardVm
{
    public string Name { get; set; } = default!;
    public string ImageUrl { get; set; } = "/images/seed/product-placeholder.jpg";
    public string DetailUrl { get; set; } = "#";
    public string? BadgeText { get; set; }
}
