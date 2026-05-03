namespace SGENERGY.DomainModels.Catalog
{
    /// <summary>
    /// Loại hàng
    /// </summary>
    public class Category
    {
        /// <summary>
        /// Mã loại hàng
        /// </summary>
        public int CategoryID { get; set; }
        /// <summary>
        /// Tên loại hàng
        /// </summary>
        public string CategoryName { get; set; } = string.Empty;
        /// <summary>
        /// Mô tả loại hàng
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// Slug SEO của loại hàng (ví dụ: dien-mat-troi-ap-mai)
        /// </summary>
        public string? Slug { get; set; }
    }
}
