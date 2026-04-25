using SGENERGY.DomainModels.Common;
using SGENERGY.DomainModels.Sgenergy;

namespace SGENERGY.DataLayers.Interfaces
{
    /// <summary>
    /// Định nghĩa các phép xử lý dữ liệu cho dự án
    /// </summary>
    public interface IProjectRepository
    {
        /// <summary>
        /// Tìm kiếm và lấy danh sách dự án dưới dạng phân trang
        /// </summary>
        Task<PagedResult<Project>> ListAsync(ProjectSearchInput input);
        /// <summary>
        /// Lấy thông tin 1 dự án
        /// </summary>
        Task<Project?> GetAsync(int projectID);
        /// <summary>
        /// Bổ sung dự án
        /// </summary>
        Task<int> AddAsync(Project data);
        /// <summary>
        /// Cập nhật dự án
        /// </summary>
        Task<bool> UpdateAsync(Project data);
        /// <summary>
        /// Xóa dự án
        /// </summary>
        Task<bool> DeleteAsync(int projectID);
        /// <summary>
        /// Kiểm tra dự án có dữ liệu liên quan không
        /// </summary>
        Task<bool> IsUsedAsync(int projectID);

        /// <summary>
        /// Lấy danh sách ảnh của dự án
        /// </summary>
        Task<List<ProjectPhoto>> ListPhotosAsync(int projectID);
        /// <summary>
        /// Lấy thông tin 1 ảnh của dự án
        /// </summary>
        Task<ProjectPhoto?> GetPhotoAsync(long photoID);
        /// <summary>
        /// Bổ sung ảnh dự án
        /// </summary>
        Task<long> AddPhotoAsync(ProjectPhoto data);
        /// <summary>
        /// Cập nhật ảnh dự án
        /// </summary>
        Task<bool> UpdatePhotoAsync(ProjectPhoto data);
        /// <summary>
        /// Xóa ảnh dự án
        /// </summary>
        Task<bool> DeletePhotoAsync(long photoID);
    }
}
