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
    public partial class AdminPhim : SecurePage
    {
        // Chỉ Admin và Staff mới được phép truy cập trang này
        protected override string[] AllowedRoles { get; set; } = new[] { Role.ADMIN, Role.STAFF };

        KetNoi kn = new KetNoi();

        private string GetConnectionString()
        {
            string dbPath = Server.MapPath("~/App_Data/Database1.mdf");
            return $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={dbPath};Integrated Security=True";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadPhim();
            }
        }

        private void LoadPhim()
        {
            string sql = "select * from Phim order by MaPhim desc";
            DataTable dt = kn.LayKetNoi(sql);
            if (dt != null)
            {
                gvPhim.DataSource = dt;
                gvPhim.DataBind();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            string tenPhim = txtTenPhim.Text.Trim();
            string theLoai = txtTheLoai.Text.Trim();
            int thoiLuong = Convert.ToInt32(txtThoiLuong.Text.Trim());
            string daoDien = txtDaoDien.Text.Trim();
            string moTa = txtMoTa.Text.Trim();
            bool laThemMoi = string.IsNullOrEmpty(hdnMaPhim.Value);
            DateTime ngayKhoiChieu;

            if (!DateTime.TryParse(txtNgayKhoiChieu.Text.Trim(), out ngayKhoiChieu))
            {
                lblMsg.ForeColor = System.Drawing.Color.Red;
                lblMsg.Text = "Định dạng ngày không hợp lệ! Hãy nhập kiểu yyyy-MM-dd.";
                return;
            }

            // Xử lý ảnh: thêm mới bắt buộc chọn ảnh; khi sửa không chọn thì giữ ảnh cũ
            string hinhAnh;
            if (fuHinhAnh.HasFile)
            {
                if (!XuLyLuuAnh(out hinhAnh))
                {
                    return; // Thông báo lỗi đã được đặt trong XuLyLuuAnh
                }
            }
            else if (laThemMoi)
            {
                lblMsg.ForeColor = System.Drawing.Color.Red;
                lblMsg.Text = "Vui lòng chọn ảnh cho phim!";
                return;
            }
            else
            {
                hinhAnh = hdnHinhAnhCu.Value; // Giữ nguyên ảnh cũ
            }

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;

                if (string.IsNullOrEmpty(hdnMaPhim.Value))
                {
                    // Chức năng: Thêm phim mới
                    cmd.CommandText = "insert into Phim (TenPhim, TheLoai, ThoiLuong, DaoDien, NgayKhoiChieu, MoTa, HinhAnh) values (@TenPhim, @TheLoai, @ThoiLuong, @DaoDien, @NgayKhoiChieu, @MoTa, @HinhAnh)";
                }
                else
                {
                    // Chức năng: Cập nhật phim
                    cmd.CommandText = "update Phim set TenPhim=@TenPhim, TheLoai=@TheLoai, ThoiLuong=@ThoiLuong, DaoDien=@DaoDien, NgayKhoiChieu=@NgayKhoiChieu, MoTa=@MoTa, HinhAnh=@HinhAnh where MaPhim=@MaPhim";
                    cmd.Parameters.AddWithValue("@MaPhim", Convert.ToInt32(hdnMaPhim.Value));
                }

                cmd.Parameters.AddWithValue("@TenPhim", tenPhim);
                cmd.Parameters.AddWithValue("@TheLoai", theLoai);
                cmd.Parameters.AddWithValue("@ThoiLuong", thoiLuong);
                cmd.Parameters.AddWithValue("@DaoDien", daoDien);
                cmd.Parameters.AddWithValue("@NgayKhoiChieu", ngayKhoiChieu);
                cmd.Parameters.AddWithValue("@MoTa", moTa);
                cmd.Parameters.AddWithValue("@HinhAnh", hinhAnh);

                int result = cmd.ExecuteNonQuery();
                if (result > 0)
                {
                    lblMsg.ForeColor = System.Drawing.Color.Green;
                    lblMsg.Text = string.IsNullOrEmpty(hdnMaPhim.Value) ? "Thêm phim mới thành công!" : "Cập nhật phim thành công!";
                    ResetForm();
                    LoadPhim();
                }
                else
                {
                    lblMsg.ForeColor = System.Drawing.Color.Red;
                    lblMsg.Text = "Có lỗi xảy ra, không thể lưu dữ liệu!";
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ResetForm();
            lblMsg.Text = "";
        }

        protected void gvPhim_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int maPhim = Convert.ToInt32(e.CommandArgument);
            lblMsg.Text = ""; // Xóa thông báo cũ khi bắt đầu thao tác mới

            if (e.CommandName == "EditMovie")
            {
                // Chức năng: Chọn phim để sửa
                string sql = "select * from Phim where MaPhim = " + maPhim;
                DataTable dt = kn.LayKetNoi(sql);
                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow r = dt.Rows[0];
                    hdnMaPhim.Value = r["MaPhim"].ToString();
                    txtTenPhim.Text = r["TenPhim"].ToString();
                    txtTheLoai.Text = r["TheLoai"].ToString();
                    txtThoiLuong.Text = r["ThoiLuong"].ToString();
                    txtDaoDien.Text = r["DaoDien"].ToString();
                    txtNgayKhoiChieu.Text = Convert.ToDateTime(r["NgayKhoiChieu"]).ToString("yyyy-MM-dd");
                    txtMoTa.Text = r["MoTa"].ToString();

                    // Ảnh: lưu tên ảnh cũ để giữ lại nếu không upload ảnh mới
                    hdnHinhAnhCu.Value = r["HinhAnh"].ToString();
                    lblHinhHienTai.Text = "Ảnh hiện tại: " + r["HinhAnh"].ToString() + " (để trống nếu không đổi ảnh)";

                    lblFormTitle.Text = "Cập nhật thông tin phim (Mã: " + maPhim + ")";
                    btnSave.Text = "Cập nhật";
                }
            }
            else if (e.CommandName == "DeleteMovie")
            {
                // Chức năng: Xóa phim
                using (SqlConnection conn = new SqlConnection(GetConnectionString()))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("delete from Phim where MaPhim = @MaPhim", conn);
                    cmd.Parameters.AddWithValue("@MaPhim", maPhim);
                    try
                    {
                        cmd.ExecuteNonQuery();
                        lblMsg.ForeColor = System.Drawing.Color.Green;
                        lblMsg.Text = "Xóa phim thành công!";
                        ResetForm();
                        LoadPhim();
                    }
                    catch (SqlException ex)
                    {
                        lblMsg.ForeColor = System.Drawing.Color.Red;
                        // Phim có thể có suất chiếu ràng buộc khóa ngoại
                        if (ex.Number == 547)
                        {
                            lblMsg.Text = "Không thể xóa phim này vì đã có Suất chiếu được xếp lịch!";
                        }
                        else
                        {
                            lblMsg.Text = "Lỗi khi xóa phim: " + ex.Message;
                        }
                    }
                }
            }
        }

        private void ResetForm()
        {
            hdnMaPhim.Value = "";
            txtTenPhim.Text = "";
            txtTheLoai.Text = "";
            txtThoiLuong.Text = "";
            txtDaoDien.Text = "";
            txtNgayKhoiChieu.Text = "";
            hdnHinhAnhCu.Value = "";
            lblHinhHienTai.Text = "";
            txtMoTa.Text = "";
            lblFormTitle.Text = "Thêm phim mới";
            btnSave.Text = "Lưu Phim";
        }

        /// <summary>
        /// Kiểm tra và lưu ảnh upload vào thư mục ~/img/.
        /// Trả về true nếu lưu thành công, tên file lưu trong tham số out hinhAnh.
        /// </summary>
        private bool XuLyLuuAnh(out string hinhAnh)
        {
            hinhAnh = "";

            // 1. Kiểm tra phần mở rộng hợp lệ
            string[] phanMoRongChoPhep = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            string phanMoRong = System.IO.Path.GetExtension(fuHinhAnh.FileName).ToLower();
            if (Array.IndexOf(phanMoRongChoPhep, phanMoRong) < 0)
            {
                lblMsg.ForeColor = System.Drawing.Color.Red;
                lblMsg.Text = "Chỉ chấp nhận ảnh định dạng jpg, jpeg, png, gif, webp!";
                return false;
            }

            // 2. Giới hạn dung lượng 4MB
            if (fuHinhAnh.PostedFile.ContentLength > 4 * 1024 * 1024)
            {
                lblMsg.ForeColor = System.Drawing.Color.Red;
                lblMsg.Text = "Ảnh vượt quá dung lượng cho phép (tối đa 4MB)!";
                return false;
            }

            // 3. Tạo tên file an toàn, tránh trùng bằng cách thêm mốc thời gian
            string tenGoc = System.IO.Path.GetFileNameWithoutExtension(fuHinhAnh.FileName);
            string tenAnToan = System.Text.RegularExpressions.Regex.Replace(tenGoc, @"[^a-zA-Z0-9_-]", "");
            if (string.IsNullOrEmpty(tenAnToan)) tenAnToan = "phim";
            string tenFile = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + tenAnToan + phanMoRong;

            // 4. Lưu file vào thư mục img
            try
            {
                string duongDan = Server.MapPath("~/img/" + tenFile);
                fuHinhAnh.SaveAs(duongDan);
                hinhAnh = tenFile;
                return true;
            }
            catch (Exception ex)
            {
                lblMsg.ForeColor = System.Drawing.Color.Red;
                lblMsg.Text = "Lỗi khi lưu ảnh: " + ex.Message;
                return false;
            }
        }
    }
}
