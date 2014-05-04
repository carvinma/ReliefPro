using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data.OleDb;
using System.Data;

namespace ReliefProMain.View
{
    /// <summary>
    /// OptionEquipment.xaml 的交互逻辑
    /// </summary>
    public partial class SelectEquipmentView :Window
    {
        public SelectEquipmentView()
        {
            InitializeComponent();
        }

        public string dbPlantFile;
        public string eqType;
        public string connectString;
        public string przFile;
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (cbxFilePath.SelectedIndex == -1)
            {
                MessageBox.Show("Select file path");
                return;
            }
            if (cbxStream.SelectedIndex == -1)
            {
                MessageBox.Show("Select equipment");
                return;
            }
            try
            {
                string filename = cbxFilePath.SelectedItem.ToString();
                string eqname = cbxStream.SelectedItem.ToString();
                OleDbConnection conn = new OleDbConnection(connectString);
                string sql = "select * from tbproiieqdata where sourcefile='" + filename + "' and eqname='" + eqname + "'";
                OleDbCommand cmd = new OleDbCommand(sql, conn);
                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                DataSet dsStreamInfo = new DataSet();
                da.Fill(dsStreamInfo);
                if (Application.Current.Properties.Contains("eqInfo"))
                    Application.Current.Properties.Remove("eqInfo");
                Application.Current.Properties.Add("eqInfo", dsStreamInfo);
                przFile = filename;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            this.DialogResult = true;

        }
        private void loadFileData()
        {
            OleDbConnection conn = new OleDbConnection(connectString);
            string sql = "select distinct sourcefile from tbproiieqdata";
            OleDbCommand cmd = new OleDbCommand(sql, conn);
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    cbxFilePath.Items.Add(dr[0]);
                }
            }
            if (cbxFilePath.Items.Count > 0)
                cbxFilePath.SelectedIndex = 0;

        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            connectString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + dbPlantFile + ";Persist Security Info=False;";
            loadFileData();
        }

        private void cbxFilePath_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string filename = cbxFilePath.SelectedItem.ToString();
            cbxStream.Items.Clear();
            if (cbxFilePath.SelectedIndex > -1)
            {
                OleDbConnection conn = new OleDbConnection(connectString);
                string sql = "select distinct eqname from tbproiieqdata where sourcefile='" + filename + "' and eqtype='" + eqType + "'";
                OleDbCommand cmd = new OleDbCommand(sql, conn);
                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        this.cbxStream.Items.Add(dr[0]);
                    }
                }
                if (cbxStream.Items.Count > 0)
                    cbxStream.SelectedIndex = 0;
            }
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
