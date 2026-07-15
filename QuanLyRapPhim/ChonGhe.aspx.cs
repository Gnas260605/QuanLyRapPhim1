using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace QuanLyRapPhim
{
    public partial class ChonGhe : System.Web.UI.Page
    {
        KetNoi kn = new KetNoi();

        // Danh sách ghế đã chọn lưu trong ViewState
        private List<int> SelectedSeatIds
        {
            get
            {
                if (ViewState["SelectedSeatIds"] == null)
                {
                    ViewState["SelectedSeatIds"] = new List<int>();
                }
                return (List<int>)ViewState["SelectedSeatIds"];
            }
            set
            {
                ViewState["SelectedSeatIds"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack) return;

            string maLichChieu = Request.QueryString["MaLichChieu"];
            if (string.IsNullOrEmpty(maLichChieu))
            {
                Response.Redirect("Phim.aspx");
                return;
            }

            // Chặn truy cập vào suất chiếu đã diễn ra (hoặc mã suất chiếu không tồn tại)
            if (!KiemTraSuatChieuConHieuLuc(maLichChieu))
            {
                Response.Redirect("Phim.aspx");
                return;
            }

            LoadGhe();
        }

        // Kiểm tra suất chiếu còn hiệu lực (tồn tại và chưa diễn ra)
        private bool KiemTraSuatChieuConHieuLuc(string maLichChieu)
        {
            int maLC;
            if (!int.TryParse(maLichChieu, out maLC)) return false;

            string sql = "select count(*) as SoLuong from LichChieu " +
                         "where MaLichChieu = " + maLC + " " +
                         "and (NgayChieu > CAST(GETDATE() AS date) " +
                         "     or (NgayChieu = CAST(GETDATE() AS date) and GioBatDau > CAST(GETDATE() AS time)))";
            DataTable dt = kn.LayKetNoi(sql);
            return dt != null && dt.Rows.Count > 0 && Convert.ToInt32(dt.Rows[0]["SoLuong"]) > 0;
        }

        // Tải sơ đồ ghế của phòng chiếu
        private void LoadGhe()
        {
            string maLichChieu = Request.QueryString["MaLichChieu"];
            
            // Lấy danh sách ghế của phòng
            string sqlGhe = "select G.* from Ghe G inner join LichChieu LC on G.MaPhong = LC.MaPhong where LC.MaLichChieu = " + maLichChieu;
            DataTable dtGhe = kn.LayKetNoi(sqlGhe);
            
            // Lấy danh sách ghế đã được đặt
            string sqlDatGhe = "select CT.MaGhe from ChiTietDatVe CT inner join DatVe DV on CT.MaDatVe = DV.MaDatVe where DV.MaLichChieu = " + maLichChieu + " and DV.TrangThai = N'Đã thanh toán'";
            DataTable dtDatGhe = kn.LayKetNoi(sqlDatGhe);
            
            List<int> bookedList = new List<int>();
            if (dtDatGhe != null)
            {
                foreach (DataRow r in dtDatGhe.Rows)
                {
                    bookedList.Add(Convert.ToInt32(r["MaGhe"]));
                }
            }
            ViewState["BookedSeatIds"] = bookedList;

            rptGhe.DataSource = dtGhe;
            rptGhe.DataBind();
        }

        // Cập nhật lại sơ đồ khi click chọn ghế
        private void LoadSodoGheUpdate()
        {
            string maLichChieu = Request.QueryString["MaLichChieu"];
            string sqlGhe = "select G.* from Ghe G inner join LichChieu LC on G.MaPhong = LC.MaPhong where LC.MaLichChieu = " + maLichChieu;
            DataTable dtGhe = kn.LayKetNoi(sqlGhe);
            rptGhe.DataSource = dtGhe;
            rptGhe.DataBind();
        }

        // Đổ class CSS màu sắc tương ứng cho ghế
        public string GetSeatClass(string loaiGhe, string maGheStr)
        {
            int maGhe = Convert.ToInt32(maGheStr);
            List<int> bookedList = (List<int>)ViewState["BookedSeatIds"];
            
            if (bookedList != null && bookedList.Contains(maGhe))
            {
                return "seat-button seat-booked";
            }
            if (SelectedSeatIds.Contains(maGhe))
            {
                return "seat-button seat-selected";
            }
            if (loaiGhe.Equals("VIP", StringComparison.OrdinalIgnoreCase))
            {
                return "seat-button seat-vip";
            }
            return "seat-button seat-normal";
        }

        protected void rptGhe_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Button btn = (Button)e.Item.FindControl("btnGhe");
                if (btn != null)
                {
                    int maGhe = Convert.ToInt32(DataBinder.Eval(e.Item.DataItem, "MaGhe"));
                    List<int> bookedList = (List<int>)ViewState["BookedSeatIds"];
                    if (bookedList != null && bookedList.Contains(maGhe))
                    {
                        btn.Enabled = false; // Vô hiệu hóa ghế đã đặt
                    }
                }
            }
        }

        protected void rptGhe_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "ChonGhe")
            {
                int maGhe = Convert.ToInt32(e.CommandArgument);
                List<int> selected = SelectedSeatIds;
                
                if (selected.Contains(maGhe))
                {
                    selected.Remove(maGhe); // Bỏ chọn
                }
                else
                {
                    selected.Add(maGhe); // Thêm chọn
                }
                SelectedSeatIds = selected;

                LoadSodoGheUpdate();
                CapNhatThongTinGia();
            }
        }

        // Tính tiền tạm thời
        private void CapNhatThongTinGia()
        {
            if (SelectedSeatIds.Count == 0)
            {
                lblGheDaChon.Text = "Chưa chọn ghế";
                lblTongTien.Text = "0";
                return;
            }

            string maLichChieu = Request.QueryString["MaLichChieu"];
            
            // Lấy giá vé cơ bản của suất chiếu
            string sqlGia = "select GiaVeCoBan from LichChieu where MaLichChieu = " + maLichChieu;
            DataTable dtGia = kn.LayKetNoi(sqlGia);
            decimal giaCoBan = 80000;
            if (dtGia != null && dtGia.Rows.Count > 0)
            {
                giaCoBan = Convert.ToDecimal(dtGia.Rows[0]["GiaVeCoBan"]);
            }

            // Lấy thông tin các ghế đang chọn
            string ids = string.Join(",", SelectedSeatIds);
            string sqlGheSelected = "select * from Ghe where MaGhe in (" + ids + ")";
            DataTable dtGheSelected = kn.LayKetNoi(sqlGheSelected);

            List<string> tenGheList = new List<string>();
            decimal tongTien = 0;

            if (dtGheSelected != null)
            {
                foreach (DataRow r in dtGheSelected.Rows)
                {
                    tenGheList.Add(r["Hang"].ToString() + r["So"].ToString());
                    string loaiGhe = r["LoaiGhe"].ToString();
                    if (loaiGhe.Equals("VIP", StringComparison.OrdinalIgnoreCase))
                    {
                        tongTien += giaCoBan + 40000; // Ghế VIP phụ thu 40.000đ
                    }
                    else if (loaiGhe.Equals("Đôi", StringComparison.OrdinalIgnoreCase))
                    {
                        tongTien += giaCoBan + 60000; // Ghế Đôi phụ thu 60.000đ
                    }
                    else
                    {
                        tongTien += giaCoBan;
                    }
                }
            }

            lblGheDaChon.Text = string.Join(", ", tenGheList);
            lblTongTien.Text = tongTien.ToString("N0");
            
            Session["TongTien"] = tongTien;
            Session["GheDaChonIds"] = SelectedSeatIds;
            Session["TenGheDaChon"] = lblGheDaChon.Text;
        }

        protected void btnTiepTuc_Click(object sender, EventArgs e)
        {
            if (SelectedSeatIds.Count == 0)
            {
                lblThongBao.Text = "Vui lòng chọn ít nhất 1 ghế!";
                return;
            }

            // Kiểm tra lại suất chiếu còn hiệu lực (người dùng có thể mở trang quá lâu)
            if (!KiemTraSuatChieuConHieuLuc(Request.QueryString["MaLichChieu"]))
            {
                lblThongBao.Text = "Suất chiếu này đã bắt đầu, không thể đặt vé nữa!";
                return;
            }

            // Kiểm tra đã đăng nhập chưa
            if (Session["MaNguoiDung"] == null)
            {
                Session["ReturnUrl"] = Request.RawUrl;
                Response.Redirect("DangNhap.aspx");
                return;
            }

            Session["MaLichChieu"] = Request.QueryString["MaLichChieu"];
            Response.Redirect("ThanhToan.aspx");
        }
    }
}