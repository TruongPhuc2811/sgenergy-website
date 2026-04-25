namespace SGENERGY.DomainModels.Sgenergy
{
    /// <summary>
    /// Dự án
    /// </summary>
    public class Project
    {
        /// <summary>
        /// Mã dự án
        /// </summary>
        public int ProjectID { get; set; }
        /// <summary>
        /// Tên dự án
        /// </summary>
        public string ProjectName { get; set; } = string.Empty;
        /// <summary>
        /// Slug (URL thân thiện)
        /// </summary>
        public string? Slug { get; set; }
        /// <summary>
        /// Địa điểm thực hiện
        /// </summary>
        public string? Location { get; set; }
        /// <summary>
        /// Chủ đầu tư
        /// </summary>
        public string? Investor { get; set; }
        /// <summary>
        /// Mô tả quy mô
        /// </summary>
        public string? ScaleDescription { get; set; }
        /// <summary>
        /// Tóm tắt dự án
        /// </summary>
        public string? Summary { get; set; }
        /// <summary>
        /// Mô tả chi tiết dự án
        /// </summary>
        public string? DetailDescription { get; set; }
        /// <summary>
        /// Tên file ảnh đại diện của dự án (nếu có)
        /// </summary>
        public string? Thumbnail { get; set; }
        /// <summary>
        /// Thứ tự hiển thị (giá trị nhỏ sẽ hiển thị trước)
        /// </summary>
        public int DisplayOrder { get; set; }
        /// <summary>
        /// Dự án nổi bật?
        /// </summary>
        public bool IsFeatured { get; set; }
        /// <summary>
        /// Dự án đang hoạt động?
        /// </summary>
        public bool IsActive { get; set; }
    }
}
