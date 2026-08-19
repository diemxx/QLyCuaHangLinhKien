using System;
using Microsoft.Data.SqlClient;

namespace ComputerStore_WPF.DataAccess
{
    /// <summary>
    /// Lớp quản lý kết nối ADO.NET đến SQL Server
    /// </summary>
    public static class AppDbContext
    {
        /// <summary>
        /// Tạo một SqlConnection mới
        /// </summary>
        public static SqlConnection GetConnection()
        {
            string connectionString = DatabaseConfig.GetConnectionString();
            return new SqlConnection(connectionString);
        }

        /// <summary>
        /// Kiểm tra kết nối đến database
        /// </summary>
        public static bool TestConnection()
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
