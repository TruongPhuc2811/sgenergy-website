using SGENERGY.DataLayers.Interfaces;
using SGENERGY.DataLayers.SQLServer;
using SGENERGY.DomainModels.Common;
using SGENERGY.DomainModels.Sgenergy;

namespace SGENERGY.BusinessLayers
{
    /// <summary>
    /// Cung cấp các chức năng xử lý dữ liệu đặc thù cho hệ thống SGENERGY,
    /// bao gồm: Dự án (Project) và Liên hệ khách hàng (ContactCustomer).
    /// </summary>
    public static class SgEnergyDataService
    {
        private static readonly IProjectRepository projectDB;
        private static readonly IContactCustomerRepository contactDB;

        static SgEnergyDataService()
        {
            projectDB = new ProjectRepository(Configuration.ConnectionString);
            contactDB = new ContactCustomerRepository(Configuration.ConnectionString);
        }

        #region Project

        /// <summary>
        /// Tìm kiếm và lấy danh sách dự án dưới dạng phân trang.
        /// </summary>
        public static async Task<PagedResult<Project>> ListProjectsAsync(ProjectSearchInput input)
        {
            return await projectDB.ListAsync(input);
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một dự án.
        /// </summary>
        public static async Task<Project?> GetProjectAsync(int projectID)
        {
            return await projectDB.GetAsync(projectID);
        }

        /// <summary>
        /// Lấy thông tin dự án theo slug (case-insensitive).
        /// </summary>
        public static async Task<Project?> GetProjectBySlugAsync(string slug)
        {
            return await projectDB.GetBySlugAsync(slug);
        }

        /// <summary>
        /// Trả về slug duy nhất cho dự án (tự động thêm hậu tố -1, -2… nếu trùng).
        /// </summary>
        public static async Task<string> GetUniqueProjectSlugAsync(string baseName, int excludeProjectID = 0)
        {
            var baseSlug = SlugHelper.GenerateSlug(baseName);
            return await SlugHelper.MakeUniqueAsync(baseSlug,
                slug => projectDB.SlugExistsAsync(slug, excludeProjectID));
        }

        /// <summary>
        /// Bổ sung một dự án mới.
        /// </summary>
        public static async Task<int> AddProjectAsync(Project data)
        {
            if (!ValidateProjectData(data, isNew: true))
                return 0;
            return await projectDB.AddAsync(data);
        }

        /// <summary>
        /// Cập nhật thông tin dự án.
        /// </summary>
        public static async Task<bool> UpdateProjectAsync(Project data)
        {
            if (!ValidateProjectData(data, isNew: false))
                return false;
            return await projectDB.UpdateAsync(data);
        }

        /// <summary>
        /// Xóa dự án (bao gồm ảnh của dự án).
        /// </summary>
        public static async Task<bool> DeleteProjectAsync(int projectID, string webRootPath)
        {
            if (await projectDB.IsUsedAsync(projectID))
                return false;

            var photos = await projectDB.ListPhotosAsync(projectID);
            var result = await projectDB.DeleteAsync(projectID);
            if (!result)
                return false;

            // Xóa file ảnh vật lý
            foreach (var photo in photos)
            {
                if (!string.IsNullOrWhiteSpace(photo.Photo) &&
                    !string.Equals(photo.Photo, "nophoto.png", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        var filePath = Path.Combine(webRootPath, "images", "projects", photo.Photo);
                        if (File.Exists(filePath))
                            File.Delete(filePath);
                    }
                    catch { }
                }
            }

            return true;
        }

        /// <summary>
        /// Kiểm tra dự án có đang được sử dụng không.
        /// </summary>
        public static async Task<bool> IsUsedProjectAsync(int projectID)
        {
            return await projectDB.IsUsedAsync(projectID);
        }

        #endregion

        #region ProjectPhoto

        /// <summary>
        /// Lấy danh sách ảnh của dự án.
        /// </summary>
        public static async Task<List<ProjectPhoto>> ListProjectPhotosAsync(int projectID)
        {
            return await projectDB.ListPhotosAsync(projectID);
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một ảnh dự án.
        /// </summary>
        public static async Task<ProjectPhoto?> GetProjectPhotoAsync(long photoID)
        {
            return await projectDB.GetPhotoAsync(photoID);
        }

        /// <summary>
        /// Bổ sung ảnh cho dự án.
        /// </summary>
        public static async Task<long> AddProjectPhotoAsync(ProjectPhoto data)
        {
            if (!ValidateProjectPhotoData(data, isNew: true))
                return 0;
            return await projectDB.AddPhotoAsync(data);
        }

        /// <summary>
        /// Cập nhật ảnh của dự án.
        /// </summary>
        public static async Task<bool> UpdateProjectPhotoAsync(ProjectPhoto data)
        {
            if (!ValidateProjectPhotoData(data, isNew: false))
                return false;
            return await projectDB.UpdatePhotoAsync(data);
        }

        /// <summary>
        /// Xóa ảnh của dự án.
        /// </summary>
        public static async Task<bool> DeleteProjectPhotoAsync(long photoID)
        {
            return await projectDB.DeletePhotoAsync(photoID);
        }

        #endregion

        #region ContactCustomer

        /// <summary>
        /// Tìm kiếm và lấy danh sách liên hệ dưới dạng phân trang.
        /// </summary>
        public static async Task<PagedResult<ContactCustomer>> ListContactsAsync(ContactSearchInput input)
        {
            return await contactDB.ListAsync(input);
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một liên hệ.
        /// </summary>
        public static async Task<ContactCustomer?> GetContactAsync(long contactID)
        {
            return await contactDB.GetAsync(contactID);
        }

        /// <summary>
        /// Bổ sung liên hệ mới (từ form liên hệ website).
        /// </summary>
        public static async Task<long> AddContactAsync(ContactCustomer data)
        {
            if (data == null || string.IsNullOrWhiteSpace(data.FullName))
                return 0;
            return await contactDB.AddAsync(data);
        }

        /// <summary>
        /// Đánh dấu đã xử lý liên hệ.
        /// </summary>
        public static async Task<bool> MarkContactHandledAsync(long contactID, string handledBy)
        {
            return await contactDB.MarkHandledAsync(contactID, handledBy);
        }

        /// <summary>
        /// Xóa liên hệ.
        /// </summary>
        public static async Task<bool> DeleteContactAsync(long contactID)
        {
            return await contactDB.DeleteAsync(contactID);
        }

        #endregion

        #region Validation

        private static bool ValidateProjectData(Project data, bool isNew)
        {
            if (data == null)
                return false;

            if (string.IsNullOrWhiteSpace(data.ProjectName))
                return false;

            if (!isNew && data.ProjectID <= 0)
                return false;

            return true;
        }

        private static bool ValidateProjectPhotoData(ProjectPhoto data, bool isNew)
        {
            if (data == null)
                return false;

            if (data.ProjectID <= 0)
                return false;

            if (string.IsNullOrWhiteSpace(data.Photo))
                return false;

            if (data.DisplayOrder <= 0)
                return false;

            if (!isNew && data.PhotoID <= 0)
                return false;

            return true;
        }

        #endregion
    }
}
