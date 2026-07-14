using System;
using System.Data;
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
            string maPhim = ddlPhim.SelectedValue;
            string maPhong = ddlPhong.SelectedValue;
            string ngayChieu = txtNgayChieu.Text.Trim();
            string gioChieu = txtGioBatDau.Text.Trim();
            string giaVe = txtGiaVe.Text.Trim();

            if (string.IsNullOrEmpty(hdnMaLichChieu.Value))
            {
                // Thêm mới
                string sqlInsert = $"INSERT INTO LichChieu (MaPhim, MaPhong, NgayChieu, GioBatDau, GiaVeCoBan) VALUES ({maPhim}, {maPhong}, '{ngayChieu}', '{gioChieu}', {giaVe})";
                DataTable dt = kn.LayKetNoi(sqlInsert);
                lblMsg.Text = "Thêm lịch chiếu mới thành công!";
                lblMsg.ForeColor = System.Drawing.Color.Green;
            }
            else
            {
                // Cập nhật
                string sqlUpdate = $"UPDATE LichChieu SET MaPhim = {maPhim}, MaPhong = {maPhong}, NgayChieu = '{ngayChieu}', GioBatDau = '{gioChieu}', GiaVeCoBan = {giaVe} WHERE MaLichChieu = {hdnMaLichChieu.Value}";
                DataTable dt = kn.LayKetNoi(sqlUpdate);
                lblMsg.Text = "Cập nhật lịch chiếu thành công!";
                lblMsg.ForeColor = System.Drawing.Color.Green;
            }

            ResetForm();
            LoadLichChieu();
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