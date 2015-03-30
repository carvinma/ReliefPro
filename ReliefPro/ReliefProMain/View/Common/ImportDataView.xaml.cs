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
using System.Threading;
using System.Windows.Threading;
using System.Timers;
using System.Threading.Tasks;
using System.Collections.Concurrent;

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
        IList<ProIIStreamData> streamListData1 = new List<ProIIStreamData>();
        IList<ProIIStreamData> streamListData2 = new List<ProIIStreamData>();
        IList<ProIIStreamData> streamListData3 = new List<ProIIStreamData>();
        IList<ProIIStreamData> streamListData4 = new List<ProIIStreamData>();
        string version;
        //BackgroundWorker backgroundWorker = new BackgroundWorker();

        public ImportDataView()
        {
            InitializeComponent();
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
            if (r1.IsChecked == true)
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
                        MessageBox.Show("this file was imported !", "Message Box");
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
                if (Directory.Exists(dir))
                {
                    Directory.Delete(dir, true);
                }
                Directory.CreateDirectory(dir);

                curprzFile = dir + @"\" + selectedFileName;
                System.IO.File.Copy(selectedFile, curprzFile, true);
                if (System.IO.File.Exists(dbPlantFile) == true)
                {
                    version = ProIIFactory.GetProIIhs2Verison(curprzFile, dir);
                    if (version != "9.1" && version != "9.2" && version != "9.0" && version != "9.3" && version != "8.3")
                    {
                        MessageBox.Show("This version is not supported!", "Message Box");
                        return;
                    }
                    SplashScreenManager.Show();
                    btnCancel.IsEnabled = false;
                    btnOK.IsEnabled = false;
                    btnImport.IsEnabled = false;
                    ProIIReader reader = new ProIIReader(version, curprzFile);
                    try
                    {
                        int[] arrEqStream = reader.GetAllCount();                      
                        total = arrEqStream[0]+arrEqStream[1];
                        //SplashScreenManager.Show(total);

                        SplashScreenManager.SentMsgToScreen("Importing data...... 20%");
                        List<ProIIEqData> eqListData= reader.GetAllEqInfo();
                        SplashScreenManager.SentMsgToScreen("Importing data...... 40%");
                        List<ProIIStreamData> streamListData = reader.GetAllStreamInfo(0,arrEqStream[1]);
                        SplashScreenManager.SentMsgToScreen("Importing data...... 60%");
                        //for (int i = 1; i <= eqList.Count; i++)
                        //{
                        //    ProIIEqData eq = eqList[i - 1];
                        //    reader.GetEqInfo(eq.EqType, eq.EqName, ref eqListData);
                        //    int percents = (i * 100) / total;
                        //    SplashScreenManager.SentMsgToScreen("Importing " + percents.ToString() + "%");
                        //}
                        
                        //Task task1 = new Task(delegate() { Run(reader1, streamList1, streamListData1, 1, test); });
                        //task1.Start();
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
                        SplashScreenManager.SentMsgToScreen("Importing data...... 80%");
                        ProIIStreamDataDAL dbStream = new ProIIStreamDataDAL();

                        foreach (ProIIStreamData data in streamListData)
                        {
                            dbStream.Add(data, SessionPlant);
                        }
                        isImportSucess = true;
                        SplashScreenManager.SentMsgToScreen("Importing data finished.");
                        SplashScreenManager.Close();
                        MessageBox.Show("Importing data sucessfully.", "Message Box");
                        this.Close();


                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Importing data failure.", "Message Box");
                        string lines = ex.ToString();

                        //using (StreamWriter writer = new StreamWriter("log.txt",true))
                        //{
                        //    writer.WriteLine(ex.ToString());
                        //    backgroundWorker.ReportProgress(100);
                        //    isImportSucess = false;
                        //}
                    }
                    finally
                    {

                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                SplashScreenManager.Close();
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

        
        int StreamCount = 0;
        int streamCountIndex = 0;
        int eqCount = 0;
        int total = 0;
        ArrayList list = new ArrayList();
        IList<ProIIEqData> eqList = new List<ProIIEqData>();
        IList<string> streamList = new List<string>();        
        

        /// <summary>     /// 共同做的工作     /// </summary>     
        private void Run(IProIIReader r, IList<string> strList, IList<ProIIStreamData> list, int start, int end)
        {

            //Monitor.Enter(this);//锁定，保持同步             
            for (int i = 1; i <= end - start + 1; i++)
            {
                string name = strList[i - 1].ToString();
                r.GetSteamInfo(name, ref list);
                if (SplashScreenManager.SplashValue < total)
                {
                    SplashScreenManager.SentMsgToScreen("Importing " + (100 * (SplashScreenManager.SplashValue + 1) / total).ToString("0") + "%");
                }
                //streamCountIndex++;                    
            }
            //if (streamCountIndex>=StreamCount )
            //{
            //    OnNumberClear(this, new EventArgs());//引发完成事件             
            //}
            //Monitor.Exit(this);//取消锁定 

        }

        
        private void MetroWindow_Closing_1(object sender, CancelEventArgs e)
        {

        }


        private void ReadData(string version,string przFile)
        {
            object objRtn=new object();
            ExcelMacroHelper.RunExcelMacro(@"Template\macro.xls",
                                                         "GetAllCount",
                                                        new Object[] { version, przFile },
                                                         out objRtn,
                                                         false);

            int[] counts = new int[2];
            counts = (int[])objRtn;

            SplashScreenManager.Show(counts[0]+counts[1]);

            ExcelMacroHelper.RunExcelMacro(@"Template\macro.xls",
                                                         "GetAllEqInfo",
                                                        new Object[] { version, przFile },
                                                         out objRtn,
                                                         false);


            ExcelMacroHelper.RunExcelMacro(@"Template\macro.xls",
                                                         "GetAllCount",
                                                        new Object[] { version, przFile },
                                                         out objRtn,
                                                         false);

            ExcelMacroHelper.RunExcelMacro(@"Template\macro.xls",
                                                         "GetStreamInfo2",
                                                        new Object[] { version, przFile,0,counts[1] },
                                                         out objRtn,
                                                         false);




            SplashScreenManager.Close();
        }
    }
}
