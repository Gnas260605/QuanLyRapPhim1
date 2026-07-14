using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace QuanLyRapPhim
{
    public partial class LichSuDatVe : System.Web.UI.Page
    {
        KetNoi kn = new KetNoi();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack) return;

            // Kiểm tra trạng thái đăng nhập
            if (Session["MaNguoiDung"] == null)
            {
                lblChuaDangNhap.Visible = true;
                gvLichSu.Visible = false;
                return;
            }

            lblChuaDangNhap.Visible = false;
            string maNguoiDung = Session["MaNguoiDung"].ToString();

            // Truy vấn lịch sử đặt vé của tài khoản hiện tại
            string sql = "select DV.MaDatVe, P.TenPhim, LC.NgayChieu, LC.GioBatDau, PC.TenPhong, DV.NgayDat, DV.TongTien, DV.TrangThai " +
                         "from DatVe DV " +
                         "inner join LichChieu LC on DV.MaLichChieu = LC.MaLichChieu " +
                         "inner join PhongChieu PC on LC.MaPhong = PC.MaPhong " +
                         "inner join Phim P on LC.MaPhim = P.MaPhim " +
                         "where DV.MaNguoiDung = " + maNguoiDung + " " +
                         "order by DV.NgayDat desc";

            DataTable dt = kn.LayKetNoi(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                gvLichSu.DataSource = dt;
                gvLichSu.DataBind();
                gvLichSu.Visible = true;
                lblThongBaoTrong.Visible = false;
            }
            else
            {
                gvLichSu.Visible = false;
                lblThongBaoTrong.Visible = true;
            }
        }
    }
}