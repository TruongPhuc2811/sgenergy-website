namespace SGENERGY.DomainModels.Sgenergy
{
    /// <summary>
    /// Ảnh của dự án
    /// </summary>
    public class ProjectPhoto
    {
        /// <summary>
        /// Mã ảnh
        /// </summary>
        public long PhotoID { get; set; }
        /// <summary>
        /// Mã dự án
        /// </summary>
        public int ProjectID { get; set; }
        /// <summary>
        /// Tên file ảnh
        /// </summary>
        public string Photo { get; set; } = string.Empty;
        /// <summary>
        /// Mô tả ảnh
        /// </summary>
        public string Description { get; set; } = string.Empty;
        /// <summary>
        /// Thứ tự hiển thị (giá trị nhỏ sẽ hiển thị trước)
        /// </summary>
        public int DisplayOrder { get; set; }
        /// <summary>
        /// Có ẩn ảnh hay không?
        /// </summary>
        public bool IsHidden { get; set; }
    }
}
