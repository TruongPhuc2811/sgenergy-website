using Dapper;
using Microsoft.Data.SqlClient;
using SGENERGY.DataLayers.Interfaces;
using SGENERGY.DomainModels.Catalog;
using SGENERGY.DomainModels.Common;

namespace SGENERGY.DataLayers.SQLServer
{
    /// <summary>
    /// Cài đặt các phép xử lý dữ liệu cho mặt hàng sử dụng SQL Server
    /// </summary>
    public class ProductRepository : IProductRepository
    {
        private readonly string _connectionString;

        public ProductRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Lấy danh sách mặt hàng dưới dạng phân trang, có thể tìm kiếm và lọc theo các tiêu chí khác nhau
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResult<Product>> ListAsync(ProductSearchInput input)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var searchParam = string.IsNullOrWhiteSpace(input.SearchValue)
                ? ""
                : $"%{input.SearchValue}%";

            string countSql = @"
        SELECT COUNT(*) FROM Products
        WHERE (@SearchValue = N'' OR ProductName LIKE @SearchValue)
          AND (@CategoryID = 0  OR CategoryID = @CategoryID)
          AND (@SupplierID = 0  OR SupplierID = @SupplierID)
          AND (@MinPrice = 0    OR Price >= @MinPrice)
          AND (@MaxPrice = 0    OR Price <= @MaxPrice)";

            string dataSql = input.PageSize > 0
                ? @"
            SELECT * FROM Products
            WHERE (@SearchValue = N'' OR ProductName LIKE @SearchValue)
              AND (@CategoryID = 0  OR CategoryID = @CategoryID)
              AND (@SupplierID = 0  OR SupplierID = @SupplierID)
              AND (@MinPrice = 0    OR Price >= @MinPrice)
              AND (@MaxPrice = 0    OR Price <= @MaxPrice)
            ORDER BY ProductName
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY"
                : @"
            SELECT * FROM Products
            WHERE (@SearchValue = N'' OR ProductName LIKE @SearchValue)
              AND (@CategoryID = 0  OR CategoryID = @CategoryID)
              AND (@SupplierID = 0  OR SupplierID = @SupplierID)
              AND (@MinPrice = 0    OR Price >= @MinPrice)
              AND (@MaxPrice = 0    OR Price <= @MaxPrice)
            ORDER BY ProductName";

            var param = new
            {
                SearchValue = searchParam,
                input.CategoryID,
                input.SupplierID,
                input.MinPrice,
                input.MaxPrice,
                Offset = input.Offset,
                input.PageSize
            };

            int rowCount = await connection.ExecuteScalarAsync<int>(countSql, param);
            var dataItems = (await connection.QueryAsync<Product>(dataSql, param)).ToList();

