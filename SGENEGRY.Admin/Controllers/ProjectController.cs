using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGENERGY.BusinessLayers;
using SGENERGY.DomainModels.Common;
using SGENERGY.DomainModels.Sgenergy;

namespace SGENERGY.Admin.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Administrator},{WebUserRoles.DataManager}")]
    public class ProjectController : Controller
    {
        private readonly ILogger<ProjectController> _logger;
        private readonly IConfiguration _configuration;

        public ProjectController(ILogger<ProjectController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        private const string PROJECTSEARCHINPUT = "ProjectSearchInput";

        public IActionResult Index()
        {
            var input = ApplicationContext.GetSessionData<ProjectSearchInput>(PROJECTSEARCHINPUT);
            if (input == null) input = new ProjectSearchInput
            {
                Page = 1,
                PageSize = ApplicationContext.PageSize,
                SearchValue = ""
            };
            return View(input);
        }

        public async Task<IActionResult> Search(ProjectSearchInput input)
        {
            var result = await SgEnergyDataService.ListProjectsAsync(input);
            ApplicationContext.SetSessionData(PROJECTSEARCHINPUT, input);
            return View(result);
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung dự án";
            var model = new Project
            {
                ProjectID = 0,
                IsActive = true,
                DisplayOrder = 1
            };
            return View("Edit", model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Title = "Cập nhật thông tin dự án";
            var model = await SgEnergyDataService.GetProjectAsync(id);
            if (model == null)
                return RedirectToAction("Index");
            ViewBag.ProjectPhotos = await SgEnergyDataService.ListProjectPhotosAsync(id);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveData(Project data, IFormFile? uploadThumbnail)
        {
            try
            {
                ViewBag.Title = data.ProjectID == 0 ? "Bổ sung dự án" : "Cập nhật thông tin dự án";

                if (string.IsNullOrWhiteSpace(data.ProjectName))
                    ModelState.AddModelError(nameof(data.ProjectName), "Vui lòng nhập tên dự án");

                if (!ModelState.IsValid)
                    return View("Edit", data);

                if (uploadThumbnail != null)
                {
                    var fileName = await SaveProjectImageAsync(uploadThumbnail);
                    if (fileName != null)
                        data.Thumbnail = fileName;
                }

                if (string.IsNullOrEmpty(data.Thumbnail)) data.Thumbnail = "nophoto.png";

                if (data.ProjectID == 0)
                {
                    var newId = await SgEnergyDataService.AddProjectAsync(data);
                    if (newId <= 0)
                    {
                        ModelState.AddModelError(string.Empty, "Không thể bổ sung dự án. Vui lòng kiểm tra dữ liệu.");
                        return View("Edit", data);
                    }
                }
                else
                {
                    var success = await SgEnergyDataService.UpdateProjectAsync(data);
                    if (!success)
                    {
                        ModelState.AddModelError(string.Empty, "Không thể cập nhật dự án. Vui lòng kiểm tra dữ liệu.");
                        return View("Edit", data);
                    }
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SaveData thất bại. ProjectID={ProjectID}", data.ProjectID);
                ModelState.AddModelError(string.Empty, "Hệ thống đang bận hoặc dữ liệu không hợp lệ. Vui lòng thử lại sau.");
                return View("Edit", data);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (Request.Method == "POST")
            {
                await SgEnergyDataService.DeleteProjectAsync(id, ApplicationContext.WWWRootPath);
                return RedirectToAction("Index");
            }
            var model = await SgEnergyDataService.GetProjectAsync(id);
            if (model == null) return RedirectToAction("Index");
            ViewBag.AllowDelete = !await SgEnergyDataService.IsUsedProjectAsync(id);
            return View(model);
        }

        #region Project Photos

        public IActionResult CreatePhoto(int id)
        {
            ViewBag.Title = "Bổ sung hình ảnh dự án";
            var model = new ProjectPhoto
            {
                ProjectID = id,
                PhotoID = 0,
                DisplayOrder = 1
            };
            ViewBag.ProjectId = id;
            return View("EditPhoto", model);
        }

        public async Task<IActionResult> EditPhoto(int id, long photoId)
        {
            ViewBag.Title = "Cập nhật hình ảnh dự án";
            var model = await SgEnergyDataService.GetProjectPhotoAsync(photoId);
            if (model == null)
                return RedirectToAction("Edit", new { id });
            ViewBag.ProjectId = id;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveProjectPhoto(ProjectPhoto data, IFormFile? uploadPhoto)
        {
            try
            {
                ViewBag.Title = data.PhotoID == 0 ? "Bổ sung ảnh" : "Cập nhật ảnh";
                ViewBag.ProjectId = data.ProjectID;

                if (data.ProjectID <= 0)
                    ModelState.AddModelError(nameof(data.ProjectID), "Dự án không hợp lệ");
                if (string.IsNullOrWhiteSpace(data.Description))
                    ModelState.AddModelError(nameof(data.Description), "Vui lòng nhập mô tả ảnh");
                if (data.DisplayOrder <= 0)
                    ModelState.AddModelError(nameof(data.DisplayOrder), "Thứ tự hiển thị phải > 0");

                if (uploadPhoto != null)
                {
                    var fileName = await SaveProjectImageAsync(uploadPhoto);
                    if (fileName != null)
                        data.Photo = fileName;
                }

                if (string.IsNullOrWhiteSpace(data.Photo))
                    ModelState.AddModelError(nameof(data.Photo), "Vui lòng chọn ảnh");

                // Kiểm tra trùng thứ tự hiển thị
                var photos = await SgEnergyDataService.ListProjectPhotosAsync(data.ProjectID);
                var isDuplicatedOrder = photos.Any(p => p.DisplayOrder == data.DisplayOrder && p.PhotoID != data.PhotoID);
                if (isDuplicatedOrder)
                    ModelState.AddModelError(nameof(data.DisplayOrder), "Thứ tự hiển thị đã tồn tại. Vui lòng chọn số khác.");

                if (!ModelState.IsValid)
                    return View("EditPhoto", data);

                if (data.PhotoID == 0)
                {
                    var newId = await SgEnergyDataService.AddProjectPhotoAsync(data);
                    if (newId <= 0)
                    {
                        ModelState.AddModelError(string.Empty, "Không thể lưu ảnh. Vui lòng kiểm tra dữ liệu.");
                        return View("EditPhoto", data);
                    }
                }
                else
                {
                    var success = await SgEnergyDataService.UpdateProjectPhotoAsync(data);
                    if (!success)
                    {
                        ModelState.AddModelError(string.Empty, "Không thể cập nhật ảnh. Vui lòng kiểm tra dữ liệu.");
                        return View("EditPhoto", data);
                    }
                }

                return Redirect($"~/Project/Edit/{data.ProjectID}#photos");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SaveProjectPhoto thất bại. ProjectID={ProjectID}, PhotoID={PhotoID}", data.ProjectID, data.PhotoID);
                ModelState.AddModelError(string.Empty, "Hệ thống đang bận hoặc dữ liệu không hợp lệ. Vui lòng thử lại sau.");
                ViewBag.ProjectId = data.ProjectID;
                return View("EditPhoto", data);
            }
        }

        public async Task<IActionResult> DeletePhoto(int id, long photoId)
        {
            if (Request.Method == "POST")
            {
                await SgEnergyDataService.DeleteProjectPhotoAsync(photoId);
                return Redirect($"~/Project/Edit/{id}#photos");
            }
            var model = await SgEnergyDataService.GetProjectPhotoAsync(photoId);
            if (model == null) return RedirectToAction("Edit", new { id });

            var project = await SgEnergyDataService.GetProjectAsync(id);
            ViewBag.ProjectName = project?.ProjectName ?? "";
            ViewBag.AllowDelete = true;
            return View(model);
        }

        #endregion

        private async Task<string?> SaveProjectImageAsync(IFormFile uploadPhoto)
        {
            var projectImagesPath = _configuration["Storage:ProjectImagesPath"];
            if (string.IsNullOrWhiteSpace(projectImagesPath))
                return null;

            Directory.CreateDirectory(projectImagesPath);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(uploadPhoto.FileName)}";
            var filePath = Path.Combine(projectImagesPath, fileName);

            await using var stream = new FileStream(filePath, FileMode.Create);
            await uploadPhoto.CopyToAsync(stream);

            return fileName;
        }
    }
}
