using System;
using System.Data;
using System.Web.UI.WebControls;

namespace QuanLyRapPhim
{
    public partial class AdminDashboard : SecurePage
    {
        // Chỉ Admin và Staff được truy cập Dashboard
        protected override string[] AllowedRoles { get; set; } = new[] { Role.ADMIN, Role.STAFF };

        KetNoi kn = new KetNoi();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            // Kiểm tra vai trò
            bool isAdmin = AuthorizationHelper.IsAdmin();

            if (!isAdmin)
            {
                // Nếu là Staff: ẩn card doanh thu và panel tài khoản hệ thống
                divRevenueCard.Visible = false;
                divTaiKhoanPanel.Visible = false;
            }

            LoadStatCards(isAdmin);
            LoadTopPhim(isAdmin);
            LoadLichChieuHomNay();
            LoadDatVeGanNhat(isAdmin);
            LoadPhongChieu();
            
            if (isAdmin)
            {
                LoadTaiKhoan();
            }
        }

        // ── 1. STAT CARDS ──────────────────────────────────────
        private void LoadStatCards(bool isAdmin)
        {
            if (isAdmin)
            {
                // Tổng doanh thu từ vé đã thanh toán
                DataTable dtDT = kn.LayKetNoi("SELECT ISNULL(SUM(TongTien),0) AS DoanhThu FROM DatVe WHERE TrangThai = N'Đã thanh toán'");
                if (dtDT != null && dtDT.Rows.Count > 0)
                    lblDoanhThu.Text = string.Format("{0:N0}", Convert.ToDecimal(dtDT.Rows[0]["DoanhThu"]));
            }

            // Tổng vé đã bán
            DataTable dtVe = kn.LayKetNoi("SELECT COUNT(*) AS SoVe FROM ChiTietDatVe CT INNER JOIN DatVe DV ON CT.MaDatVe = DV.MaDatVe WHERE DV.TrangThai = N'Đã thanh toán'");
            if (dtVe != null && dtVe.Rows.Count > 0)
                lblSoVe.Text = dtVe.Rows[0]["SoVe"].ToString();

            // Tổng số phim
            DataTable dtPhim = kn.LayKetNoi("SELECT COUNT(*) AS SoPhim FROM Phim");
            if (dtPhim != null && dtPhim.Rows.Count > 0)
                lblSoPhim.Text = dtPhim.Rows[0]["SoPhim"].ToString();

            // Tổng tài khoản
            DataTable dtUser = kn.LayKetNoi("SELECT COUNT(*) AS SoTK FROM NguoiDung");
            if (dtUser != null && dtUser.Rows.Count > 0)
                lblSoTaiKhoan.Text = dtUser.Rows[0]["SoTK"].ToString();
        }

        // ── 2. TOP 5 PHIM ĐƯỢC ĐẶT NHIỀU NHẤT ─────────────────
        private void LoadTopPhim(bool isAdmin)
        {
            string sql = @"
                SELECT TOP 5
                    P.TenPhim,
                    COUNT(CT.MaGhe) AS SoVe,
                    ISNULL(SUM(DV.TongTien), 0) AS DoanhThu
                FROM Phim P
                LEFT JOIN LichChieu LC ON LC.MaPhim = P.MaPhim
                LEFT JOIN DatVe DV ON DV.MaLichChieu = LC.MaLichChieu AND DV.TrangThai = N'Đã thanh toán'
                LEFT JOIN ChiTietDatVe CT ON CT.MaDatVe = DV.MaDatVe
                GROUP BY P.MaPhim, P.TenPhim
                ORDER BY SoVe DESC";

            DataTable dt = kn.LayKetNoi(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                if (isAdmin)
                {
                    rptTopPhimAdmin.DataSource = dt;
                    rptTopPhimAdmin.DataBind();
                    rptTopPhimAdmin.Visible = true;
                    rptTopPhimStaff.Visible = false;
                }
                else
                {
                    rptTopPhimStaff.DataSource = dt;
                    rptTopPhimStaff.DataBind();
                    rptTopPhimStaff.Visible = true;
                    rptTopPhimAdmin.Visible = false;
                }
                pnlTopPhimEmpty.Visible = false;
            }
            else
            {
                rptTopPhimAdmin.Visible = false;
                rptTopPhimStaff.Visible = false;
                pnlTopPhimEmpty.Visible = true;
            }
        }