            return new PagedResult<Product>
            {
                Page = input.Page,
                PageSize = input.PageSize,
                RowCount = rowCount,
                DataItems = dataItems
            };
        }
        /// <summary>
        /// Lấy thông tin của một mặt hàng
        /// </summary>
        /// <param name="productID">Mã mặt hàng</param>
        /// <returns></returns>
        public async Task<Product?> GetAsync(int productID)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string sql = "SELECT * FROM Products WHERE ProductID = @ProductID";
            return await connection.QueryFirstOrDefaultAsync<Product>(sql, new { ProductID = productID });
        }
        /// <summary>
        /// Lấy thông tin mặt hàng theo slug (case-insensitive)
        /// </summary>
        /// <param name="slug">Slug SEO</param>
        /// <returns></returns>
        public async Task<Product?> GetBySlugAsync(string slug)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string sql = "SELECT * FROM Products WHERE LOWER(Slug) = LOWER(@Slug)";
            return await connection.QueryFirstOrDefaultAsync<Product>(sql, new { Slug = slug });
        }
        /// <summary>
        /// Kiểm tra slug đã tồn tại chưa
        /// </summary>
        public async Task<bool> SlugExistsAsync(string slug, int excludeProductID = 0)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string sql = @"SELECT COUNT(*) FROM Products
                           WHERE LOWER(Slug) = LOWER(@Slug)
                             AND ProductID <> @ExcludeID";
            int count = await connection.ExecuteScalarAsync<int>(sql, new { Slug = slug, ExcludeID = excludeProductID });
            return count > 0;
        }
        /// <summary>
        /// Thêm một mặt hàng mới vào cơ sở dữ liệu, trả về mã mặt hàng vừa tạo
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> AddAsync(Product data)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string sql = @"
                INSERT INTO Products(ProductName, ProductDescription, SupplierID, CategoryID, Unit, Price, Photo, IsSelling, Slug)
                VALUES (@ProductName, @ProductDescription, @SupplierID, @CategoryID, @Unit, @Price, @Photo, @IsSelling, @Slug);
                SELECT SCOPE_IDENTITY();";

            return await connection.ExecuteScalarAsync<int>(sql, new
            {
                data.ProductName,
                data.ProductDescription,
                data.SupplierID,
                data.CategoryID,
                data.Unit,
                data.Price,
                data.Photo,
                data.IsSelling,
                data.Slug
            });
        }
        /// <summary>
        /// Cập nhật một mặt hàng đã tồn tại trong cơ sở dữ liệu,
        /// trả về true nếu cập nhật thành công, ngược lại trả về false
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync(Product data)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string sql = @"
                UPDATE Products
                SET ProductName        = @ProductName,
                    ProductDescription = @ProductDescription,
                    SupplierID         = @SupplierID,
                    CategoryID         = @CategoryID,
                    Unit               = @Unit,
                    Price              = @Price,
                    Photo              = @Photo,
                    IsSelling          = @IsSelling,
                    Slug               = @Slug
                WHERE ProductID = @ProductID";

            int rowsAffected = await connection.ExecuteAsync(sql, new
            {
                data.ProductName,
                data.ProductDescription,
                data.SupplierID,
                data.CategoryID,
                data.Unit,
                data.Price,
                data.Photo,
                data.IsSelling,
                data.Slug,
                data.ProductID
            });

            return rowsAffected > 0;
        }
        /// <summary>
        /// Xóa sản phẩm khỏi cơ sở dữ liệu, trả về true nếu xóa thành công, ngược lại trả về false.
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(int productID)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            // Xóa ảnh và thuộc tính liên quan trước, sau đó xóa mặt hàng
            string sql = @"
                DELETE FROM ProductPhotos     WHERE ProductID = @ProductID;
                DELETE FROM ProductAttributes WHERE ProductID = @ProductID;
                DELETE FROM Products          WHERE ProductID = @ProductID;";

            int rowsAffected = await connection.ExecuteAsync(sql, new { ProductID = productID });
            return rowsAffected > 0;
        }
        /// <summary>
        /// Kiểm tra sản phẩm có đang được sử dụng trong chi tiết đơn hàng hay không,
        /// trả về true nếu đang được sử dụng, ngược lại trả về false.
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public async Task<bool> IsUsedAsync(int productID)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string sql = "SELECT COUNT(*) FROM OrderDetails WHERE ProductID = @ProductID";
            int count = await connection.ExecuteScalarAsync<int>(sql, new { ProductID = productID });
            return count > 0;
        }

        #region Thuộc tính 
        /// <summary>
        /// Trả về danh sách các thuộc tính của một mặt hàng,
        /// được sắp xếp theo thứ tự hiển thị (DisplayOrder)
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public async Task<List<ProductAttribute>> ListAttributesAsync(int productID)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string sql = @"
                SELECT * FROM ProductAttributes
                WHERE ProductID = @ProductID
                ORDER BY DisplayOrder";

            return (await connection.QueryAsync<ProductAttribute>(sql, new { ProductID = productID })).ToList();
        }
        /// <summary>
        /// Trả về thông tin của một thuộc tính dựa vào mã thuộc tính (AttributeID)
        /// </summary>
        /// <param name="attributeID"></param>
        /// <returns></returns>
        public async Task<ProductAttribute?> GetAttributeAsync(long attributeID)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string sql = "SELECT * FROM ProductAttributes WHERE AttributeID = @AttributeID";
            return await connection.QueryFirstOrDefaultAsync<ProductAttribute>(sql, new { AttributeID = attributeID });
        }
        /// <summary>
        /// Thêm mới một thuộc tính cho mặt hàng, trả về mã thuộc tính vừa tạo
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<long> AddAttributeAsync(ProductAttribute data)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string sql = @"
                INSERT INTO ProductAttributes(ProductID, AttributeName, AttributeValue, DisplayOrder)
                VALUES (@ProductID, @AttributeName, @AttributeValue, @DisplayOrder);
                SELECT SCOPE_IDENTITY();";

            return await connection.ExecuteScalarAsync<long>(sql, new
            {
                data.ProductID,
                data.AttributeName,
                data.AttributeValue,
                data.DisplayOrder
            });
        }
        /// <summary>
        /// Cập nhật một thuộc tính đã tồn tại trong cơ sở dữ liệu,
        /// trả về true nếu cập nhật thành công, ngược lại trả về false
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<bool> UpdateAttributeAsync(ProductAttribute data)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string sql = @"
                UPDATE ProductAttributes
                SET AttributeName  = @AttributeName,
                    AttributeValue = @AttributeValue,
                    DisplayOrder   = @DisplayOrder
                WHERE AttributeID = @AttributeID";

            int rowsAffected = await connection.ExecuteAsync(sql, new
            {
                data.AttributeName,
                data.AttributeValue,
                data.DisplayOrder,
                data.AttributeID
            });

            return rowsAffected > 0;
        }
        /// <summary>
        /// Xóa thuộc tính khỏi cơ sở dữ liệu 
        /// trả về true nếu xóa thành công, ngược lại trả về false.
        /// </summary>
        /// <param name="attributeID"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAttributeAsync(long attributeID)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string sql = "DELETE FROM ProductAttributes WHERE AttributeID = @AttributeID";
            int rowsAffected = await connection.ExecuteAsync(sql, new { AttributeID = attributeID });
            return rowsAffected > 0;
        }

        #endregion

        #region Ảnh
        /// <summary>
        /// Danh sách ảnh của mặt hàng,
        /// được sắp xếp theo thứ tự hiển thị (DisplayOrder)
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public async Task<List<ProductPhoto>> ListPhotosAsync(int productID)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string sql = @"
                SELECT * FROM ProductPhotos
                WHERE ProductID = @ProductID
                ORDER BY DisplayOrder";

            return (await connection.QueryAsync<ProductPhoto>(sql, new { ProductID = productID })).ToList();
        }
        /// <summary>
        /// Lấy thông tin của một ảnh dựa vào mã ảnh (PhotoID)
        /// </summary>
        /// <param name="photoID"></param>
        /// <returns></returns>
        public async Task<ProductPhoto?> GetPhotoAsync(long photoID)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string sql = "SELECT * FROM ProductPhotos WHERE PhotoID = @PhotoID";
            return await connection.QueryFirstOrDefaultAsync<ProductPhoto>(sql, new { PhotoID = photoID });
        }
        /// <summary>
        /// Thêm thông tin ảnh mới vào danh sách ảnh của mặt hàng, trả về mã ảnh vừa tạo
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<long> AddPhotoAsync(ProductPhoto data)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string sql = @"
                INSERT INTO ProductPhotos(ProductID, Photo, Description, DisplayOrder, IsHidden)
                VALUES (@ProductID, @Photo, @Description, @DisplayOrder, @IsHidden);
                SELECT SCOPE_IDENTITY();";

            return await connection.ExecuteScalarAsync<long>(sql, new
            {
                data.ProductID,
                data.Photo,
                data.Description,
                data.DisplayOrder,
                data.IsHidden
            });
        }
        /// <summary>
        ///  Cập nhật thông tin của ảnh đã tồn tại trong cơ sở dữ liệu,
        ///  trả về true nếu cập nhật thành công, ngược lại trả về false
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<bool> UpdatePhotoAsync(ProductPhoto data)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string sql = @"
                UPDATE ProductPhotos
                SET Photo        = @Photo,
                    Description  = @Description,
                    DisplayOrder = @DisplayOrder,
                    IsHidden     = @IsHidden
                WHERE PhotoID = @PhotoID";

            int rowsAffected = await connection.ExecuteAsync(sql, new
            {
                data.Photo,
                data.Description,
                data.DisplayOrder,
                data.IsHidden,
                data.PhotoID
            });

            return rowsAffected > 0;
        }
        /// <summary>
        /// Xóa thông tin ảnh trong danh dách sách ảnh của mặt hàng, 
        /// trả về true nếu xóa thành công, ngược lại trả về false.
        /// </summary>
        /// <param name="photoID"></param>
        /// <returns></returns>
        public async Task<bool> DeletePhotoAsync(long photoID)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string sql = "DELETE FROM ProductPhotos WHERE PhotoID = @PhotoID";
            int rowsAffected = await connection.ExecuteAsync(sql, new { PhotoID = photoID });
            return rowsAffected > 0;
        }
    }
    #endregion
}