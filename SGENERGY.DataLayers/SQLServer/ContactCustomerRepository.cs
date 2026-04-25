using Dapper;
using Microsoft.Data.SqlClient;
using SGENERGY.DataLayers.Interfaces;
using SGENERGY.DomainModels.Common;
using SGENERGY.DomainModels.Sgenergy;

namespace SGENERGY.DataLayers.SQLServer
{
    /// <summary>
    /// Cài đặt các phép xử lý dữ liệu cho liên hệ khách hàng sử dụng SQL Server
    /// </summary>
    public class ContactCustomerRepository : IContactCustomerRepository
    {
        private readonly string _connectionString;

        public ContactCustomerRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<PagedResult<ContactCustomer>> ListAsync(ContactSearchInput input)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var searchParam = string.IsNullOrWhiteSpace(input.SearchValue)
                ? ""
                : $"%{input.SearchValue}%";

            string countSql = @"
                SELECT COUNT(*) FROM ContactCustomers
                WHERE (@SearchValue = N'' OR FullName LIKE @SearchValue OR Phone LIKE @SearchValue OR Email LIKE @SearchValue)
                  AND (@IsHandled IS NULL OR IsHandled = @IsHandled)";

            string dataSql = input.PageSize > 0
                ? @"
                SELECT * FROM ContactCustomers
                WHERE (@SearchValue = N'' OR FullName LIKE @SearchValue OR Phone LIKE @SearchValue OR Email LIKE @SearchValue)
                  AND (@IsHandled IS NULL OR IsHandled = @IsHandled)
                ORDER BY CreatedAt DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY"
                : @"
                SELECT * FROM ContactCustomers
                WHERE (@SearchValue = N'' OR FullName LIKE @SearchValue OR Phone LIKE @SearchValue OR Email LIKE @SearchValue)
                  AND (@IsHandled IS NULL OR IsHandled = @IsHandled)
                ORDER BY CreatedAt DESC";

            var param = new
            {
                SearchValue = searchParam,
                input.IsHandled,
                Offset = input.Offset,
                input.PageSize
            };

            int rowCount = await connection.ExecuteScalarAsync<int>(countSql, param);
            var dataItems = (await connection.QueryAsync<ContactCustomer>(dataSql, param)).ToList();

            return new PagedResult<ContactCustomer>
            {
                Page = input.Page,
                PageSize = input.PageSize,
                RowCount = rowCount,
                DataItems = dataItems
            };
        }

        public async Task<ContactCustomer?> GetAsync(long contactID)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string sql = "SELECT * FROM ContactCustomers WHERE ContactID = @ContactID";
            return await connection.QueryFirstOrDefaultAsync<ContactCustomer>(sql, new { ContactID = contactID });
        }

        public async Task<long> AddAsync(ContactCustomer data)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string sql = @"
                INSERT INTO ContactCustomers(FullName, Phone, Email, CompanyName, Address, Subject, Message, ProductID, ProjectID, SourcePage, IsHandled, CreatedAt)
                VALUES (@FullName, @Phone, @Email, @CompanyName, @Address, @Subject, @Message, @ProductID, @ProjectID, @SourcePage, 0, SYSDATETIME());
                SELECT SCOPE_IDENTITY();";

            return await connection.ExecuteScalarAsync<long>(sql, new
            {
                data.FullName,
                data.Phone,
                data.Email,
                data.CompanyName,
                data.Address,
                data.Subject,
                data.Message,
                data.ProductID,
                data.ProjectID,
                data.SourcePage
            });
        }

        public async Task<bool> MarkHandledAsync(long contactID, string handledBy)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string sql = @"
                UPDATE ContactCustomers
                SET IsHandled = 1,
                    HandledBy = @HandledBy,
                    HandledAt = SYSDATETIME()
                WHERE ContactID = @ContactID";

            int rowsAffected = await connection.ExecuteAsync(sql, new { ContactID = contactID, HandledBy = handledBy });
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(long contactID)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string sql = "DELETE FROM ContactCustomers WHERE ContactID = @ContactID";
            int rowsAffected = await connection.ExecuteAsync(sql, new { ContactID = contactID });
            return rowsAffected > 0;
        }
    }
}
