using SGENERGY.DomainModels.Common;
using SGENERGY.DomainModels.Sgenergy;

namespace SGENERGY.DataLayers.Interfaces
{
    /// <summary>
    /// Định nghĩa các phép xử lý dữ liệu cho liên hệ khách hàng
    /// </summary>
    public interface IContactCustomerRepository
    {
        /// <summary>
        /// Tìm kiếm và lấy danh sách liên hệ dưới dạng phân trang
        /// </summary>
        Task<PagedResult<ContactCustomer>> ListAsync(ContactSearchInput input);
        /// <summary>
        /// Lấy thông tin 1 liên hệ
        /// </summary>
        Task<ContactCustomer?> GetAsync(long contactID);
        /// <summary>
        /// Bổ sung liên hệ mới
        /// </summary>
        Task<long> AddAsync(ContactCustomer data);
        /// <summary>
        /// Đánh dấu đã xử lý liên hệ
        /// </summary>
        Task<bool> MarkHandledAsync(long contactID, string handledBy);
        /// <summary>
        /// Xóa liên hệ
        /// </summary>
        Task<bool> DeleteAsync(long contactID);
    }
}
