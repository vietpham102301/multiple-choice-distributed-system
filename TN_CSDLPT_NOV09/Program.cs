﻿using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;
using TN_CSDLPT_NOV09.views;

namespace TN_CSDLPT_NOV09
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static SqlConnection conn = new SqlConnection();
        public static String connstr = "";
        public static String connstr_publisher = "Data Source=TUANBUI-NOV09;Initial Catalog=TN_CSDLPT;Integrated Security=True";

        public static SqlDataReader myReader;
        public static String servername = "";
        public static String username = "";
        public static String mlogin = "";
        public static String password = "";

        public static String database = "TN_CSDLPT";
        public static String remoteLogin = "HTKN";
        public static String remotePassword = "123";

        public static String mLoginDN = "";
        public static String passwordDN = "";
        public static String mGroup = "";
        public static String mHoTen = "";
        public static String maCoSo = "";
        public static int indexCoSo = -1;

        public static String maSinhVien = "";
        public static String maLop = "";
        public static String tenLop = "";

        public static BindingSource bds_DanhSachPhanManh = new BindingSource();

        public static FormMain formChinh;
        public static FormDangNhap formDangNhap;
        public static int KetNoi()
        {
            if (Program.conn != null && Program.conn.State == ConnectionState.Open)
                Program.conn.Close();
            try
            {
                Program.connstr = "Data Source=" + Program.servername + ";Initial Catalog=" +
                      Program.database + ";User ID=" +
                      Program.mlogin + ";password=" + Program.password;
                Program.conn.ConnectionString = Program.connstr;
                Program.conn.Open();
                return 1;
            }

            catch (Exception e)
            {
                MessageBox.Show("Lỗi kết nối cơ sở dữ liệu.\nBạn xem lại user name và password.\n " + e.Message, "", MessageBoxButtons.OK);
                return 0;
            }
        }
        public static SqlDataReader ExecSqlDataReader(String strLenh)
        {
            SqlDataReader myreader;
            SqlCommand sqlcmd = new SqlCommand(strLenh, Program.conn);
            sqlcmd.CommandType = CommandType.Text;
            if (Program.conn.State == ConnectionState.Closed) Program.conn.Open();
            try
            {
                myreader = sqlcmd.ExecuteReader(); return myreader;

            }
            catch (SqlException ex)
            {
                Program.conn.Close();
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public static DataTable ExecSqlDataTable(String cmd)
        {
            DataTable dt = new DataTable();
            if (Program.conn.State == ConnectionState.Closed) Program.conn.Open();
            SqlDataAdapter da = new SqlDataAdapter(cmd, conn);
            da.Fill(dt);
            conn.Close();
            return dt;
        }

        public static int ExecSqlNonQuery(String strlenh)
        {

            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            SqlCommand Sqlcmd = new SqlCommand(strlenh, conn);
            Sqlcmd.CommandType = CommandType.Text;
            Sqlcmd.CommandTimeout = 300;// 5 phut 
            try
            {
                Sqlcmd.ExecuteNonQuery();
                return 0;

            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
                conn.Close();
                return ex.State; // lấy trạng thái lỗi gởi từ RAISERROR trong SQL Server qua
            }
        }

        [STAThread]

        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            formChinh = new FormMain();
            formDangNhap = new FormDangNhap();
            Application.Run(formDangNhap);
            //Application.Run(formChinh);
        }
    }
}
