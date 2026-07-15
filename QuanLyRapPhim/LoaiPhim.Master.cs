using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace QuanLyRapPhim
{
    public partial class LoaiPhim : System.Web.UI.MasterPage
    {
        KetNoi kn = new KetNoi();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack) return;

            // Kiểm tra xem có đang ở trang quản trị hay không
            string pageName = System.IO.Path.GetFileName(Request.Url.AbsolutePath).ToLower();
            bool isAdminPage = pageName.Contains("admin") || pageName.Contains("quanly");

            if (isAdminPage)
            {
                // Ẩn cột thể loại (cột trái) và cột khuyến mãi (cột phải)
                divLeft.Visible = false;
                divRight.Visible = false;
            }
            else
            {
                // Chỉ lấy danh sách thể loại phim khi ở trang thường
                string q = "select distinct TheLoai from Phim";
                this.DataList1.DataSource = kn.LayKetNoi(q);
                this.DataList1.DataBind();
            }

            // Điều khiển hiển thị nav theo trạng thái đăng nhập và role
            bool isLoggedIn = AuthorizationHelper.IsLoggedIn();
            bool isAdminOrStaff = AuthorizationHelper.IsAdminOrStaff();
            bool isAdmin = AuthorizationHelper.IsAdmin();

            // Hiển thị tên người dùng và nút Đăng xuất khi đã đăng nhập
            pnlDaDangNhap.Visible = isLoggedIn;
            pnlChuaDangNhap.Visible = !isLoggedIn;

            // Link "Dashboard", "Quản lý Phim", "Lịch chiếu" cho Admin và Staff
            // (ẩn/hiện cả dấu phân cách đi kèm để nav không bị thừa dấu |)
            lnkDashboard.Visible = isAdminOrStaff;
            litSepDash.Visible = isAdminOrStaff;
            lnkQuanLyPhim.Visible = isAdminOrStaff;
            litSepQuanLy.Visible = isAdminOrStaff;
            lnkQuanLyLichChieu.Visible = isAdminOrStaff;
            litSepLichChieu.Visible = isAdminOrStaff;
            lnkQuanLyPhong.Visible = isAdminOrStaff;
            litSepPhong.Visible = isAdminOrStaff;

            // Link "Người dùng" chỉ dành cho Admin
            lnkQuanLyNguoiDung.Visible = isAdmin;
            litSepNguoiDung.Visible = isAdmin;

            if (isLoggedIn)
            {
                lblTenNguoiDung.Text = AuthorizationHelper.GetUserName();
            }
        }


        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            // Lọc phim theo thể loại
            string theLoai = ((LinkButton)sender).CommandArgument;
            Context.Items["TL"] = theLoai;
            Server.Transfer("Phim.aspx");
        }

        protected void btnDangXuat_Click(object sender, EventArgs e)
        {
            // Đăng xuất và về trang chủ
            AuthorizationHelper.Logout();
            Response.Redirect("Phim.aspx");
        }
    }
}