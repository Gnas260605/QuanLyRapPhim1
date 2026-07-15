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
            LoadLichSu();
        }

        private void LoadLichSu()
        {
            int maNguoiDung = Convert.ToInt32(Session["MaNguoiDung"]);

            // Lịch sử đặt vé của tài khoản hiện tại, kèm:
            //  - DanhSachGhe: gom chuỗi tên ghế (FOR XML PATH vì SQL Server 2016 chưa có STRING_AGG)
            //  - CoTheHuy: 1 nếu vé đã thanh toán VÀ suất chiếu còn ở tương lai (được phép hủy)
            string sql =
                "select DV.MaDatVe, P.TenPhim, LC.NgayChieu, LC.GioBatDau, PC.TenPhong, DV.NgayDat, DV.TongTien, DV.TrangThai, " +
                "STUFF((select ', ' + G.Hang + CAST(G.So as varchar(10)) " +
                "       from ChiTietDatVe CT " +
                "       inner join Ghe G on CT.MaGhe = G.MaGhe " +
                "       where CT.MaDatVe = DV.MaDatVe " +
                "       order by G.Hang, G.So " +
                "       for xml path('')), 1, 2, '') as DanhSachGhe, " +
                "case when DV.TrangThai = N'Đã thanh toán' " +
                "     and (LC.NgayChieu > CAST(GETDATE() AS date) " +
                "          or (LC.NgayChieu = CAST(GETDATE() AS date) and LC.GioBatDau > CAST(GETDATE() AS time))) " +
                "     then 1 else 0 end as CoTheHuy " +
                "from DatVe DV " +
                "inner join LichChieu LC on DV.MaLichChieu = LC.MaLichChieu " +
                "inner join PhongChieu PC on LC.MaPhong = PC.MaPhong " +
                "inner join Phim P on LC.MaPhim = P.MaPhim " +
                "where DV.MaNguoiDung = @MaNguoiDung " +
                "order by DV.NgayDat desc";

            DataTable dt = kn.LayKetNoi(sql, new SqlParameter[]
            {
                new SqlParameter("@MaNguoiDung", maNguoiDung)
            });

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

        protected void gvLichSu_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "CancelVe") return;

            // Phải đăng nhập mới được hủy
            if (Session["MaNguoiDung"] == null)
            {
                Response.Redirect("DangNhap.aspx");
                return;
            }

            int maDatVe;
            if (!int.TryParse(e.CommandArgument.ToString(), out maDatVe)) return;

            int maNguoiDung = Convert.ToInt32(Session["MaNguoiDung"]);

            // Kiểm tra lại ở server: vé thuộc về đúng người dùng, đang ở trạng thái đã thanh toán,
            // và suất chiếu vẫn còn ở tương lai. Chống việc gửi lệnh hủy vé không hợp lệ.
            string sqlCheck =
                "select count(*) as SoLuong " +
                "from DatVe DV inner join LichChieu LC on DV.MaLichChieu = LC.MaLichChieu " +
                "where DV.MaDatVe = @MaDatVe and DV.MaNguoiDung = @MaNguoiDung " +
                "  and DV.TrangThai = N'Đã thanh toán' " +
                "  and (LC.NgayChieu > CAST(GETDATE() AS date) " +
                "       or (LC.NgayChieu = CAST(GETDATE() AS date) and LC.GioBatDau > CAST(GETDATE() AS time)))";
            DataTable dtCheck = kn.LayKetNoi(sqlCheck, new SqlParameter[]
            {
                new SqlParameter("@MaDatVe", maDatVe),
                new SqlParameter("@MaNguoiDung", maNguoiDung)
            });

            if (dtCheck == null || Convert.ToInt32(dtCheck.Rows[0]["SoLuong"]) == 0)
            {
                BaoLoi("Không thể hủy vé này (vé không hợp lệ hoặc suất chiếu đã bắt đầu).");
                LoadLichSu();
                return;
            }

            // Đổi trạng thái sang 'Đã hủy' (giữ lại lịch sử thay vì xóa cứng).
            // Ghế sẽ tự động được trả lại vì các truy vấn ghế đã đặt chỉ tính vé 'Đã thanh toán'.
            int ketQua = kn.ThucThi(
                "update DatVe set TrangThai = N'Đã hủy' where MaDatVe = @MaDatVe and MaNguoiDung = @MaNguoiDung",
                new SqlParameter[]
                {
                    new SqlParameter("@MaDatVe", maDatVe),
                    new SqlParameter("@MaNguoiDung", maNguoiDung)
                });

            if (ketQua > 0)
            {
                lblMsg.ForeColor = System.Drawing.Color.FromArgb(0x48, 0xBB, 0x78);
                lblMsg.Text = "Đã hủy vé #" + maDatVe + " thành công. Ghế đã được trả lại cho suất chiếu.";
            }
            else
            {
                BaoLoi("Có lỗi xảy ra, không thể hủy vé!");
            }

            LoadLichSu();
        }

        private void BaoLoi(string thongBao)
        {
            lblMsg.ForeColor = System.Drawing.Color.FromArgb(0xE5, 0x3E, 0x3E);
            lblMsg.Text = thongBao;
        }
    }
}
