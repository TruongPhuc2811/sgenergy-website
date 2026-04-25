using SGENERGY.DomainModels.Common;

namespace SGENERGY.DomainModels.Sgenergy
{
    /// <summary>
    /// Biểu diễn dữ liệu đầu vào tìm kiếm, phân trang đối với dự án
    /// </summary>
    public class ProjectSearchInput : PaginationSearchInput
    {
        /// <summary>
        /// Lọc theo trạng thái hoạt động (null = tất cả)
        /// </summary>
        public bool? IsActive { get; set; }
    }
}
