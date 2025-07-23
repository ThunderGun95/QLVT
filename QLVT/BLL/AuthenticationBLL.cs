using QLVT.DAL;
using QLVT.Models;
using QLVT.Utils;

namespace QLVT.BLL
{
    public class AuthenticationBLL
    {
        private readonly UserDAL userDAL;
        private static User? currentUser;

        public AuthenticationBLL()
        {
            userDAL = new UserDAL();
        }

        /// <summary>
        /// Đăng nhập người dùng
        /// </summary>
        /// <param name="username">Tên đăng nhập</param>
        /// <param name="password">Mật khẩu gốc</param>
        /// <returns>Kết quả đăng nhập</returns>
        public LoginResult Login(string username, string password)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    return new LoginResult 
                    { 
                        Success = false, 
                        Message = "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu!" 
                    };
                }

                // Hash password
                string passwordHash = PasswordHelper.HashPassword(password);

                // Authenticate user
                var user = userDAL.AuthenticateUser(username, passwordHash);
                
                if (user == null)
                {
                    return new LoginResult 
                    { 
                        Success = false, 
                        Message = "Tên đăng nhập hoặc mật khẩu không đúng!" 
                    };
                }

                if (!user.IsActive)
                {
                    return new LoginResult 
                    { 
                        Success = false, 
                        Message = "Tài khoản đã bị khóa. Vui lòng liên hệ quản trị viên!" 
                    };
                }

                // Update last login
                userDAL.UpdateLastLogin(user.UserID);

                // Set current user
                currentUser = user;

                return new LoginResult 
                { 
                    Success = true, 
                    Message = "Đăng nhập thành công!", 
                    User = user 
                };
            }
            catch (Exception ex)
            {
                return new LoginResult 
                { 
                    Success = false, 
                    Message = $"Lỗi kết nối: {ex.Message}" 
                };
            }
        }

        /// <summary>
        /// Đăng xuất người dùng
        /// </summary>
        public void Logout()
        {
            currentUser = null;
        }

        /// <summary>
        /// Lấy thông tin người dùng hiện tại
        /// </summary>
        /// <returns>User hiện tại</returns>
        public static User? GetCurrentUser()
        {
            return currentUser;
        }

        /// <summary>
        /// Kiểm tra xem người dùng đã đăng nhập chưa
        /// </summary>
        /// <returns>True nếu đã đăng nhập</returns>
        public static bool IsLoggedIn()
        {
            return currentUser != null;
        }

        /// <summary>
        /// Đổi mật khẩu
        /// </summary>
        /// <param name="oldPassword">Mật khẩu cũ</param>
        /// <param name="newPassword">Mật khẩu mới</param>
        /// <returns>Kết quả đổi mật khẩu</returns>
        public ChangePasswordResult ChangePassword(string oldPassword, string newPassword)
        {
            try
            {
                if (currentUser == null)
                {
                    return new ChangePasswordResult 
                    { 
                        Success = false, 
                        Message = "Vui lòng đăng nhập trước!" 
                    };
                }

                // Validate input
                if (string.IsNullOrWhiteSpace(oldPassword) || string.IsNullOrWhiteSpace(newPassword))
                {
                    return new ChangePasswordResult 
                    { 
                        Success = false, 
                        Message = "Vui lòng nhập đầy đủ mật khẩu cũ và mật khẩu mới!" 
                    };
                }

                if (newPassword.Length < 6)
                {
                    return new ChangePasswordResult 
                    { 
                        Success = false, 
                        Message = "Mật khẩu mới phải có ít nhất 6 ký tự!" 
                    };
                }

                // Verify old password
                string oldPasswordHash = PasswordHelper.HashPassword(oldPassword);
                if (!PasswordHelper.VerifyPassword(oldPassword, currentUser.PasswordHash))
                {
                    return new ChangePasswordResult 
                    { 
                        Success = false, 
                        Message = "Mật khẩu cũ không đúng!" 
                    };
                }

                // Hash new password
                string newPasswordHash = PasswordHelper.HashPassword(newPassword);

                // Update password
                bool success = userDAL.ChangePassword(currentUser.UserID, newPasswordHash);
                
                if (success)
                {
                    currentUser.PasswordHash = newPasswordHash;
                    return new ChangePasswordResult 
                    { 
                        Success = true, 
                        Message = "Đổi mật khẩu thành công!" 
                    };
                }
                else
                {
                    return new ChangePasswordResult 
                    { 
                        Success = false, 
                        Message = "Không thể đổi mật khẩu. Vui lòng thử lại!" 
                    };
                }
            }
            catch (Exception ex)
            {
                return new ChangePasswordResult 
                { 
                    Success = false, 
                    Message = $"Lỗi: {ex.Message}" 
                };
            }
        }

        /// <summary>
        /// Lấy danh sách roles của user hiện tại
        /// </summary>
        /// <returns>Danh sách Role</returns>
        public List<Role> GetCurrentUserRoles()
        {
            if (currentUser == null)
                return new List<Role>();

            return userDAL.GetUserRoles(currentUser.UserID);
        }
    }

    // Result classes
    public class LoginResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public User? User { get; set; }
    }

    public class ChangePasswordResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
