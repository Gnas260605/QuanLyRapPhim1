using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace QuanLyRapPhim
{
    /// <summary>
    /// Helper class để kiểm tra quyền truy cập
    /// </summary>
    public class AuthorizationHelper
    {
        /// <summary>
        /// Kiểm tra xem người dùng có đăng nhập hay không
        /// </summary>
        public static bool IsLoggedIn()
        {
            return HttpContext.Current.Session["MaNguoiDung"] != null;
        }

        /// <summary>
        /// Lấy ID người dùng từ Session
        /// </summary>
        public static int GetUserId()
        {
            if (IsLoggedIn())
            {
                int.TryParse(HttpContext.Current.Session["MaNguoiDung"].ToString(), out int userId);
                return userId;
            }
            return 0;
        }

        /// <summary>
        /// Lấy tên người dùng từ Session
        /// </summary>
        public static string GetUserName()
        {
            if (IsLoggedIn())
            {
                return HttpContext.Current.Session["HoTen"]?.ToString() ?? "";
            }
            return "";
        }

        /// <summary>
        /// Lấy role của người dùng từ Session
        /// </summary>
        public static string GetUserRole()
        {
            if (IsLoggedIn())
            {
                return HttpContext.Current.Session["VaiTro"]?.ToString() ?? "";
            }
            return "";
        }

        /// <summary>
        /// Kiểm tra xem người dùng có role cụ thể không
        /// </summary>
        public static bool HasRole(string role)
        {
            return GetUserRole() == role;
        }

        /// <summary>
        /// Kiểm tra xem người dùng có một trong các role được cấp không
        /// </summary>
        public static bool HasAnyRole(params string[] roles)
        {
            string userRole = GetUserRole();
            return roles.Contains(userRole);
        }

        /// <summary>
        /// Kiểm tra xem người dùng có phải Admin không
        /// </summary>
        public static bool IsAdmin()
        {
            return HasRole(Role.ADMIN);
        }

        /// <summary>
        /// Kiểm tra xem người dùng có phải Staff không
        /// </summary>
        public static bool IsStaff()
        {
            return HasRole(Role.STAFF);
        }

        /// <summary>
        /// Kiểm tra xem người dùng có phải Admin hoặc Staff không
        /// </summary>
        public static bool IsAdminOrStaff()
        {
            return HasAnyRole(Role.ADMIN, Role.STAFF);
        }

        /// <summary>
        /// Đăng xuất người dùng
        /// </summary>
        public static void Logout()
        {
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.Abandon();
        }
    }
}
