using SGENERGY.DomainModels.Catalog;
using SGENERGY.DomainModels.Partner;

namespace SGENERGY.Website.Models;

/// <summary>
/// ViewModel cho trang chi tiết sản phẩm
/// </summary>
public class ProductDetailVm
{
    public Product Product { get; set; } = default!;
    public List<ProductPhoto> Photos { get; set; } = [];
    public List<ProductAttribute> Attributes { get; set; } = [];
    public Category? Category { get; set; }
    public Supplier? Supplier { get; set; }
    /// <summary>Sản phẩm liên quan (cùng loại)</summary>
    public List<Product> RelatedProducts { get; set; } = [];
}
