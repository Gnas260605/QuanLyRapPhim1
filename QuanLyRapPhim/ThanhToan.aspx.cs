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
    public partial class ThanhToan : System.Web.UI.Page
    {
        KetNoi kn = new KetNoi();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack) return;

            // Kiểm tra thông tin đặt vé trong Session
            if (Session["MaNguoiDung"] == null || Session["MaLichChieu"] == null || Session["GheDaChonIds"] == null)
            {
                Response.Redirect("Phim.aspx");
                return;
            }

            lblKhachHang.Text = Session["HoTen"].ToString();
            lblGheChon.Text = Session["TenGheDaChon"].ToString();
            lblTongTien.Text = Convert.ToDecimal(Session["TongTien"]).ToString("N0");

            // Tải thông tin suất chiếu
            string maLichChieu = Session["MaLichChieu"].ToString();
            string sqlLC = "select LC.NgayChieu, LC.GioBatDau, PC.TenPhong, P.TenPhim from LichChieu LC inner join PhongChieu PC on LC.MaPhong = PC.MaPhong inner join Phim P on LC.MaPhim = P.MaPhim where LC.MaLichChieu = " + maLichChieu;
            DataTable dtLC = kn.LayKetNoi(sqlLC);
            if (dtLC != null && dtLC.Rows.Count > 0)
            {
                DataRow r = dtLC.Rows[0];
                lblTenPhim.Text = r["TenPhim"].ToString();
                lblSuatChieu.Text = Convert.ToDateTime(r["NgayChieu"]).ToString("dd/MM/yyyy") + " - Lúc: " + r["GioBatDau"].ToString();
                lblPhongChieu.Text = r["TenPhong"].ToString();
            }
        }

        protected void btnXacNhan_Click(object sender, EventArgs e)
        {
            if (Session["MaNguoiDung"] == null || Session["MaLichChieu"] == null || Session["GheDaChonIds"] == null)
            {
                lblThongBao.Text = "Phiên giao dịch đã hết hạn, vui lòng đặt vé lại!";
                return;
            }

            int maNguoiDung = Convert.ToInt32(Session["MaNguoiDung"]);
            int maLichChieu = Convert.ToInt32(Session["MaLichChieu"]);
            decimal tongTien = Convert.ToDecimal(Session["TongTien"]);
            List<int> listGhe = (List<int>)Session["GheDaChonIds"];

            string dbPath = HttpContext.Current.Server.MapPath("~/App_Data/Database1.mdf");
            string connStr = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={dbPath};Integrated Security=True";

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();

                try
                {
                    // 1. Thêm vào bảng DatVe
                    string sqlDatVe = "insert into DatVe (MaNguoiDung, MaLichChieu, NgayDat, TongTien, TrangThai) values (@MaNguoiDung, @MaLichChieu, GETDATE(), @TongTien, N'Đã thanh toán'); SELECT SCOPE_IDENTITY();";
                    int maDatVe = 0;
                    using (SqlCommand cmd = new SqlCommand(sqlDatVe, conn, trans))
                    {
                        cmd.Parameters.AddWithValue("@MaNguoiDung", maNguoiDung);
                        cmd.Parameters.AddWithValue("@MaLichChieu", maLichChieu);
                        cmd.Parameters.AddWithValue("@TongTien", tongTien);
                        
                        maDatVe = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // 2. Thêm vào bảng ChiTietDatVe cho từng ghế
                    string sqlLC = "select GiaVeCoBan from LichChieu where MaLichChieu = " + maLichChieu;
                    decimal giaCoBan = 80000;
                    using (SqlCommand cmdLC = new SqlCommand(sqlLC, conn, trans))
                    {
                        object obj = cmdLC.ExecuteScalar();
                        if (obj != null) giaCoBan = Convert.ToDecimal(obj);
                    }

                    foreach (int maGhe in listGhe)
                    {
                        string sqlLoaiGhe = "select LoaiGhe from Ghe where MaGhe = " + maGhe;
                        string loaiGhe = "Thường";
                        using (SqlCommand cmdLoai = new SqlCommand(sqlLoaiGhe, conn, trans))
                        {
                            object obj = cmdLoai.ExecuteScalar();
                            if (obj != null) loaiGhe = obj.ToString();
                        }

                        decimal giaVeGhe = giaCoBan;
                        if (loaiGhe.Equals("VIP", StringComparison.OrdinalIgnoreCase)) giaVeGhe += 40000;
                        else if (loaiGhe.Equals("Đôi", StringComparison.OrdinalIgnoreCase)) giaVeGhe += 60000;

                        string sqlChiTiet = "insert into ChiTietDatVe (MaDatVe, MaGhe, GiaVe) values (@MaDatVe, @MaGhe, @GiaVe)";
                        using (SqlCommand cmdDetail = new SqlCommand(sqlChiTiet, conn, trans))
                        {
                            cmdDetail.Parameters.AddWithValue("@MaDatVe", maDatVe);
                            cmdDetail.Parameters.AddWithValue("@MaGhe", maGhe);
                            cmdDetail.Parameters.AddWithValue("@GiaVe", giaVeGhe);
                            cmdDetail.ExecuteNonQuery();
                        }
                    }

                    trans.Commit();
                    
                    lblThongBao.Text = "";
                    lblThanhCong.Text = "Thanh toán thành công! Vé đã được đặt.";

                    // Xóa thông tin đặt vé trong Session
                    Session["MaLichChieu"] = null;
                    Session["GheDaChonIds"] = null;
                    Session["TenGheDaChon"] = null;
                    Session["TongTien"] = null;

                    // Chuyển hướng sang trang lịch sử đặt vé sau 2 giây
                    Response.Write("<script>setTimeout(function(){ window.location.href='LichSuDatVe.aspx'; }, 2000);</script>");
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    lblThongBao.Text = "Lỗi thanh toán: " + ex.Message;
                    lblThanhCong.Text = "";
                }
            }
        }

        protected void btnHuy_Click(object sender, EventArgs e)
        {
            // Hủy giao dịch, quay lại danh sách phim
            Session["MaLichChieu"] = null;
            Session["GheDaChonIds"] = null;
            Session["TenGheDaChon"] = null;
            Session["TongTien"] = null;
            Response.Redirect("Phim.aspx");
        }
    }
}