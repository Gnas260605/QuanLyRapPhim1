using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace QuanLyRapPhim
{
    public partial class QuanLyNguoiDung : SecurePage
    {
        // Chỉ Admin mới được phép vào trang quản lý tài khoản người dùng
        protected override string[] AllowedRoles { get; set; } = new[] { Role.ADMIN };
        KetNoi kn = new KetNoi();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
            LoadNguoiDung();
        }

        private void LoadNguoiDung()
        {
            DataTable dt = kn.LayKetNoi("SELECT MaNguoiDung, HoTen, Email, SoDienThoai, VaiTro FROM NguoiDung ORDER BY VaiTro, HoTen");
            if (dt != null)
            {
                gvNguoiDung.DataSource = dt;
                gvNguoiDung.DataBind();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            // Bảo vệ tầng backend: chỉ Admin mới được thực hiện lưu
            if (AuthorizationHelper.GetUserRole() != Role.ADMIN)
            {
                lblMsg.Text = "Bạn không có quyền thực hiện hành động này!";
                lblMsg.ForeColor = System.Drawing.Color.Red;
                return;
            }

            string hoTen = txtHoTen.Text.Trim();
            string email = txtEmail.Text.Trim();
            string sdt = txtSoDienThoai.Text.Trim();
            string matKhau = txtMatKhau.Text.Trim();
            string vaiTro = ddlVaiTro.SelectedValue;

            if (string.IsNullOrEmpty(hdnMaNguoiDung.Value))
            {
                // Thêm mới (Mật khẩu bắt buộc)
                if (string.IsNullOrEmpty(matKhau))
                {
                    lblMsg.Text = "Mật khẩu không được để trống khi tạo mới!";
                    lblMsg.ForeColor = System.Drawing.Color.Red;
                    return;
                }

                // Kiểm tra email trùng (dùng parameterized query chống SQL Injection)
                DataTable dtCheck = kn.LayKetNoi(
                    "SELECT COUNT(*) AS SoTK FROM NguoiDung WHERE Email = @Email",
                    new SqlParameter[] { new SqlParameter("@Email", email) });
                if (dtCheck != null && Convert.ToInt32(dtCheck.Rows[0]["SoTK"]) > 0)
                {
                    lblMsg.Text = "Email này đã được sử dụng!";
                    lblMsg.ForeColor = System.Drawing.Color.Red;
                    return;
                }

                string sqlInsert = "INSERT INTO NguoiDung (HoTen, Email, SoDienThoai, MatKhau, VaiTro) VALUES (@HoTen, @Email, @SoDienThoai, @MatKhau, @VaiTro)";
                kn.ThucThi(sqlInsert, new SqlParameter[]
                {
                    new SqlParameter("@HoTen", hoTen),
                    new SqlParameter("@Email", email),
                    new SqlParameter("@SoDienThoai", sdt),
                    new SqlParameter("@MatKhau", matKhau),
                    new SqlParameter("@VaiTro", vaiTro)
                });
                lblMsg.Text = "Thêm tài khoản thành công!";
                lblMsg.ForeColor = System.Drawing.Color.Green;
            }
            else
            {
                // Cập nhật (dùng parameterized query chống SQL Injection)
                string sqlUpdate;
                if (!string.IsNullOrEmpty(matKhau))
                {
                    // Có đổi mật khẩu mới
                    sqlUpdate = "UPDATE NguoiDung SET HoTen = @HoTen, Email = @Email, SoDienThoai = @SoDienThoai, MatKhau = @MatKhau, VaiTro = @VaiTro WHERE MaNguoiDung = @MaNguoiDung";
                }
                else
                {
                    // Giữ nguyên mật khẩu cũ
                    sqlUpdate = "UPDATE NguoiDung SET HoTen = @HoTen, Email = @Email, SoDienThoai = @SoDienThoai, VaiTro = @VaiTro WHERE MaNguoiDung = @MaNguoiDung";
                }

                var listParams = new System.Collections.Generic.List<SqlParameter>
                {
                    new SqlParameter("@HoTen", hoTen),
                    new SqlParameter("@Email", email),
                    new SqlParameter("@SoDienThoai", sdt),
                    new SqlParameter("@VaiTro", vaiTro),
                    new SqlParameter("@MaNguoiDung", Convert.ToInt32(hdnMaNguoiDung.Value))
                };
                if (!string.IsNullOrEmpty(matKhau))
                {
                    listParams.Add(new SqlParameter("@MatKhau", matKhau));
                }

                kn.ThucThi(sqlUpdate, listParams.ToArray());
                lblMsg.Text = "Cập nhật tài khoản thành công!";
                lblMsg.ForeColor = System.Drawing.Color.Green;
            }

            ResetForm();
            LoadNguoiDung();
        }

        protected void gvNguoiDung_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (AuthorizationHelper.GetUserRole() != Role.ADMIN) return;

            string maND = e.CommandArgument.ToString();

            if (e.CommandName == "EditND")
            {
                DataTable dt = kn.LayKetNoi($"SELECT * FROM NguoiDung WHERE MaNguoiDung = {maND}");
                if (dt != null && dt.Rows.Count > 0)
                {
                    hdnMaNguoiDung.Value = maND;
                    txtHoTen.Text = dt.Rows[0]["HoTen"].ToString();
                    txtEmail.Text = dt.Rows[0]["Email"].ToString();
                    txtSoDienThoai.Text = dt.Rows[0]["SoDienThoai"].ToString();
                    ddlVaiTro.SelectedValue = dt.Rows[0]["VaiTro"].ToString();
                    txtMatKhau.Text = ""; // Không hiển thị mật khẩu cũ
                    rfvMatKhau.Enabled = false; // Sửa thì không bắt buộc nhập mật khẩu mới
                    lblFormTitle.Text = "Sửa tài khoản #" + maND;
                }
            }
            else if (e.CommandName == "DeleteND")
            {
                // Không cho phép tự xóa tài khoản của chính mình đang đăng nhập
                string currentUserId = Session["MaNguoiDung"]?.ToString();
                if (maND == currentUserId)
                {
                    lblMsg.Text = "Không được xóa tài khoản của chính mình đang đăng nhập!";
                    lblMsg.ForeColor = System.Drawing.Color.Red;
                    return;
                }

                // Kiểm tra xem tài khoản này đã có lịch sử đặt vé chưa
                DataTable dtCheck = kn.LayKetNoi($"SELECT COUNT(*) AS SoVe FROM DatVe WHERE MaNguoiDung = {maND}");
                if (dtCheck != null && Convert.ToInt32(dtCheck.Rows[0]["SoVe"]) > 0)
                {
                    lblMsg.Text = "Không thể xóa tài khoản đã có lịch sử giao dịch đặt vé!";
                    lblMsg.ForeColor = System.Drawing.Color.Red;
                    return;
                }

                kn.LayKetNoi($"DELETE FROM NguoiDung WHERE MaNguoiDung = {maND}");
                lblMsg.Text = "Xóa tài khoản thành công!";
                lblMsg.ForeColor = System.Drawing.Color.Green;
                LoadNguoiDung();
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        private void ResetForm()
        {
            hdnMaNguoiDung.Value = "";
            txtHoTen.Text = "";
            txtEmail.Text = "";
            txtSoDienThoai.Text = "";
            txtMatKhau.Text = "";
            ddlVaiTro.SelectedIndex = 0;
            rfvMatKhau.Enabled = true;
            lblFormTitle.Text = "Thêm tài khoản mới";
        }
    }
}