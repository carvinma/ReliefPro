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
using System.Collections.ObjectModel;
using System.Data;
using System.Collections;
using System.ComponentModel;
using ProII;
using NHibernate;
using ReliefProDAL;
using ReliefProBLL.Common;
using ReliefProModel;

namespace ReliefProMain.View
{
    /// <summary>
    /// ImportData.xaml 的交互逻辑
    /// </summary>
    public partial class ImportDataView : Window
    {
        public ISession SessionPlant;
        SourceFileDAL dal;
        IList<ProIIEqType> eqTypeList = null;
        IList<ProIIEqData> eqListData = new List<ProIIEqData>();
        IList<ProIIStreamData> streamListData = new List<ProIIStreamData>();
        string version;
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
        string dbPlantFile = string.Empty;
        string selectedFile = string.Empty;
        string selectedFileName = string.Empty;       
        string curprzFile = string.Empty;
        string FileNameNoExt = string.Empty;
        bool isCanImport = false;
        bool isImportSucess = false;
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
                    SourceFile sf = dal.GetModel(selectedFileName, SessionPlant);
                    if (sf == null)
                    {
                        isCanImport = true;
                    }
                    else
                    {
                        MessageBox.Show("this file was imported !","Message Box");
                    }
                }
            }
            
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (!isCanImport)
            {
                MessageBox.Show("Please select right file !", "Message Box");
                return;
            }
            try
            {
                if (this.txtSourceFile.Text == string.Empty)
                {
                    btnImport.BorderBrush = Brushes.Red;
                    btnImport.BorderThickness = new Thickness(2, 2, 2, 2);
                    return;
                }
                FileNameNoExt = System.IO.Path.GetFileNameWithoutExtension(selectedFile);
                string dir = dirInfo + @"\" + FileNameNoExt;
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                curprzFile = dir+ @"\" + selectedFileName;
                System.IO.File.Copy(selectedFile, curprzFile, true);
                if (System.IO.File.Exists(dbPlantFile) == true)
                {
                    version = ProIIFactory.GetProIIVerison(curprzFile, dir);
                    //IProIIRunCalcSave cs = ProIIFactory.CreateRunCalcSave(version);
                    //bool b=cs.CalcSave(curprzFile);

                    using (var helper = new NHibernateHelper(dbPlantFile))
                    {
                        ISession Session = helper.GetCurrentSession();
                        ProIIEqTypeDAL db = new ProIIEqTypeDAL();
                        eqTypeList = db.GetAllList(Session);
                    }
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
            dbPlantFile = dirInfo + @"\plant.mdb";
            dal = new SourceFileDAL();
        }
        
        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            ArrayList list = new ArrayList();
            IList<ProIIEqData> eqList=new List<ProIIEqData>();
            IList<string> streamList = new List<string>();         
            try
            {
                
                IProIIReader reader = ProIIFactory.CreateReader(version);
                reader.InitProIIReader(curprzFile);

                int total = reader.GetAllEqAndStreamTotal(eqTypeList, ref eqList, ref streamList);
                int eqCount = eqList.Count;
                for (int i = 1; i <= eqList.Count; i++)
                {
                    ProIIEqData eq = eqList[i - 1];
                    reader.GetEqInfo(eq.EqType, eq.EqName, ref eqListData);                    
                    int percents = (i * 100) / total;
                    backgroundWorker.ReportProgress(percents, i);
                }

                for (int i = 1; i <= streamList.Count; i++)
                {
                    string name = streamList[i - 1].ToString();

                    reader.GetSteamInfo(name, ref streamListData);
                    int percents = ((eqCount + i) * 100) / total;
                    backgroundWorker.ReportProgress(percents);
                }
                reader.ReleaseProIIReader();

               
                SourceFile df = new SourceFile();
                SourceFileDAL dal = new SourceFileDAL();
                df.FileName = selectedFileName.ToLower();
                df.FileType = 0;
                df.FileVersion = version;
                df.FileNameNoExt = FileNameNoExt;
                dal.Add(df, SessionPlant);

                ProIIEqDataDAL dbEq = new ProIIEqDataDAL();
                foreach (ProIIEqData data in eqListData)
                {
                    dbEq.Add(data, SessionPlant);
                }

                ProIIStreamDataDAL dbStream = new ProIIStreamDataDAL();
                foreach (ProIIStreamData data in streamListData)
                {
                    dbStream.Add(data, SessionPlant);
                }
                isImportSucess = true;
                backgroundWorker.ReportProgress(100);
                
            }
            catch (Exception ex)
            {
                string lines = ex.ToString() ;

                using (StreamWriter writer = new StreamWriter("log.txt",true))
                {
                    writer.WriteLine(ex.ToString());
                    backgroundWorker.ReportProgress(100);
                    isImportSucess = false;
                }
            }

        }
        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }
        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (isImportSucess)
            {
                MessageBox.Show("Data is imported sucessfully!", "Message Box");
                this.DialogResult = true;
            }
            else
            {
                MessageBox.Show("Data is imported failed!", "Message Box");
                this.DialogResult = false;
            }
            
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
