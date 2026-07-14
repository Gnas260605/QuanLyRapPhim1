using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace QuanLyRapPhim
{
    public partial class ChiTietPhim : System.Web.UI.Page
    {
        KetNoi kn = new KetNoi();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack) return;

            string maPhim = Request.QueryString["MaPhim"];
            if (string.IsNullOrEmpty(maPhim))
            {
                Response.Redirect("Phim.aspx");
                return;
            }

            // Lấy thông tin phim
            string sqlPhim = "select * from Phim where MaPhim = " + maPhim;
            DataTable dtPhim = kn.LayKetNoi(sqlPhim);
            if (dtPhim != null && dtPhim.Rows.Count > 0)
            {
                DataRow r = dtPhim.Rows[0];
                lblTenPhim.Text = r["TenPhim"].ToString();
                lblTheLoai.Text = r["TheLoai"].ToString();
                lblThoiLuong.Text = r["ThoiLuong"].ToString();
                lblDaoDien.Text = r["DaoDien"].ToString();
                lblNgayKhoiChieu.Text = Convert.ToDateTime(r["NgayKhoiChieu"]).ToString("dd/MM/yyyy");
                lblMoTa.Text = r["MoTa"].ToString();
                imgPoster.ImageUrl = "~/img/" + r["HinhAnh"].ToString();
            }

            // Lấy danh sách suất chiếu
            string sqlLC = "select LC.*, PC.TenPhong from LichChieu LC inner join PhongChieu PC on LC.MaPhong = PC.MaPhong where LC.MaPhim = " + maPhim;
            DataTable dtLC = kn.LayKetNoi(sqlLC);
            if (dtLC != null && dtLC.Rows.Count > 0)
            {
                dlSuatChieu.DataSource = dtLC;
                dlSuatChieu.DataBind();
                dlSuatChieu.Visible = true;
                lblThongBaoLC.Visible = false;
            }
            else
            {
                dlSuatChieu.Visible = false;
                lblThongBaoLC.Visible = true;
            }
        }
    }
}