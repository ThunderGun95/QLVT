using Microsoft.Data.SqlClient;
using QLVT.Models;
using QLVT.Utils;
using System.Data;

namespace QLVT.DAL
{
    public class UserDAL
    {
        /// <summary>
        /// Xác thực thông tin đăng nhập
        /// </summary>
        /// <param name="username">Tên đăng nhập</param>
        /// <param name="passwordHash">Mật khẩu đã hash</param>
        /// <returns>User object nếu hợp lệ, null nếu không</returns>
        public User? AuthenticateUser(string username, string passwordHash)
        {
            string sql = @"
                SELECT UserID, Username, PasswordHash, FullName, IsActive, CreatedDate, LastLogin 
                FROM Users 
                WHERE Username = @Username AND PasswordHash = @PasswordHash AND IsActive = 1";

            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@PasswordHash", passwordHash);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                UserID = reader.GetInt32("UserID"),
                                Username = reader.GetString("Username"),
                                PasswordHash = reader.GetString("PasswordHash"),
                                FullName = reader.GetString("FullName"),
                                IsActive = reader.GetBoolean("IsActive"),
                                CreatedDate = reader.GetDateTime("CreatedDate"),
                                LastLogin = reader.IsDBNull("LastLogin") ? null : reader.GetDateTime("LastLogin")
                            };
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Cập nhật thời gian đăng nhập cuối
        /// </summary>
        /// <param name="userID">ID người dùng</param>
        public void UpdateLastLogin(int userID)
        {
            string sql = "UPDATE Users SET LastLogin = GETDATE() WHERE UserID = @UserID";
            DatabaseHelper.ExecuteNonQuery(sql, new SqlParameter("@UserID", userID));
        }

        /// <summary>
        /// Lấy danh sách roles của user
        /// </summary>
        /// <param name="userID">ID người dùng</param>
        /// <returns>Danh sách Role</returns>
        public List<Role> GetUserRoles(int userID)
        {
            var roles = new List<Role>();
            string sql = @"
                SELECT r.RoleID, r.RoleName, r.Description, r.IsActive
                FROM Roles r
                INNER JOIN UserRoles ur ON r.RoleID = ur.RoleID
                WHERE ur.UserID = @UserID AND r.IsActive = 1";

            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userID);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            roles.Add(new Role
                            {
                                RoleID = reader.GetInt32("RoleID"),
                                RoleName = reader.GetString("RoleName"),
                                Description = reader.GetString("Description"),
                                IsActive = reader.GetBoolean("IsActive")
                            });
                        }
                    }
                }
            }
            return roles;
        }

        /// <summary>
        /// Đổi mật khẩu user
        /// </summary>
        /// <param name="userID">ID người dùng</param>
        /// <param name="newPasswordHash">Mật khẩu mới đã hash</param>
        /// <returns>True nếu thành công</returns>
        public bool ChangePassword(int userID, string newPasswordHash)
        {
            try
            {
                string sql = "UPDATE Users SET PasswordHash = @PasswordHash WHERE UserID = @UserID";
                int rowsAffected = DatabaseHelper.ExecuteNonQuery(sql, 
                    new SqlParameter("@PasswordHash", newPasswordHash),
                    new SqlParameter("@UserID", userID));
                
                return rowsAffected > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}
