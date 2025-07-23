using System.Security.Cryptography;
using System.Text;

namespace QLVT.Utils
{
    public static class PasswordHelper
    {
        /// <summary>
        /// Mã hóa password bằng SHA256
        /// </summary>
        /// <param name="password">Password cần mã hóa</param>
        /// <returns>Chuỗi hash SHA256</returns>
        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                return string.Empty;

            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Compute hash từ input
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Convert byte array thành string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        /// <summary>
        /// Kiểm tra password có khớp với hash không
        /// </summary>
        /// <param name="password">Password cần kiểm tra</param>
        /// <param name="hash">Hash đã lưu</param>
        /// <returns>True nếu khớp</returns>
        public static bool VerifyPassword(string password, string hash)
        {
            string hashOfInput = HashPassword(password);
            return StringComparer.OrdinalIgnoreCase.Compare(hashOfInput, hash) == 0;
        }
    }
}
