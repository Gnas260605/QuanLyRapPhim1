using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;

namespace QuanLyRapPhim
{
    public partial class DangKy : System.Web.UI.Page
    {
        KetNoi kn = new KetNoi();
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnDangKy_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            string hoTen = txtHoTen.Text.Trim();
            string email = txtEmail.Text.Trim();
            string soDienThoai = txtSoDienThoai.Text.Trim();
            string matKhau = txtMatKhau.Text.Trim();

            if (string.IsNullOrEmpty(hoTen) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(matKhau))
            {
                lblThongBao.Text = "Vui lòng nhập đầy đủ các trường thông tin!";
                lblThanhCong.Text = "";
                return;
            }

            // Kiểm tra xem email đã tồn tại hay chưa
            string sqlCheck = "select * from NguoiDung where Email = '" + email + "'";
            DataTable dt = kn.LayKetNoi(sqlCheck);
            if (dt != null && dt.Rows.Count > 0)
            {
                lblThongBao.Text = "Email này đã được sử dụng!";
                lblThanhCong.Text = "";
                return;
            }

            // Kết nối CSDL trực tiếp để thực hiện lệnh INSERT an toàn
            string dbPath = HttpContext.Current.Server.MapPath("~/App_Data/Database1.mdf");
            string connStr = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={dbPath};Integrated Security=True";

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sqlInsert = "insert into NguoiDung (HoTen, Email, SoDienThoai, MatKhau, VaiTro) values (@HoTen, @Email, @SoDienThoai, @MatKhau, @VaiTro)";
                using (SqlCommand cmd = new SqlCommand(sqlInsert, conn))
                {
                    cmd.Parameters.AddWithValue("@HoTen", hoTen);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@SoDienThoai", soDienThoai);
                    cmd.Parameters.AddWithValue("@MatKhau", matKhau);
                    cmd.Parameters.AddWithValue("@VaiTro", "Khách hàng");

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();

                        lblThongBao.Text = "";
                        lblThanhCong.Text = "Đăng ký tài khoản thành công!";

                        // Tự động đăng nhập sau khi tạo tài khoản
                        DataTable dtNew = kn.LayKetNoi("select * from NguoiDung where Email = '" + email + "'");
                        if (dtNew != null && dtNew.Rows.Count > 0)
                        {
                            Session["MaNguoiDung"] = dtNew.Rows[0]["MaNguoiDung"];
                            Session["HoTen"] = dtNew.Rows[0]["HoTen"];
                        }

                        // Quay lại trang chủ sau khi đăng ký
                        Response.Write("<script>setTimeout(function(){ window.location.href='Phim.aspx'; }, 1500);</script>");
                    }
                    catch (Exception ex)
                    {
                        lblThongBao.Text = "Lỗi hệ thống: " + ex.Message;
                        lblThanhCong.Text = "";
                    }
                }
            }
        }
    }
}