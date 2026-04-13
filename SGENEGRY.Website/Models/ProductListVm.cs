namespace SGENERGY.Website.Models;

public class ProductListVm
{
    public string Title { get; set; } = "Sản phẩm";
    public List<ProductCategoryVm> Categories { get; set; } = [];
    public List<ProductFilterGroupVm> FilterGroups { get; set; } = [];
    public List<ProductCardVm> NewProducts { get; set; } = [];
    public List<ProductCardVm> Products { get; set; } = [];

    // paging prototype
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
    public string? BadgeText { get; set; }   // ví dụ “G5.6”
}