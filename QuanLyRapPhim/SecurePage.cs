using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace QuanLyRapPhim
{
    /// <summary>
    /// Custom Page Base Class để kiểm tra quyền truy cập trang
    /// </summary>
    public class SecurePage : Page
    {
        /// <summary>
        /// Các role được phép truy cập trang này
        /// </summary>
        protected virtual string[] AllowedRoles { get; set; } = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public SecurePage()
        {
            this.PreLoad += SecurePage_PreLoad;
        }

        /// <summary>
        /// Kiểm tra quyền truy cập khi trang load
        /// </summary>
        private void SecurePage_PreLoad(object sender, EventArgs e)
        {
            CheckAccess();
        }

        /// <summary>
        /// Kiểm tra quyền truy cập
        /// </summary>
        protected virtual void CheckAccess()
        {
            // Nếu không có yêu cầu về role thì cho phép tất cả
            if (AllowedRoles == null || AllowedRoles.Length == 0)
                return;

            // Nếu chưa đăng nhập, redirect đến trang đăng nhập
            if (!AuthorizationHelper.IsLoggedIn())
            {
                Response.Redirect("DangNhap.aspx");
                return;
            }

            // Kiểm tra role
            if (!AuthorizationHelper.HasAnyRole(AllowedRoles))
            {
                Response.StatusCode = 403;
                Response.StatusDescription = "Access Denied";
                Response.Write("<h1>Không có quyền truy cập trang này!</h1>");
                Response.End();
            }
        }
    }
}
