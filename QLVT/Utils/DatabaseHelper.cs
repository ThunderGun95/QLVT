using Microsoft.Data.SqlClient;
using System.Configuration;

namespace QLVT.Utils
{
    public static class DatabaseHelper
    {
        // Connection string - trong thực tế nên để trong config file
        private static readonly string ConnectionString =
            "Server=(localdb)\\MSSQLLocalDB;Database=QLVT_DB;User ID=sa;Password=123456;Integrated Security=true;TrustServerCertificate=true;";

        /// <summary>
        /// Tạo kết nối đến database
        /// </summary>
        /// <returns>SqlConnection object</returns>
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        /// <summary>
        /// Kiểm tra kết nối database
        /// </summary>
        /// <returns>True nếu kết nối thành công</returns>
        public static bool TestConnection()
        {
            try
            {
                using (var connection = GetConnection())
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
        /// Thực thi câu lệnh SQL không trả về kết quả
        /// </summary>
        /// <param name="sql">Câu lệnh SQL</param>
        /// <param name="parameters">Tham số</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        public static int ExecuteNonQuery(string sql, params SqlParameter[] parameters)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(sql, connection))
                {
                    if (parameters != null)
                        command.Parameters.AddRange(parameters);
                    
                    return command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Thực thi câu lệnh SQL trả về một giá trị
        /// </summary>
        /// <param name="sql">Câu lệnh SQL</param>
        /// <param name="parameters">Tham số</param>
        /// <returns>Giá trị đầu tiên</returns>
        public static object? ExecuteScalar(string sql, params SqlParameter[] parameters)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(sql, connection))
                {
                    if (parameters != null)
                        command.Parameters.AddRange(parameters);
                    
                    return command.ExecuteScalar();
                }
            }
        }
    }
}
