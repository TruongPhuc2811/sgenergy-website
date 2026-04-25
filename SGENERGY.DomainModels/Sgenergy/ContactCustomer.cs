namespace SGENERGY.DomainModels.Sgenergy
{
    /// <summary>
    /// Thông tin liên hệ từ khách hàng
    /// </summary>
    public class ContactCustomer
    {
        /// <summary>
        /// Mã liên hệ
        /// </summary>
        public long ContactID { get; set; }
        /// <summary>
        /// Họ tên khách hàng
        /// </summary>
        public string FullName { get; set; } = string.Empty;
        /// <summary>
        /// Số điện thoại
        /// </summary>
        public string? Phone { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        public string? Email { get; set; }
        /// <summary>
        /// Tên công ty
        /// </summary>
        public string? CompanyName { get; set; }
        /// <summary>
        /// Địa chỉ
        /// </summary>
        public string? Address { get; set; }
        /// <summary>
        /// Chủ đề liên hệ
        /// </summary>
        public string? Subject { get; set; }
        /// <summary>
        /// Nội dung liên hệ
        /// </summary>
        public string? Message { get; set; }
        /// <summary>
        /// Mã sản phẩm liên quan (nếu có)
        /// </summary>
        public int? ProductID { get; set; }
        /// <summary>
        /// Mã dự án liên quan (nếu có)
        /// </summary>
        public int? ProjectID { get; set; }
        /// <summary>
        /// URL trang nguồn gửi form
        /// </summary>
        public string? SourcePage { get; set; }
        /// <summary>
        /// Đã xử lý liên hệ chưa?
        /// </summary>
        public bool IsHandled { get; set; }
        /// <summary>
        /// Người xử lý
        /// </summary>
        public string? HandledBy { get; set; }
        /// <summary>
        /// Thời điểm xử lý
        /// </summary>
        public DateTime? HandledAt { get; set; }
        /// <summary>
        /// Thời điểm tạo
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
