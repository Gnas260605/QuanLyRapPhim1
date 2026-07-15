using System;
using System.Data;
using System.Data.SqlClient;

namespace QuanLyRapPhim
{
    public partial class DoiMatKhau : SecurePage
    {
        // Bất kỳ tài khoản nào đã đăng nhập đều được đổi mật khẩu của chính mình
        protected override string[] AllowedRoles { get; set; } = new[] { Role.ADMIN, Role.STAFF };
        KetNoi kn = new KetNoi();

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnDoi_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            int maNguoiDung = AuthorizationHelper.GetUserId();
            if (maNguoiDung == 0)
            {
                Response.Redirect("DangNhap.aspx");
                return;
            }

            string matKhauCu = txtMatKhauCu.Text;
            string matKhauMoi = txtMatKhauMoi.Text;

            // Lấy mật khẩu hiện tại trong DB để đối chiếu
            DataTable dt = kn.LayKetNoi(
                "select MatKhau from NguoiDung where MaNguoiDung = @MaNguoiDung",
                new SqlParameter[] { new SqlParameter("@MaNguoiDung", maNguoiDung) });
            if (dt == null || dt.Rows.Count == 0)
            {
                BaoLoi("Không tìm thấy tài khoản!");
                return;
            }

            string matKhauHienTai = dt.Rows[0]["MatKhau"].ToString();
            if (matKhauHienTai != matKhauCu)
            {
                BaoLoi("Mật khẩu hiện tại không đúng!");
                return;
            }
            if (matKhauMoi == matKhauCu)
            {
                BaoLoi("Mật khẩu mới phải khác mật khẩu hiện tại!");
                return;
            }

            int ketQua = kn.ThucThi(
                "update NguoiDung set MatKhau = @MatKhauMoi where MaNguoiDung = @MaNguoiDung",
                new SqlParameter[]
                {
                    new SqlParameter("@MatKhauMoi", matKhauMoi),
                    new SqlParameter("@MaNguoiDung", maNguoiDung)
                });

            if (ketQua > 0)
            {
                lblMsg.ForeColor = System.Drawing.Color.FromArgb(0x48, 0xBB, 0x78);
                lblMsg.Text = "Đổi mật khẩu thành công!";
                txtMatKhauCu.Text = "";
                txtMatKhauMoi.Text = "";
                txtXacNhan.Text = "";
            }
            else
            {
                BaoLoi("Có lỗi xảy ra, không thể đổi mật khẩu!");
            }
        }

        private void BaoLoi(string thongBao)
        {
            lblMsg.ForeColor = System.Drawing.Color.FromArgb(0xE5, 0x3E, 0x3E);
            lblMsg.Text = thongBao;
        }
    }
}
