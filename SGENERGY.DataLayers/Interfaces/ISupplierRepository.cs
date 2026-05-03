using SGENERGY.DomainModels.Partner;

namespace SGENERGY.DataLayers.Interfaces
{
    /// <summary>
    /// Định nghĩa các phép xử lý dữ liệu cho nhà cung cấp, bổ sung truy vấn theo slug
    /// </summary>
    public interface ISupplierRepository : IGenericRepository<Supplier>
    {
        /// <summary>
        /// Lấy thông tin nhà cung cấp theo slug (case-insensitive)
        /// </summary>
        Task<Supplier?> GetBySlugAsync(string slug);
        /// <summary>
        /// Kiểm tra slug đã tồn tại chưa
        /// </summary>
        Task<bool> SlugExistsAsync(string slug, int excludeSupplierID = 0);
    }
}
