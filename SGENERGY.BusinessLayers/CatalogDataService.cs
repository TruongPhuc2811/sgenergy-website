using SGENERGY.DataLayers.Interfaces;
using SGENERGY.DataLayers.SQLServer;
using SGENERGY.DomainModels.Catalog;
using SGENERGY.DomainModels.Common;

namespace SGENERGY.BusinessLayers
{
    /// <summary>
    /// Cung cấp các chức năng xử lý dữ liệu liên quan đến danh mục hàng hóa của hệ thống, 
    /// bao gồm: Loại hàng (Cagegory), mặt hàng (Product), thuộc tính của mặt hàng (ProductAttribute) và ảnh của mặt hàng (ProductPhoto).
    /// </summary>
    public static class CatalogDataService
    {
        private static readonly IProductRepository productDB;
        private static readonly IGenericRepository<Category> categoryDB;

        /// <summary>
        /// Constructor
        /// </summary>
        static CatalogDataService()
        {
            categoryDB = new CategoryRepository(Configuration.ConnectionString);
            productDB = new ProductRepository(Configuration.ConnectionString);
        }

        #region Category

        /// <summary>
        /// Tìm kiếm và lấy danh sách loại hàng dưới dạng phân trang.
        /// </summary>
        public static async Task<PagedResult<Category>> ListCategoriesAsync(PaginationSearchInput input)
        {
            return await categoryDB.ListAsync(input);
        }

