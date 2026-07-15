using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace QuanLyRapPhim
{
    public partial class QuanLyPhongChieu : SecurePage
    {
        // Admin và Staff đều được quản lý phòng chiếu (cùng nhóm với Quản lý Phim / Lịch chiếu)
        protected override string[] AllowedRoles { get; set; } = new[] { Role.ADMIN, Role.STAFF };
        KetNoi kn = new KetNoi();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
            LoadPhong();
        }

        // ══════════════════ PHÒNG CHIẾU ══════════════════

        private void LoadPhong()
        {
            // Kèm số ghế thực tế đã tạo cho từng phòng
            string sql = @"
                SELECT PC.MaPhong, PC.TenPhong, PC.SucChua,
                       (SELECT COUNT(*) FROM Ghe G WHERE G.MaPhong = PC.MaPhong) AS SoGheThucTe
                FROM PhongChieu PC
                ORDER BY PC.TenPhong";
            DataTable dt = kn.LayKetNoi(sql);
            if (dt != null)
            {
                gvPhong.DataSource = dt;
                gvPhong.DataBind();
            }
        }

        protected void btnSavePhong_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            string tenPhong = txtTenPhong.Text.Trim();
            int sucChua;
            if (!int.TryParse(txtSucChua.Text.Trim(), out sucChua) || sucChua <= 0)
            {
                BaoLoi("Sức chứa phải là số nguyên lớn hơn 0!");
                return;
            }

            // Kiểm tra trùng tên phòng (bỏ qua chính phòng đang sửa)
            int maPhongHienTai = 0;
            if (!string.IsNullOrEmpty(hdnMaPhong.Value))
            {
                maPhongHienTai = Convert.ToInt32(hdnMaPhong.Value);
            }

            DataTable dtTrung = kn.LayKetNoi(
                "SELECT COUNT(*) AS SoTrung FROM PhongChieu WHERE TenPhong = @TenPhong AND MaPhong <> @MaPhong",
                new SqlParameter[]
                {
                    new SqlParameter("@TenPhong", tenPhong),
                    new SqlParameter("@MaPhong", maPhongHienTai)
                });
            if (dtTrung != null && Convert.ToInt32(dtTrung.Rows[0]["SoTrung"]) > 0)
            {
                BaoLoi("Tên phòng này đã tồn tại!");
                return;
            }

            string sql;
            if (maPhongHienTai == 0)
            {
                sql = "INSERT INTO PhongChieu (TenPhong, SucChua) VALUES (@TenPhong, @SucChua)";
            }
            else
            {
                sql = "UPDATE PhongChieu SET TenPhong = @TenPhong, SucChua = @SucChua WHERE MaPhong = @MaPhong";
            }

            var listParams = new System.Collections.Generic.List<SqlParameter>
            {
                new SqlParameter("@TenPhong", tenPhong),
                new SqlParameter("@SucChua", sucChua)
            };
            if (maPhongHienTai != 0)
            {
                listParams.Add(new SqlParameter("@MaPhong", maPhongHienTai));
            }

            int ketQua = kn.ThucThi(sql, listParams.ToArray());
            if (ketQua > 0)
            {
                BaoThanhCong(maPhongHienTai == 0 ? "Thêm phòng chiếu thành công!" : "Cập nhật phòng chiếu thành công!");
                ResetFormPhong();
                LoadPhong();
            }
            else
            {
                BaoLoi("Có lỗi xảy ra, không thể lưu phòng chiếu!");
            }
        }

        protected void gvPhong_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int maPhong;
            if (!int.TryParse(e.CommandArgument.ToString(), out maPhong)) return;

            if (e.CommandName == "EditPhong")
            {
                DataTable dt = kn.LayKetNoi(
                    "SELECT * FROM PhongChieu WHERE MaPhong = @MaPhong",
                    new SqlParameter[] { new SqlParameter("@MaPhong", maPhong) });
                if (dt != null && dt.Rows.Count > 0)
                {
                    hdnMaPhong.Value = maPhong.ToString();
                    txtTenPhong.Text = dt.Rows[0]["TenPhong"].ToString();
                    txtSucChua.Text = dt.Rows[0]["SucChua"].ToString();
                    lblFormTitle.Text = "Cập nhật phòng #" + maPhong;
                    btnSavePhong.Text = "Cập nhật";
                }
            }
            else if (e.CommandName == "DeletePhong")
            {
                XoaPhong(maPhong);
            }
            else if (e.CommandName == "ManageSeats")
            {
                MoQuanLyGhe(maPhong);
            }
        }

        private void XoaPhong(int maPhong)
        {
            // Chặn xóa nếu phòng đang có lịch chiếu
            DataTable dtLC = kn.LayKetNoi(
                "SELECT COUNT(*) AS SoLC FROM LichChieu WHERE MaPhong = @MaPhong",
                new SqlParameter[] { new SqlParameter("@MaPhong", maPhong) });
            if (dtLC != null && Convert.ToInt32(dtLC.Rows[0]["SoLC"]) > 0)
            {
                BaoLoi("Không thể xóa phòng đang có lịch chiếu! Hãy xóa các lịch chiếu của phòng trước.");
                return;
            }

            // Chặn xóa nếu ghế trong phòng đã có vé đặt
            DataTable dtVe = kn.LayKetNoi(
                @"SELECT COUNT(*) AS SoVe
                  FROM ChiTietDatVe CT
                  INNER JOIN Ghe G ON CT.MaGhe = G.MaGhe
                  WHERE G.MaPhong = @MaPhong",
                new SqlParameter[] { new SqlParameter("@MaPhong", maPhong) });
            if (dtVe != null && Convert.ToInt32(dtVe.Rows[0]["SoVe"]) > 0)
            {
                BaoLoi("Không thể xóa phòng vì có ghế đã bán vé!");
                return;
            }

            // Xóa ghế của phòng rồi xóa phòng trong cùng một transaction
            string dbPath = Server.MapPath("~/App_Data/Database1.mdf");
            string connStr = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={dbPath};Integrated Security=True";
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();
                try
                {
                    using (SqlCommand cmdGhe = new SqlCommand("DELETE FROM Ghe WHERE MaPhong = @MaPhong", conn, trans))
                    {
                        cmdGhe.Parameters.AddWithValue("@MaPhong", maPhong);
                        cmdGhe.ExecuteNonQuery();
                    }
                    using (SqlCommand cmdPhong = new SqlCommand("DELETE FROM PhongChieu WHERE MaPhong = @MaPhong", conn, trans))
                    {
                        cmdPhong.Parameters.AddWithValue("@MaPhong", maPhong);
                        cmdPhong.ExecuteNonQuery();
                    }
                    trans.Commit();
                    BaoThanhCong("Xóa phòng chiếu thành công!");
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    BaoLoi("Lỗi khi xóa phòng: " + ex.Message);
                }
            }

            // Nếu đang mở quản lý ghế của chính phòng vừa xóa thì đóng lại
            if (hdnMaPhongGhe.Value == maPhong.ToString())
            {
                pnlGhe.Visible = false;
            }
            ResetFormPhong();
            LoadPhong();
        }

        protected void btnCancelPhong_Click(object sender, EventArgs e)
        {
            ResetFormPhong();
        }

        private void ResetFormPhong()
        {
            hdnMaPhong.Value = "";
            txtTenPhong.Text = "";
            txtSucChua.Text = "";
            lblFormTitle.Text = "Thêm phòng chiếu mới";
            btnSavePhong.Text = "Lưu phòng";
        }

        // ══════════════════ GHẾ ══════════════════

        private void MoQuanLyGhe(int maPhong)
        {
            DataTable dt = kn.LayKetNoi(
                "SELECT TenPhong FROM PhongChieu WHERE MaPhong = @MaPhong",
                new SqlParameter[] { new SqlParameter("@MaPhong", maPhong) });
            if (dt == null || dt.Rows.Count == 0)
            {
                BaoLoi("Phòng không tồn tại!");
                return;
            }

            hdnMaPhongGhe.Value = maPhong.ToString();
            lblTenPhongGhe.Text = dt.Rows[0]["TenPhong"].ToString();
            pnlGhe.Visible = true;
            LoadGhe(maPhong);
        }

        private void LoadGhe(int maPhong)
        {
            DataTable dt = kn.LayKetNoi(
                "SELECT MaGhe, Hang, So, LoaiGhe FROM Ghe WHERE MaPhong = @MaPhong ORDER BY Hang, So",
                new SqlParameter[] { new SqlParameter("@MaPhong", maPhong) });
            if (dt != null)
            {
                gvGhe.DataSource = dt;
                gvGhe.DataBind();
            }
        }

        protected void btnSinhGhe_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            int maPhong;
            if (!int.TryParse(hdnMaPhongGhe.Value, out maPhong))
            {
                BaoLoi("Chưa chọn phòng để sinh ghế!");
                return;
            }

            int soHang, soGheMoiHang;
            if (!int.TryParse(txtSoHang.Text.Trim(), out soHang) || soHang <= 0 || soHang > 26)
            {
                BaoLoi("Số hàng phải từ 1 đến 26 (tương ứng A đến Z)!");
                return;
            }
            if (!int.TryParse(txtSoGheMoiHang.Text.Trim(), out soGheMoiHang) || soGheMoiHang <= 0 || soGheMoiHang > 50)
            {
                BaoLoi("Số ghế mỗi hàng phải từ 1 đến 50!");
                return;
            }
            string loaiGhe = ddlLoaiGhe.SelectedValue;

            // Lấy các ghế đã tồn tại để không tạo trùng (Hang + So)
            DataTable dtGheCu = kn.LayKetNoi(
                "SELECT Hang, So FROM Ghe WHERE MaPhong = @MaPhong",
                new SqlParameter[] { new SqlParameter("@MaPhong", maPhong) });
            var gheDaCo = new System.Collections.Generic.HashSet<string>();
            if (dtGheCu != null)
            {
                foreach (DataRow r in dtGheCu.Rows)
                {
                    gheDaCo.Add(r["Hang"].ToString() + "-" + r["So"].ToString());
                }
            }

            int soGheTao = 0;
            string dbPath = Server.MapPath("~/App_Data/Database1.mdf");
            string connStr = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={dbPath};Integrated Security=True";
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();
                try
                {
                    for (int h = 0; h < soHang; h++)
                    {
                        string tenHang = ((char)('A' + h)).ToString();
                        for (int so = 1; so <= soGheMoiHang; so++)
                        {
                            if (gheDaCo.Contains(tenHang + "-" + so)) continue; // Bỏ qua ghế đã có

                            using (SqlCommand cmd = new SqlCommand(
                                "INSERT INTO Ghe (MaPhong, Hang, So, LoaiGhe) VALUES (@MaPhong, @Hang, @So, @LoaiGhe)", conn, trans))
                            {
                                cmd.Parameters.AddWithValue("@MaPhong", maPhong);
                                cmd.Parameters.AddWithValue("@Hang", tenHang);
                                cmd.Parameters.AddWithValue("@So", so);
                                cmd.Parameters.AddWithValue("@LoaiGhe", loaiGhe);
                                cmd.ExecuteNonQuery();
                            }
                            soGheTao++;
                        }
                    }
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    BaoLoi("Lỗi khi sinh ghế: " + ex.Message);
                    return;
                }
            }

            BaoThanhCong("Đã tạo thêm " + soGheTao + " ghế " + loaiGhe + " (ghế đã tồn tại được giữ nguyên).");
            txtSoHang.Text = "";
            txtSoGheMoiHang.Text = "";
            LoadGhe(maPhong);
            LoadPhong();
        }

        protected void btnXoaTatCaGhe_Click(object sender, EventArgs e)
        {
            int maPhong;
            if (!int.TryParse(hdnMaPhongGhe.Value, out maPhong)) return;

            // Chặn nếu bất kỳ ghế nào của phòng đã được bán vé
            DataTable dtVe = kn.LayKetNoi(
                @"SELECT COUNT(*) AS SoVe
                  FROM ChiTietDatVe CT
                  INNER JOIN Ghe G ON CT.MaGhe = G.MaGhe
                  WHERE G.MaPhong = @MaPhong",
                new SqlParameter[] { new SqlParameter("@MaPhong", maPhong) });
            if (dtVe != null && Convert.ToInt32(dtVe.Rows[0]["SoVe"]) > 0)
            {
                BaoLoi("Không thể xóa toàn bộ ghế vì có ghế đã bán vé!");
                return;
            }

            int ketQua = kn.ThucThi(
                "DELETE FROM Ghe WHERE MaPhong = @MaPhong",
                new SqlParameter[] { new SqlParameter("@MaPhong", maPhong) });
            if (ketQua >= 0)
            {
                BaoThanhCong("Đã xóa toàn bộ ghế của phòng.");
                LoadGhe(maPhong);
                LoadPhong();
            }
            else
            {
                BaoLoi("Có lỗi xảy ra khi xóa ghế!");
            }
        }

        protected void gvGhe_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "DeleteGhe") return;

            int maGhe;
            if (!int.TryParse(e.CommandArgument.ToString(), out maGhe)) return;

            // Chặn xóa ghế đã có vé đặt
            DataTable dtVe = kn.LayKetNoi(
                "SELECT COUNT(*) AS SoVe FROM ChiTietDatVe WHERE MaGhe = @MaGhe",
                new SqlParameter[] { new SqlParameter("@MaGhe", maGhe) });
            if (dtVe != null && Convert.ToInt32(dtVe.Rows[0]["SoVe"]) > 0)
            {
                BaoLoi("Không thể xóa ghế đã được bán vé!");
                return;
            }

            kn.ThucThi("DELETE FROM Ghe WHERE MaGhe = @MaGhe",
                new SqlParameter[] { new SqlParameter("@MaGhe", maGhe) });
            BaoThanhCong("Xóa ghế thành công!");

            int maPhong;
            if (int.TryParse(hdnMaPhongGhe.Value, out maPhong))
            {
                LoadGhe(maPhong);
                LoadPhong();
            }
        }

        protected void btnDongGhe_Click(object sender, EventArgs e)
        {
            pnlGhe.Visible = false;
            hdnMaPhongGhe.Value = "";
        }

        // ══════════════════ TIỆN ÍCH ══════════════════

        private void BaoLoi(string thongBao)
        {
            lblMsg.Text = thongBao;
            lblMsg.ForeColor = System.Drawing.Color.Red;
        }

        private void BaoThanhCong(string thongBao)
        {
            lblMsg.Text = thongBao;
            lblMsg.ForeColor = System.Drawing.Color.Green;
        }
    }
}
