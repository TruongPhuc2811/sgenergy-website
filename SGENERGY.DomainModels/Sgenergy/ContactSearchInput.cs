using SGENERGY.DomainModels.Common;

namespace SGENERGY.DomainModels.Sgenergy
{
    /// <summary>
    /// Biểu diễn dữ liệu đầu vào tìm kiếm, phân trang đối với liên hệ khách hàng
    /// </summary>
    public class ContactSearchInput : PaginationSearchInput
    {
        /// <summary>
        /// Lọc theo trạng thái xử lý (null = tất cả, true = đã xử lý, false = chưa xử lý)
        /// </summary>
        public bool? IsHandled { get; set; }
    }
}
