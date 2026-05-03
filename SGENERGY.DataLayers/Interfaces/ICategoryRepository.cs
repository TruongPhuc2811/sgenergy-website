using SGENERGY.DomainModels.Catalog;

namespace SGENERGY.DataLayers.Interfaces
{
    /// <summary>
    /// Định nghĩa các phép xử lý dữ liệu cho loại hàng, bổ sung truy vấn theo slug
    /// </summary>
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        /// <summary>
        /// Lấy thông tin loại hàng theo slug (case-insensitive)
        /// </summary>
        Task<Category?> GetBySlugAsync(string slug);
        /// <summary>
        /// Kiểm tra slug đã tồn tại chưa
        /// </summary>
        Task<bool> SlugExistsAsync(string slug, int excludeCategoryID = 0);
    }
}
