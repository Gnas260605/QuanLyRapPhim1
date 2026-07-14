using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace QuanLyRapPhim
{
    public class KetNoi
    {
        SqlConnection connect;
        public void moKetNoi()
        {
            string chuoiKetNoi = HttpContext.Current.Server.MapPath("~/App_Data/Database1.mdf");
            string cnn = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={chuoiKetNoi};Integrated Security=True";
            connect = new SqlConnection(cnn);
            connect.Open();
        }
        public void dongKetNoi()
        {
            if (connect != null && connect.State == ConnectionState.Open)
            {
                connect.Close();
            }
        }
        public DataTable LayKetNoi(string q)
        {
            DataTable dt = new DataTable();
            try
            {
                moKetNoi();
                SqlDataAdapter da = new SqlDataAdapter(q, connect);
                da.Fill(dt);

            }
            catch
            {
                dt = null;
            }
            finally
            {
                dongKetNoi();

            }
            return dt;
        }
    }
}
