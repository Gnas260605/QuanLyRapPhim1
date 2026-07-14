using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace QuanLyRapPhim
{
    public partial class DangNhap : System.Web.UI.Page
    {
        KetNoi kn = new KetNoi();
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnDangNhap_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string matKhau = txtMatKhau.Text.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(matKhau))
            {
                lblBaoLoi.Text = "Vui lòng điền đầy đủ email và mật khẩu!";
                return;
            }

            // Đăng nhập
            string sql = "select * from NguoiDung where Email = '" + email + "' and MatKhau = '" + matKhau + "'";
            DataTable dt = kn.LayKetNoi(sql);

            if (dt != null && dt.Rows.Count > 0)
            {
                // Lưu session người dùng
                Session["MaNguoiDung"] = dt.Rows[0]["MaNguoiDung"];
                Session["HoTen"] = dt.Rows[0]["HoTen"];

                // Lưu role/vai trò người dùng vào Session
                // Hệ thống chỉ có 2 role: Admin và Staff
                string vaiTro = Role.STAFF; // Mặc định là Staff
                if (dt.Rows[0].Table.Columns.Contains("VaiTro") && dt.Rows[0]["VaiTro"] != DBNull.Value)
                {
                    string vaiTroDb = dt.Rows[0]["VaiTro"].ToString().Trim();
                    // Chỉ chấp nhận role hợp lệ trong hệ thống
                    if (Role.GetAllRoles().Contains(vaiTroDb))
                    {
                        vaiTro = vaiTroDb;
                    }
                }
                Session["VaiTro"] = vaiTro;

                // Quay lại trang trước đó nếu có
                if (Session["ReturnUrl"] != null)
                {
                    string returnUrl = Session["ReturnUrl"].ToString();
                    Session["ReturnUrl"] = null;
                    Response.Redirect(returnUrl);
                }
                else
                {
                    Response.Redirect("Phim.aspx");
                }
            }
            else
            {
                lblBaoLoi.Text = "Email hoặc mật khẩu không chính xác!";
            }
        }
    }
}