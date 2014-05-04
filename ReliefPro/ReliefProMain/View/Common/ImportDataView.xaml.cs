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
using System.IO;
using System.Data.OleDb;
using System.Collections.ObjectModel;
using System.Data;
using System.Collections;
using System.ComponentModel;
using ProII;
using ImportLib;
using ProII91;

namespace ReliefProMain.View
{
    /// <summary>
    /// ImportData.xaml 的交互逻辑
    /// </summary>
    public partial class ImportDataView : Window
    {
        ArrayList eqList = new ArrayList();
        ArrayList streamList = new ArrayList();
        BackgroundWorker backgroundWorker = new BackgroundWorker();
       
        public ImportDataView()
        {
            InitializeComponent();
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.DoWork += backgroundWorker_DoWork;
            backgroundWorker.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;
            backgroundWorker.ProgressChanged+=backgroundWorker_ProgressChanged;
        }
        public string dirInfo = string.Empty;
        string dbFile = string.Empty;
        string selectedFile = string.Empty;
        string selectedFileName = string.Empty;       
        string curprzFile = string.Empty;
        
        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlgOpenDiagram = new Microsoft.Win32.OpenFileDialog();
            if (r1.IsChecked==true)
            {
                dlgOpenDiagram.Filter = "PRO/II(*.prz) |*.prz";
                if (dlgOpenDiagram.ShowDialog() == true)
                {
                    this.txtSourceFile.Text = dlgOpenDiagram.FileName;
                    selectedFile = dlgOpenDiagram.FileName;
                    selectedFileName = dlgOpenDiagram.SafeFileName;
                }
            }
            
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.txtSourceFile.Text == string.Empty)
                {
                    btnImport.BorderBrush = Brushes.Red;
                    btnImport.BorderThickness = new Thickness(2, 2, 2, 2);
                    return;
                }
                curprzFile = dirInfo + @"\" + selectedFileName;
                System.IO.File.Copy(selectedFile, curprzFile, true);
                if (System.IO.File.Exists(dbFile) == true)
                {
                    progressBar.Visibility = Visibility.Visible;
                    btnCancel.IsEnabled = false;
                    btnOK.IsEnabled = false;
                    btnImport.IsEnabled = false;
                    backgroundWorker.RunWorkerAsync();
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
            
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        
        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {          
            dbFile = dirInfo + @"\plant.mdb";
        }
        ProII91.ProIIReader picker;
        ImportDB dbRelief;
        DataTable dtEqList;
        DataTable dtStream;
        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                dbRelief = new ImportDB(dbFile);
                DataTable dtEqType = dbRelief.GetDataByTableName("tbProIIEqType","");
                dtStream = dbRelief.GetTableStructureByTableName("tbProIIStreamData");
                dtEqList = dbRelief.GetTableStructureByTableName("tbProIIEqData");
                picker = new ProIIReader();
                picker.InitProIIPicker(curprzFile);
                int total = picker.getAllEqAndStreamTotal(dtEqType, ref eqList, ref streamList);
                int eqCount = eqList.Count;
                for (int i = 1; i <= eqList.Count; i++)
                {
                    EqInfo eq = (EqInfo)eqList[i - 1];
                    picker.getEqInfo(eq.eqType, eq.eqName, ref dtEqList);
                    int percents = (i * 100) / total;
                    backgroundWorker.ReportProgress(percents, i);
                }

                for (int i = 1; i <= streamList.Count; i++)
                {
                    picker.getSteamInfo(streamList[i - 1].ToString(), ref dtStream);
                    int percents = ((eqCount + i) * 100) / total;
                    backgroundWorker.ReportProgress(percents);
                }
                picker.ReleaseProIIPicker();
                backgroundWorker.ReportProgress(100);
                
            }
            catch (Exception ex)
            {
                string lines = ex.ToString() ;

                using (StreamWriter writer = new StreamWriter("log.txt",true))
                {
                    writer.WriteLine(ex.ToString());
                }
            }

        }
        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }
        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                DataSet ds = new DataSet();

                ds.Tables.Add(dtStream.Copy());
                ds.Tables.Add(dtEqList.Copy());
                dbRelief.ImportDataByDataSet(ds);
            }
            catch (Exception ex)
            {
            }
            this.DialogResult = true;
        }

        private void MetroWindow_Closing_1(object sender, CancelEventArgs e)
        {
            if (backgroundWorker.IsBusy)
            {
                e.Cancel = true;
                MessageBox.Show("Data is importing...","Message Box");

            }
        }
    }
}
