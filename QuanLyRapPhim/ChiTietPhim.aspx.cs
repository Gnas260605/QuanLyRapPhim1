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
    public partial class ChiTietPhim : System.Web.UI.Page
    {
        KetNoi kn = new KetNoi();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack) return;

            int maPhim;
            if (!int.TryParse(Request.QueryString["MaPhim"], out maPhim))
            {
                Response.Redirect("Phim.aspx");
                return;
            }

            // Lấy thông tin phim (parameterized chống SQL Injection)
            string sqlPhim = "select * from Phim where MaPhim = @MaPhim";
            DataTable dtPhim = kn.LayKetNoi(sqlPhim, new SqlParameter[] { new SqlParameter("@MaPhim", maPhim) });
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

            // Lấy danh sách suất chiếu chưa diễn ra, kèm tổng ghế và số ghế còn trống của từng suất
            string sqlLC =
                "select LC.*, PC.TenPhong, " +
                "  (select count(*) from Ghe G where G.MaPhong = LC.MaPhong) as TongGhe, " +
                "  (select count(*) from Ghe G where G.MaPhong = LC.MaPhong) " +
                "  - (select count(*) from ChiTietDatVe CT " +
                "       inner join DatVe DV on CT.MaDatVe = DV.MaDatVe " +
                "       where DV.MaLichChieu = LC.MaLichChieu and DV.TrangThai = N'Đã thanh toán') as SoGheTrong " +
                "from LichChieu LC inner join PhongChieu PC on LC.MaPhong = PC.MaPhong " +
                "where LC.MaPhim = @MaPhim " +
                "and (LC.NgayChieu > CAST(GETDATE() AS date) " +
                "     or (LC.NgayChieu = CAST(GETDATE() AS date) and LC.GioBatDau > CAST(GETDATE() AS time))) " +
                "order by LC.NgayChieu, LC.GioBatDau";
            DataTable dtLC = kn.LayKetNoi(sqlLC, new SqlParameter[] { new SqlParameter("@MaPhim", maPhim) });
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

        // Đổ số ghế trống và ẩn nút "Đặt vé" khi suất chiếu đã hết ghế
        protected void dlSuatChieu_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            DataRowView row = (DataRowView)e.Item.DataItem;
            int soGheTrong = Convert.ToInt32(row["SoGheTrong"]);
            int tongGhe = Convert.ToInt32(row["TongGhe"]);

            Label lblGheTrong = (Label)e.Item.FindControl("lblGheTrong");
            if (lblGheTrong != null)
            {
                lblGheTrong.Text = soGheTrong + "/" + tongGhe + " ghế";
                // Sắp hết gh(<=5) tô cam cảnh báo, hết ghế tô đỏ
                if (soGheTrong <= 0)
                    lblGheTrong.ForeColor = System.Drawing.Color.FromArgb(0xE5, 0x3E, 0x3E);
                else if (soGheTrong <= 5)
                    lblGheTrong.ForeColor = System.Drawing.Color.FromArgb(0xFF, 0x7B, 0x00);
                else
                    lblGheTrong.ForeColor = System.Drawing.Color.FromArgb(0x48, 0xBB, 0x78);
            }

            Button btnDatVe = (Button)e.Item.FindControl("btnDatVe");
            Label lblHetGhe = (Label)e.Item.FindControl("lblHetGhe");
            if (soGheTrong <= 0)
            {
                if (btnDatVe != null) btnDatVe.Visible = false;
                if (lblHetGhe != null) lblHetGhe.Visible = true;
            }
        }
    }
}