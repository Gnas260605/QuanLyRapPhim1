using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace QuanLyRapPhim
{
    public partial class LoaiPhim : System.Web.UI.MasterPage
    {
        KetNoi kn = new KetNoi();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack) return;
            // Lấy danh sách thể loại phim
            string q = "select distinct TheLoai from Phim";
            this.DataList1.DataSource = kn.LayKetNoi(q);
            this.DataList1.DataBind();
        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            // Lọc phim theo thể loại
            string theLoai = ((LinkButton)sender).CommandArgument;
            Context.Items["TL"] = theLoai;
            Server.Transfer("Phim.aspx");
        }
    }
}