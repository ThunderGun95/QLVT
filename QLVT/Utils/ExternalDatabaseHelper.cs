using Microsoft.Data.SqlClient;

namespace QLVT.Utils
{
    public static class ExternalDatabaseHelper
    {
        // Connection string đến database ERP - có thể config từ file
        private static readonly string ExternalConnectionString =
            "Server=192.168.2.55,8893; Database=EOSNA; User ID=DEVManh;Password=ManhDev$2026;Max Pool Size=100;Min Pool Size=5; TrustServerCertificate=True;MultipleActiveResultSets=True";

        /// <summary>
        /// Tạo kết nối đến database ERP
        /// </summary>
        /// <returns>SqlConnection object</returns>
        public static SqlConnection GetExternalConnection()
        {
            return new SqlConnection(ExternalConnectionString);
        }

        /// <summary>
        /// Kiểm tra kết nối database ERP
        /// </summary>
        /// <returns>True nếu kết nối thành công</returns>
        public static bool TestExternalConnection()
        {
            try
            {
                using (var connection = GetExternalConnection())
                {
                    connection.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Lấy connection string hiện tại (để hiển thị thông tin)
        /// </summary>
        /// <returns>Connection string info</returns>
        public static string GetConnectionInfo()
        {
            try
            {
                var builder = new SqlConnectionStringBuilder(ExternalConnectionString);
                return $"Server: {builder.DataSource}, Database: {builder.InitialCatalog}";
            }
            catch
            {
                return "Không thể đọc thông tin kết nối";
            }
        }
    }
}
