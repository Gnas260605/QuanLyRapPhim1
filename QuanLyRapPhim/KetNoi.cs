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

        /// <summary>
        /// Truy vấn có tham số (parameterized query) để chống SQL Injection.
        /// Dùng cho các câu SQL có chứa dữ liệu người dùng nhập vào.
        /// </summary>
        public DataTable LayKetNoi(string q, SqlParameter[] parameters)
        {
            DataTable dt = new DataTable();
            try
            {
                moKetNoi();
                SqlCommand cmd = new SqlCommand(q, connect);
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                SqlDataAdapter da = new SqlDataAdapter(cmd);
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

        /// <summary>
        /// Thực thi câu lệnh INSERT/UPDATE/DELETE có tham số.
        /// Trả về số dòng bị ảnh hưởng, -1 nếu có lỗi.
        /// </summary>
        public int ThucThi(string q, SqlParameter[] parameters)
        {
            try
            {
                moKetNoi();
                SqlCommand cmd = new SqlCommand(q, connect);
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                return cmd.ExecuteNonQuery();
            }
            catch
            {
                return -1;
            }
            finally
            {
                dongKetNoi();
            }
        }
    }
}
