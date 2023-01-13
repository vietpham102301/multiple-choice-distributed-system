﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraReports.UI;

namespace TN_CSDLPT_NOV09.views
{
    public partial class FormRpBangDiemMonHoc : DevExpress.XtraEditors.XtraForm
    {
        public FormRpBangDiemMonHoc()
        {
            InitializeComponent();
        }

        private void mONHOCBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.bindingSourceMonHoc.EndEdit();
            this.tableAdapterManager.UpdateAll(this.TN_CSDLPT_DataSet);

        }

        private void FormRpBangDiemMonHoc_Load(object sender, EventArgs e)
        {
            TN_CSDLPT_DataSet.EnforceConstraints = false;
            // TODO: This line of code loads data into the 'tN_CSDLPT_DataSet.LOP' table. You can move, or remove it, as needed.
            this.tableAdapterLop.Connection.ConnectionString = Program.connstr;
            this.tableAdapterLop.Fill(this.TN_CSDLPT_DataSet.LOP);
            // TODO: This line of code loads data into the 'tN_CSDLPT_DataSet.MONHOC' table. You can move, or remove it, as needed.
            this.tableAdapterMonHoc.Connection.ConnectionString = Program.connstr;
            this.tableAdapterMonHoc.Fill(this.TN_CSDLPT_DataSet.MONHOC);

            //load dữ liệu vào combobox cơ sở
            comboBoxCoSo.DataSource = Program.bds_DanhSachPhanManh;
            comboBoxCoSo.DisplayMember = "TENCS";
            comboBoxCoSo.ValueMember = "TENSERVER";
            comboBoxCoSo.SelectedIndex = Program.indexCoSo;
            //chỉ trường mới có quyền xem trên cơ sở khác
            if (Program.mGroup == "TRUONG")
            {
                comboBoxCoSo.Enabled = true;
            }
            else
            {
                comboBoxCoSo.Enabled = false;

            }
        }

        private void comboBoxCoSo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxCoSo.SelectedValue.ToString() == "System.Data.DataRowView")
            {
                return;
            }

            Program.servername = comboBoxCoSo.SelectedValue.ToString();
            if (comboBoxCoSo.SelectedIndex != Program.indexCoSo)
            {
                Program.mlogin = Program.remoteLogin;
                Program.password = Program.remotePassword;
            }
            else
            {
                Program.mlogin = Program.mLoginDN;
                Program.password = Program.passwordDN;
            }
            if (Program.KetNoi() == 0)
            {
                MessageBox.Show("Lỗi kết nối tới cơ sở " + comboBoxCoSo.Text, "", MessageBoxButtons.OK);
                return;
            }
            else
            {
                TN_CSDLPT_DataSet.EnforceConstraints = false;
                // TODO: This line of code loads data into the 'tN_CSDLPT_DataSet.LOP' table. You can move, or remove it, as needed.
                this.tableAdapterLop.Connection.ConnectionString = Program.connstr;
                this.tableAdapterLop.Fill(this.TN_CSDLPT_DataSet.LOP);
                // TODO: This line of code loads data into the 'tN_CSDLPT_DataSet.MONHOC' table. You can move, or remove it, as needed.
                //vif môn học nhân bản nên khi chuyển cơ sở dữ liệu trong combobox cũng k thay đổi
                //this.tableAdapterMonHoc.Connection.ConnectionString = Program.connstr;
                //this.tableAdapterMonHoc.Fill(this.TN_CSDLPT_DataSet.MONHOC);

                //Dùng sau
                //maCoSo = ((DataRowView)bindingSourceMonHoc[0])["MACS"].ToString();
            }
        }

        private void buttonThoat_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void buttonPreview_Click(object sender, EventArgs e)
        {
            //String tenMon = "";
            String tenLop = "";
            String maLop = "";

            String strLenh = "EXEC SP_REPORT_BANGDIEM_MONHOC_THONGTIN_LOPMON '" + comboBoxMaLop.SelectedValue.ToString() + "', '" +
            comboBoxMaMonHoc.SelectedValue.ToString() + "', " + spinEditLan.Value;
            try
            {
                Program.myReader = Program.ExecSqlDataReader(strLenh);
                Program.myReader.Read();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Không tìm thấy sinh viên, hãy thử lại\n" + ex.Message, "", MessageBoxButtons.OK);
                Program.myReader.Close();
                Program.conn.Close();
                return;
            }
            try
            {
                maLop = Program.myReader.GetString(0).Trim();
                tenLop = Program.myReader.GetString(1).Trim();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không tìm thấy");
                return;
            }
            finally
            {
                Program.myReader.Close();
                Program.conn.Close();
            }

            XtraReportBangDiemMonHoc xtraReportBangDiemMonHoc = new XtraReportBangDiemMonHoc(comboBoxMaLop.SelectedValue.ToString().Trim()
                                                              , comboBoxMaMonHoc.SelectedValue.ToString().Trim()
                                                              , Decimal.ToInt16(spinEditLan.Value));
            xtraReportBangDiemMonHoc.labelTieuDe.Text = "BẢNG ĐIỂM MÔN " + this.comboBoxMaMonHoc.Text.Trim() + " CỦA LỚP " + tenLop;
            //xtraReportBangDiemMonHoc.xrLabelHoTen.Text = hoTenSinhVien;
            xtraReportBangDiemMonHoc.xrLabelLop.Text = tenLop;
            xtraReportBangDiemMonHoc.xrLabelMaLop.Text = maLop;

            //xtraReportBangDiemMonHoc.xrLabelNgayThi.Text = DateTime.Now.ToString("dd/MM/yyyy") + "cần hỏi thầy ngày là lấy ngày của gv đăng kí?";
            xtraReportBangDiemMonHoc.xrLabelMonThi.Text = this.comboBoxMaMonHoc.Text.Trim();
            //xtraReportBangDiemMonHoc.xrLabelLan.Text = this.spinEditLan.Value.ToString();

            ReportPrintTool printTool = new ReportPrintTool(xtraReportBangDiemMonHoc);
            printTool.ShowPreviewDialog();
        }
    }
}