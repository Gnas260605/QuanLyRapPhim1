using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace QuanLyRapPhim
{
    public partial class Phim : System.Web.UI.Page
    {
        KetNoi kn = new KetNoi();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack) return;

            string theLoai = "";
            if (Context.Items["TL"] != null)
            {
                theLoai = Context.Items["TL"].ToString();
            }

            string q;
            if (string.IsNullOrEmpty(theLoai))
            {
                // Lấy toàn bộ danh sách phim
                q = "select * from Phim";
            }
            else
            {
                // Lọc phim theo thể loại
                q = "select * from Phim where TheLoai = N'" + theLoai + "'";
            }

            this.DataList1.DataSource = kn.LayKetNoi(q);
            this.DataList1.DataBind();
        }
    }
}