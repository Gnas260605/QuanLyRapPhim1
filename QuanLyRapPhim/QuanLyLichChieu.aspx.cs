using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace QuanLyRapPhim
{
    public partial class QuanLyLichChieu : SecurePage
    {
        protected override string[] AllowedRoles { get; set; } = new[] { Role.ADMIN, Role.STAFF };
        KetNoi kn = new KetNoi();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
            LoadPhim();
            LoadPhong();
            LoadLichChieu();
        }

        private void LoadPhim()
        {
            DataTable dt = kn.LayKetNoi("SELECT MaPhim, TenPhim FROM Phim ORDER BY TenPhim");
            if (dt != null)
            {
                ddlPhim.DataSource = dt;
                ddlPhim.DataTextField = "TenPhim";
                ddlPhim.DataValueField = "MaPhim";
                ddlPhim.DataBind();
            }
        }

        private void LoadPhong()
        {
            DataTable dt = kn.LayKetNoi("SELECT MaPhong, TenPhong FROM PhongChieu ORDER BY TenPhong");
            if (dt != null)
            {
                ddlPhong.DataSource = dt;
                ddlPhong.DataTextField = "TenPhong";
                ddlPhong.DataValueField = "MaPhong";
                ddlPhong.DataBind();
            }
        }

        private void LoadLichChieu()
        {
            string sql = @"
                SELECT 
                    LC.MaLichChieu, 
                    P.TenPhim, 
                    PC.TenPhong, 
                    LC.NgayChieu, 
                    LC.GioBatDau, 
                    LC.GiaVeCoBan,
                    (SELECT COUNT(*) FROM DatVe WHERE MaLichChieu = LC.MaLichChieu AND TrangThai = N'Đã thanh toán') AS SoVeDat
                FROM LichChieu LC
                INNER JOIN Phim P ON LC.MaPhim = P.MaPhim
                INNER JOIN PhongChieu PC ON LC.MaPhong = PC.MaPhong
                ORDER BY LC.NgayChieu DESC, LC.GioBatDau DESC";

            DataTable dt = kn.LayKetNoi(sql);
            if (dt != null)
            {
                gvLichChieu.DataSource = dt;
                gvLichChieu.DataBind();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            // ── Bước 1: Kiểm tra dữ liệu nhập ──────────────────────
            int maPhim, maPhong;
            if (!int.TryParse(ddlPhim.SelectedValue, out maPhim) || !int.TryParse(ddlPhong.SelectedValue, out maPhong))
            {
                BaoLoi("Vui lòng chọn Phim và Phòng chiếu!");
                return;
            }

            DateTime ngayChieu;
            if (!DateTime.TryParse(txtNgayChieu.Text.Trim(), out ngayChieu))
            {
                BaoLoi("Ngày chiếu không hợp lệ! Hãy nhập theo dạng yyyy-MM-dd.");
                return;
            }

            TimeSpan gioBatDau;
            if (!TimeSpan.TryParse(txtGioBatDau.Text.Trim(), out gioBatDau) || gioBatDau.TotalHours >= 24)
            {
                BaoLoi("Giờ bắt đầu không hợp lệ! Hãy nhập theo dạng HH:mm (ví dụ 19:30).");
                return;
            }

            decimal giaVe;
            if (!decimal.TryParse(txtGiaVe.Text.Trim(), out giaVe) || giaVe <= 0)
            {
                BaoLoi("Giá vé phải là một số lớn hơn 0!");
                return;
            }

            // Không cho xếp lịch chiếu trong quá khứ
            if (ngayChieu.Date.Add(gioBatDau) <= DateTime.Now)
            {
                BaoLoi("Không thể xếp lịch chiếu ở thời điểm trong quá khứ!");
                return;
            }

            // ── Bước 2: Kiểm tra trùng lịch trong cùng phòng ───────
            // Lấy thời lượng của phim sắp xếp lịch
            DataTable dtPhim = kn.LayKetNoi(
                "SELECT ThoiLuong FROM Phim WHERE MaPhim = @MaPhim",
                new SqlParameter[] { new SqlParameter("@MaPhim", maPhim) });
            if (dtPhim == null || dtPhim.Rows.Count == 0)
            {
                BaoLoi("Phim được chọn không tồn tại!");
                return;
            }
            int thoiLuongMoi = Convert.ToInt32(dtPhim.Rows[0]["ThoiLuong"]);

            // Khi đang sửa thì bỏ qua chính suất chiếu đó (MaLichChieu <> @MaLichChieuHienTai)
            int maLichChieuHienTai = 0;
            if (!string.IsNullOrEmpty(hdnMaLichChieu.Value))
            {
                maLichChieuHienTai = Convert.ToInt32(hdnMaLichChieu.Value);
            }

            // Hai suất bị trùng khi: suất mới bắt đầu trước khi suất cũ kết thúc
            // VÀ suất cũ bắt đầu trước khi suất mới kết thúc (kết thúc = bắt đầu + thời lượng phim)
            string sqlTrung =
                "SELECT COUNT(*) AS SoTrung " +
                "FROM LichChieu LC " +
                "INNER JOIN Phim P ON LC.MaPhim = P.MaPhim " +
                "WHERE LC.MaPhong = @MaPhong " +
                "  AND LC.NgayChieu = @NgayChieu " +
                "  AND LC.MaLichChieu <> @MaLichChieuHienTai " +
                "  AND @GioBatDau < DATEADD(MINUTE, P.ThoiLuong, LC.GioBatDau) " +
                "  AND LC.GioBatDau < DATEADD(MINUTE, @ThoiLuongMoi, @GioBatDau)";

            DataTable dtTrung = kn.LayKetNoi(sqlTrung, new SqlParameter[]
            {
                new SqlParameter("@MaPhong", maPhong),
                new SqlParameter("@NgayChieu", ngayChieu.Date),
                new SqlParameter("@MaLichChieuHienTai", maLichChieuHienTai),
                new SqlParameter("@GioBatDau", gioBatDau),
                new SqlParameter("@ThoiLuongMoi", thoiLuongMoi)
            });
            if (dtTrung != null && Convert.ToInt32(dtTrung.Rows[0]["SoTrung"]) > 0)
            {
                BaoLoi("Phòng này đã có suất chiếu khác bị trùng khung giờ! Hãy chọn giờ hoặc phòng khác.");
                return;
            }

            // ── Bước 3: Lưu dữ liệu (parameterized query) ──────────
            string sql;
            if (maLichChieuHienTai == 0)
            {
                sql = "INSERT INTO LichChieu (MaPhim, MaPhong, NgayChieu, GioBatDau, GiaVeCoBan) " +
                      "VALUES (@MaPhim, @MaPhong, @NgayChieu, @GioBatDau, @GiaVeCoBan)";
            }
            else
            {
                sql = "UPDATE LichChieu SET MaPhim = @MaPhim, MaPhong = @MaPhong, NgayChieu = @NgayChieu, " +
                      "GioBatDau = @GioBatDau, GiaVeCoBan = @GiaVeCoBan WHERE MaLichChieu = @MaLichChieu";
            }

            var listParams = new System.Collections.Generic.List<SqlParameter>
            {
                new SqlParameter("@MaPhim", maPhim),
                new SqlParameter("@MaPhong", maPhong),
                new SqlParameter("@NgayChieu", ngayChieu.Date),
                new SqlParameter("@GioBatDau", gioBatDau),
                new SqlParameter("@GiaVeCoBan", giaVe)
            };
            if (maLichChieuHienTai != 0)
            {
                listParams.Add(new SqlParameter("@MaLichChieu", maLichChieuHienTai));
            }

            int ketQua = kn.ThucThi(sql, listParams.ToArray());
            if (ketQua > 0)
            {
                lblMsg.Text = maLichChieuHienTai == 0 ? "Thêm lịch chiếu mới thành công!" : "Cập nhật lịch chiếu thành công!";
                lblMsg.ForeColor = System.Drawing.Color.Green;
                ResetForm();
                LoadLichChieu();
            }
            else
            {
                BaoLoi("Có lỗi xảy ra, không thể lưu lịch chiếu!");
            }
        }

        // Hiển thị thông báo lỗi màu đỏ
        private void BaoLoi(string thongBao)
        {
            lblMsg.Text = thongBao;
            lblMsg.ForeColor = System.Drawing.Color.Red;
        }

        protected void gvLichChieu_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string maLC = e.CommandArgument.ToString();

            if (e.CommandName == "EditLC")
            {
                DataTable dt = kn.LayKetNoi($"SELECT * FROM LichChieu WHERE MaLichChieu = {maLC}");
                if (dt != null && dt.Rows.Count > 0)
                {
                    hdnMaLichChieu.Value = maLC;
                    ddlPhim.SelectedValue = dt.Rows[0]["MaPhim"].ToString();
                    ddlPhong.SelectedValue = dt.Rows[0]["MaPhong"].ToString();
                    txtNgayChieu.Text = Convert.ToDateTime(dt.Rows[0]["NgayChieu"]).ToString("yyyy-MM-dd");
                    txtGioBatDau.Text = dt.Rows[0]["GioBatDau"].ToString();
                    txtGiaVe.Text = Convert.ToInt32(dt.Rows[0]["GiaVeCoBan"]).ToString();
                    lblFormTitle.Text = "Sửa lịch chiếu #" + maLC;
                }
            }
            else if (e.CommandName == "DeleteLC")
            {
                // Kiểm tra xem đã có vé đặt chưa
                DataTable dtCheck = kn.LayKetNoi($"SELECT COUNT(*) AS SoVe FROM DatVe WHERE MaLichChieu = {maLC}");
                if (dtCheck != null && Convert.ToInt32(dtCheck.Rows[0]["SoVe"]) > 0)
                {
                    lblMsg.Text = "Không thể xóa lịch chiếu đã có vé đặt!";
                    lblMsg.ForeColor = System.Drawing.Color.Red;
                    return;
                }

                kn.LayKetNoi($"DELETE FROM LichChieu WHERE MaLichChieu = {maLC}");
                lblMsg.Text = "Xóa lịch chiếu thành công!";
                lblMsg.ForeColor = System.Drawing.Color.Green;
                LoadLichChieu();
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        private void ResetForm()
        {
            hdnMaLichChieu.Value = "";
            txtNgayChieu.Text = "";
            txtGioBatDau.Text = "";
            txtGiaVe.Text = "";
            lblFormTitle.Text = "Thêm lịch chiếu mới";
        }
    }
}