using System;
using System.Security.Cryptography;
using System.Text;

namespace ComputerStore_WPF.Utilities
{
    public static class SecurityHelper
    {
        /// <summary>
        /// Hash mật khẩu bằng SHA256
        /// </summary>
        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder sb = new StringBuilder();
                foreach (byte b in bytes)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }
    }
}