        // ── 3. LỊCH CHIẾU HÔM NAY ──────────────────────────────
        private void LoadLichChieuHomNay()
        {
            lblHomNay.Text = DateTime.Now.ToString("dd/MM/yyyy");

            string sql = @"
                SELECT
                    P.TenPhim,
                    LC.GioBatDau,
                    PC.TenPhong,
                    COUNT(CT.MaGhe) AS SoVeDat
                FROM LichChieu LC
                INNER JOIN Phim P ON P.MaPhim = LC.MaPhim
                INNER JOIN PhongChieu PC ON PC.MaPhong = LC.MaPhong
                LEFT JOIN DatVe DV ON DV.MaLichChieu = LC.MaLichChieu AND DV.TrangThai = N'Đã thanh toán'
                LEFT JOIN ChiTietDatVe CT ON CT.MaDatVe = DV.MaDatVe
                WHERE CAST(LC.NgayChieu AS DATE) = CAST(GETDATE() AS DATE)
                GROUP BY P.TenPhim, LC.GioBatDau, PC.TenPhong
                ORDER BY LC.GioBatDau";

            DataTable dt = kn.LayKetNoi(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                rptLichChieu.DataSource = dt;
                rptLichChieu.DataBind();
                pnlLichEmpty.Visible = false;
            }
            else
            {
                rptLichChieu.Visible = false;
                pnlLichEmpty.Visible = true;
            }
        }

        // ── 4. 10 ĐẶT VÉ GẦN NHẤT ─────────────────────────────
        private void LoadDatVeGanNhat(bool isAdmin)
        {
            string sql = @"
                SELECT TOP 10
                    DV.MaDatVe,
                    ND.HoTen,
                    P.TenPhim,
                    LC.NgayChieu,
                    LC.GioBatDau,
                    COUNT(CT.MaGhe) AS SoGhe,
                    DV.TongTien,
                    DV.NgayDat,
                    DV.TrangThai
                FROM DatVe DV
                INNER JOIN NguoiDung ND ON ND.MaNguoiDung = DV.MaNguoiDung
                INNER JOIN LichChieu LC ON LC.MaLichChieu = DV.MaLichChieu
                INNER JOIN Phim P ON P.MaPhim = LC.MaPhim
                LEFT JOIN ChiTietDatVe CT ON CT.MaDatVe = DV.MaDatVe
                GROUP BY DV.MaDatVe, ND.HoTen, P.TenPhim, LC.NgayChieu, LC.GioBatDau, DV.TongTien, DV.NgayDat, DV.TrangThai
                ORDER BY DV.NgayDat DESC";

            DataTable dt = kn.LayKetNoi(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                if (isAdmin)
                {
                    rptDatVeAdmin.DataSource = dt;
                    rptDatVeAdmin.DataBind();
                    rptDatVeAdmin.Visible = true;
                    rptDatVeStaff.Visible = false;
                }
                else
                {
                    rptDatVeStaff.DataSource = dt;
                    rptDatVeStaff.DataBind();
                    rptDatVeStaff.Visible = true;
                    rptDatVeAdmin.Visible = false;
                }
                pnlDatVeEmpty.Visible = false;
            }
            else
            {
                rptDatVeAdmin.Visible = false;
                rptDatVeStaff.Visible = false;
                pnlDatVeEmpty.Visible = true;
            }
        }

        // ── 5. DANH SÁCH PHÒNG CHIẾU + THỐNG KÊ GHẾ ───────────
        private void LoadPhongChieu()
        {
            string sql = @"
                SELECT
                    PC.TenPhong,
                    COUNT(G.MaGhe) AS TongGhe,
                    SUM(CASE WHEN G.LoaiGhe = 'VIP' THEN 1 ELSE 0 END) AS GheVip,
                    SUM(CASE WHEN G.LoaiGhe != 'VIP' THEN 1 ELSE 0 END) AS GheThuong
                FROM PhongChieu PC
                LEFT JOIN Ghe G ON G.MaPhong = PC.MaPhong
                GROUP BY PC.MaPhong, PC.TenPhong
                ORDER BY PC.TenPhong";

            DataTable dt = kn.LayKetNoi(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                rptPhongChieu.DataSource = dt;
                rptPhongChieu.DataBind();
            }
        }

        // ── 6. DANH SÁCH TÀI KHOẢN ─────────────────────────────
        private void LoadTaiKhoan()
        {
            DataTable dt = kn.LayKetNoi("SELECT HoTen, Email, VaiTro FROM NguoiDung ORDER BY VaiTro, HoTen");
            if (dt != null && dt.Rows.Count > 0)
            {
                rptTaiKhoan.DataSource = dt;
                rptTaiKhoan.DataBind();
            }
        }
    }
}