        /// <summary>
        /// Lấy tất cả loại hàng đang hoạt động (không phân trang) — dùng cho bộ lọc sidebar.
        /// </summary>
        public static async Task<List<Category>> ListAllCategoriesAsync()
        {
            var result = await categoryDB.ListAsync(new PaginationSearchInput { PageSize = 0 });
            return result.DataItems;
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một loại hàng dựa vào mã loại hàng.
        /// </summary>
        public static async Task<Category?> GetCategoryAsync(int CategoryID)
        {
            return await categoryDB.GetAsync(CategoryID);
        }

        /// <summary>
        /// Bổ sung một loại hàng mới vào hệ thống.
        /// </summary>
        public static async Task<int> AddCategoryAsync(Category data)
        {
            if (!ValidateCategoryData(data, true))
                return 0;
            return await categoryDB.AddAsync(data);
        }

        /// <summary>
        /// Cập nhật thông tin của một loại hàng.
        /// </summary>
        public static async Task<bool> UpdateCategoryAsync(Category data)
        {
            if (!ValidateCategoryData(data, false))
                return false;
            return await categoryDB.UpdateAsync(data);
        }

        /// <summary>
        /// Xóa một loại hàng dựa vào mã loại hàng.
        /// </summary>
        public static async Task<bool> DeleteCategoryAsync(int CategoryID)
        {
            if (await categoryDB.IsUsedAsync(CategoryID))
                return false;

            return await categoryDB.DeleteAsync(CategoryID);
        }

        /// <summary>
        /// Kiểm tra xem một loại hàng có đang được sử dụng trong dữ liệu hay không.
        /// </summary>
        public static async Task<bool> IsUsedCategoryAsync(int CategoryID)
        {
            return await categoryDB.IsUsedAsync(CategoryID);
        }

        #endregion

        #region Product

        /// <summary>
        /// Tìm kiếm và lấy danh sách mặt hàng dưới dạng phân trang.
        /// </summary>
        public static async Task<PagedResult<Product>> ListProductsAsync(ProductSearchInput input)
        {
            return await productDB.ListAsync(input);
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một mặt hàng.
        /// </summary>
        public static async Task<Product?> GetProductAsync(int productID)
        {
            return await productDB.GetAsync(productID);
        }

        /// <summary>
        /// Lấy thông tin mặt hàng theo slug — dùng cho đường dẫn /san-pham/{slug}.
        /// </summary>
        public static async Task<Product?> GetProductBySlugAsync(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
                return null;
            return await productDB.GetBySlugAsync(slug);
        }

        /// <summary>
        /// Bổ sung một mặt hàng mới vào hệ thống.
        /// </summary>
        public static async Task<int> AddProductAsync(Product data)
        {
            if (!ValidateProductData(data, true))
                return 0;
            return await productDB.AddAsync(data);
        }

        /// <summary>
        /// Cập nhật thông tin của một mặt hàng.
        /// </summary>
        public static async Task<bool> UpdateProductAsync(Product data)
        {
               if (!ValidateProductData(data, false))
                return false;
            return await productDB.UpdateAsync(data);
        }

        /// <summary>
        /// Xóa một mặt hàng dựa vào mã mặt hàng.
        /// </summary>
        public static async Task<bool> DeleteProductAsync(int productID, string webRootPath)
        {
            if (await productDB.IsUsedAsync(productID))
                return false;
            var item = await productDB.GetAsync(productID);
            var photo = item?.Photo;
            var result = await productDB.DeleteAsync(productID);
            if (!result)
                return false;

            if (!string.IsNullOrWhiteSpace(photo) && !string.Equals(photo, "nophoto.png", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var filePath = Path.Combine(webRootPath, "images", "products", photo);
                    if (File.Exists(filePath))
                        File.Delete(filePath);
                }
                catch
                {
                }
            }
            return true;
        }

        /// <summary>
        /// Kiểm tra xem một mặt hàng có đang được sử dụng trong dữ liệu hay không.
        /// </summary>
        public static async Task<bool> IsUsedProductAsync(int productID)
        {
            return await productDB.IsUsedAsync(productID);
        }

        #endregion

        #region ProductAttribute

        /// <summary>
        /// Lấy danh sách các thuộc tính của một mặt hàng.
        /// </summary>
        public static async Task<List<ProductAttribute>> ListAttributesAsync(int productID)
        {
            return await productDB.ListAttributesAsync(productID);
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một thuộc tính của mặt hàng.
        /// </summary>
        public static async Task<ProductAttribute?> GetAttributeAsync(long attributeID)
        {
            return await productDB.GetAttributeAsync(attributeID);
        }

        /// <summary>
        /// Bổ sung một thuộc tính mới cho mặt hàng.
        /// </summary>
        public static async Task<long> AddAttributeAsync(ProductAttribute data)
        {
            if (!ValidateProductAttributeData(data, true))
                return 0;
            return await productDB.AddAttributeAsync(data);
        }

        /// <summary>
        /// Cập nhật thông tin của một thuộc tính mặt hàng.
        /// </summary>
        public static async Task<bool> UpdateAttributeAsync(ProductAttribute data)
        {
            if (!ValidateProductAttributeData(data, false))
                return false;
            return await productDB.UpdateAttributeAsync(data);
        }

        /// <summary>
        /// Xóa một thuộc tính của mặt hàng.
        /// </summary>
        public static async Task<bool> DeleteAttributeAsync(long attributeID)
        {
            return await productDB.DeleteAttributeAsync(attributeID);
        }

        #endregion

        #region ProductPhoto

        /// <summary>
        /// Lấy danh sách ảnh của một mặt hàng.
        /// </summary>
        public static async Task<List<ProductPhoto>> ListPhotosAsync(int productID)
        {
            return await productDB.ListPhotosAsync(productID);
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một ảnh của mặt hàng.
        /// </summary>
        public static async Task<ProductPhoto?> GetPhotoAsync(long photoID)
        {
            return await productDB.GetPhotoAsync(photoID);
        }

        /// <summary>
        /// Bổ sung một ảnh mới cho mặt hàng.
        /// </summary>
        public static async Task<long> AddPhotoAsync(ProductPhoto data)
        {
            if (!ValidateProductPhotoData(data, true))
                return 0;
            return await productDB.AddPhotoAsync(data);
        }

        /// <summary>
        /// Cập nhật thông tin của một ảnh mặt hàng.
        /// </summary>
        public static async Task<bool> UpdatePhotoAsync(ProductPhoto data)
        {
            if (!ValidateProductPhotoData(data, false))
                return false;
            return await productDB.UpdatePhotoAsync(data);
        }

        /// <summary>
        /// Xóa một ảnh của mặt hàng.
        /// </summary>
        public static async Task<bool> DeletePhotoAsync(long photoID)
        {
            return await productDB.DeletePhotoAsync(photoID);
        }

        #endregion

        #region Validation
        private static bool ValidateCategoryData(Category data, bool isNew)
        {
            if (data == null)
                return false;

            if (string.IsNullOrWhiteSpace(data.CategoryName))
                return false;

            if (!isNew && data.CategoryID <= 0)
                return false;

            return true;
        }

        private static bool ValidateProductData(Product data, bool isNew)
        {
            if (data == null)
                return false;

            if (string.IsNullOrWhiteSpace(data.ProductName))
                return false;

            if (string.IsNullOrWhiteSpace(data.Unit))
                return false;

            if (data.Price <= 0)
                return false;

            if (data.SupplierID.HasValue && data.SupplierID <= 0)
                return false;

            if (data.CategoryID.HasValue && data.CategoryID <= 0)
                return false;
        
            if (!isNew && data.ProductID <= 0)
                return false;

            return true;
        }

        private static bool ValidateProductAttributeData(ProductAttribute data, bool isNew)
        {
            if (data == null)
                return false;

            if (data.ProductID <= 0)
                return false;

            if (string.IsNullOrWhiteSpace(data.AttributeName))
                return false;

            if (string.IsNullOrWhiteSpace(data.AttributeValue))
                return false;


            if (!isNew && data.AttributeID <= 0)
                return false;

            return true;
        }

        private static bool ValidateProductPhotoData(ProductPhoto data, bool isNew)
        {
            if (data == null)
                return false;

            if (data.ProductID <= 0)
                return false;

            if (string.IsNullOrWhiteSpace(data.Photo))
                return false;

            if (string.IsNullOrWhiteSpace(data.Description))
                return false;

            if (data.DisplayOrder < 0)
                return false;

            if (!isNew && data.PhotoID <= 0)
                return false;

            return true;
        }

        #endregion
  
    }
}
