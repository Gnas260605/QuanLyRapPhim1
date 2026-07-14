using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyRapPhim
{
    /// <summary>
    /// Định nghĩa các Role trong hệ thống.
    /// Hệ thống chỉ có 2 role:
    ///   - Admin: Quản trị viên, có toàn quyền
    ///   - Staff: Nhân viên, có quyền truy cập trang quản lý
    /// Tài khoản chỉ được tạo bởi Admin, không có đăng ký công khai.
    /// </summary>
    public class Role
    {
        /// <summary>Quản trị viên - toàn quyền</summary>
        public const string ADMIN = "Admin";

        /// <summary>Nhân viên - quyền truy cập trang quản lý</summary>
        public const string STAFF = "Staff";

        /// <summary>Trả về danh sách tất cả role trong hệ thống</summary>
        public static List<string> GetAllRoles()
        {
            return new List<string> { ADMIN, STAFF };
        }
    }
}
