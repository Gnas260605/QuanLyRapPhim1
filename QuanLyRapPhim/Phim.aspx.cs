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
    public partial class Phim : System.Web.UI.Page
    {
        KetNoi kn = new KetNoi();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack) return;

            // Thể loại đến từ menu bên trái (Server.Transfer), từ khóa tìm kiếm đến từ query string
            string theLoai = Context.Items["TL"] as string ?? "";
            string tuKhoa = (Request.QueryString["search"] ?? "").Trim();

            string sql;
            SqlParameter[] thamSo = null;

            if (!string.IsNullOrEmpty(tuKhoa))
            {
                // Ưu tiên tìm kiếm theo tên phim (parameterized chống SQL Injection)
                txtTimKiem.Text = tuKhoa;
                sql = "select * from Phim where TenPhim like @kw order by TenPhim";
                thamSo = new SqlParameter[] { new SqlParameter("@kw", "%" + tuKhoa + "%") };
                lblKetQua.Visible = true;
                lblKetQua.Text = "Kết quả tìm kiếm cho: \"" + Server.HtmlEncode(tuKhoa) + "\"";
                btnXoaTim.Visible = true;
            }
            else if (!string.IsNullOrEmpty(theLoai))
            {
                // Lọc theo thể loại
                sql = "select * from Phim where TheLoai = @tl order by TenPhim";
                thamSo = new SqlParameter[] { new SqlParameter("@tl", theLoai) };
                lblKetQua.Visible = true;
                lblKetQua.Text = "Thể loại: " + Server.HtmlEncode(theLoai);
                btnXoaTim.Visible = true;
            }
            else
            {
                // Toàn bộ danh sách phim
                sql = "select * from Phim order by TenPhim";
            }

            DataTable dt = thamSo == null ? kn.LayKetNoi(sql) : kn.LayKetNoi(sql, thamSo);
            DataList1.DataSource = dt;
            DataList1.DataBind();

            // Trạng thái rỗng
            lblKhongCoPhim.Visible = (dt == null || dt.Rows.Count == 0);
        }

        protected void btnTimKiem_Click(object sender, EventArgs e)
        {
            // Dùng query string để URL tìm kiếm có thể chia sẻ / bookmark được
            string tuKhoa = txtTimKiem.Text.Trim();
            if (string.IsNullOrEmpty(tuKhoa))
            {
                Response.Redirect("Phim.aspx");
            }
            else
            {
                Response.Redirect("Phim.aspx?search=" + Server.UrlEncode(tuKhoa));
            }
        }

        protected void btnXoaTim_Click(object sender, EventArgs e)
        {
            Response.Redirect("Phim.aspx");
        }
    }
}
