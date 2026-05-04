using Dapper;
using Microsoft.Data.SqlClient;
using SGENERGY.DataLayers.Interfaces;
using SGENERGY.DomainModels.Common;
using SGENERGY.DomainModels.Sgenergy;

namespace SGENERGY.DataLayers.SQLServer
{
    /// <summary>
    /// Cài đặt các phép xử lý dữ liệu cho dự án sử dụng SQL Server
    /// </summary>
    public class ProjectRepository : IProjectRepository
    {
        private readonly string _connectionString;

        public ProjectRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        #region Project

        public async Task<PagedResult<Project>> ListAsync(ProjectSearchInput input)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var searchParam = string.IsNullOrWhiteSpace(input.SearchValue)
                ? ""
                : $"%{input.SearchValue}%";

            string countSql = @"
                SELECT COUNT(*) FROM Projects
                WHERE (@SearchValue = N'' OR ProjectName LIKE @SearchValue)
                  AND (@IsActive IS NULL OR IsActive = @IsActive)";

            string dataSql = input.PageSize > 0
                ? @"
                SELECT * FROM Projects
                WHERE (@SearchValue = N'' OR ProjectName LIKE @SearchValue)
                  AND (@IsActive IS NULL OR IsActive = @IsActive)
                ORDER BY DisplayOrder, ProjectID DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY"
                : @"
                SELECT * FROM Projects
                WHERE (@SearchValue = N'' OR ProjectName LIKE @SearchValue)
                  AND (@IsActive IS NULL OR IsActive = @IsActive)
                ORDER BY DisplayOrder, ProjectID DESC";

            var param = new
            {
                SearchValue = searchParam,
                input.IsActive,
                Offset = input.Offset,
                input.PageSize
            };

            int rowCount = await connection.ExecuteScalarAsync<int>(countSql, param);
            var dataItems = (await connection.QueryAsync<Project>(dataSql, param)).ToList();

            return new PagedResult<Project>
            {
                Page = input.Page,
                PageSize = input.PageSize,
                RowCount = rowCount,
                DataItems = dataItems
            };
        }

        public async Task<Project?> GetAsync(int projectID)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string sql = "SELECT * FROM Projects WHERE ProjectID = @ProjectID";
            return await connection.QueryFirstOrDefaultAsync<Project>(sql, new { ProjectID = projectID });
        }

        public async Task<Project?> GetBySlugAsync(string slug)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string sql = "SELECT * FROM Projects WHERE LOWER(Slug) = LOWER(@Slug) AND IsActive = 1";
            return await connection.QueryFirstOrDefaultAsync<Project>(sql, new { Slug = slug });
        }

        public async Task<bool> SlugExistsAsync(string slug, int excludeProjectID = 0)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string sql = @"SELECT COUNT(*) FROM Projects
                           WHERE LOWER(Slug) = LOWER(@Slug)
                             AND ProjectID <> @ExcludeID";
            int count = await connection.ExecuteScalarAsync<int>(sql, new { Slug = slug, ExcludeID = excludeProjectID });
            return count > 0;
        }

        public async Task<int> AddAsync(Project data)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string sql = @"
                INSERT INTO Projects(ProjectName, Slug, Location, Investor, ScaleDescription, Summary, DetailDescription, Thumbnail, DisplayOrder, IsFeatured, IsActive)
                VALUES (@ProjectName, @Slug, @Location, @Investor, @ScaleDescription, @Summary, @DetailDescription, @Thumbnail, @DisplayOrder, @IsFeatured, @IsActive);
                SELECT SCOPE_IDENTITY();";

            return await connection.ExecuteScalarAsync<int>(sql, new
            {
                data.ProjectName,
                data.Slug,
                data.Location,
                data.Investor,
                data.ScaleDescription,
                data.Summary,
                data.DetailDescription,
                data.Thumbnail,
                data.DisplayOrder,
                data.IsFeatured,
                data.IsActive
            });
        }

        public async Task<bool> UpdateAsync(Project data)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string sql = @"
                UPDATE Projects
                SET ProjectName       = @ProjectName,
                    Slug              = @Slug,
                    Location          = @Location,
                    Investor          = @Investor,
                    ScaleDescription  = @ScaleDescription,
                    Summary           = @Summary,
                    DetailDescription = @DetailDescription,
                    Thumbnail         = @Thumbnail,
                    DisplayOrder      = @DisplayOrder,
                    IsFeatured        = @IsFeatured,
                    IsActive          = @IsActive
                WHERE ProjectID = @ProjectID";

            int rowsAffected = await connection.ExecuteAsync(sql, new
            {
                data.ProjectName,
                data.Slug,
                data.Location,
                data.Investor,
                data.ScaleDescription,
                data.Summary,
                data.DetailDescription,
                data.Thumbnail,
                data.DisplayOrder,
                data.IsFeatured,
                data.IsActive,
                data.ProjectID
            });

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int projectID)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string sql = @"
                DELETE FROM ProjectPhotos WHERE ProjectID = @ProjectID;
                DELETE FROM Projects      WHERE ProjectID = @ProjectID;";

            int rowsAffected = await connection.ExecuteAsync(sql, new { ProjectID = projectID });
            return rowsAffected > 0;
        }

        public async Task<bool> IsUsedAsync(int projectID)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string sql = "SELECT COUNT(*) FROM ContactCustomers WHERE ProjectID = @ProjectID";
            int count = await connection.ExecuteScalarAsync<int>(sql, new { ProjectID = projectID });
            return count > 0;
        }

        #endregion

        #region ProjectPhoto

        public async Task<List<ProjectPhoto>> ListPhotosAsync(int projectID)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string sql = @"
                SELECT * FROM ProjectPhotos
                WHERE ProjectID = @ProjectID
                ORDER BY DisplayOrder";

            return (await connection.QueryAsync<ProjectPhoto>(sql, new { ProjectID = projectID })).ToList();
        }

        public async Task<ProjectPhoto?> GetPhotoAsync(long photoID)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string sql = "SELECT * FROM ProjectPhotos WHERE PhotoID = @PhotoID";
            return await connection.QueryFirstOrDefaultAsync<ProjectPhoto>(sql, new { PhotoID = photoID });
        }

        public async Task<long> AddPhotoAsync(ProjectPhoto data)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string sql = @"
                INSERT INTO ProjectPhotos(ProjectID, Photo, Description, DisplayOrder, IsHidden)
                VALUES (@ProjectID, @Photo, @Description, @DisplayOrder, @IsHidden);
                SELECT SCOPE_IDENTITY();";

            return await connection.ExecuteScalarAsync<long>(sql, new
            {
                data.ProjectID,
                data.Photo,
                data.Description,
                data.DisplayOrder,
                data.IsHidden
            });
        }

        public async Task<bool> UpdatePhotoAsync(ProjectPhoto data)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string sql = @"
                UPDATE ProjectPhotos
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

        public async Task<bool> DeletePhotoAsync(long photoID)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string sql = "DELETE FROM ProjectPhotos WHERE PhotoID = @PhotoID";
            int rowsAffected = await connection.ExecuteAsync(sql, new { PhotoID = photoID });
            return rowsAffected > 0;
        }

        #endregion
    }
}
