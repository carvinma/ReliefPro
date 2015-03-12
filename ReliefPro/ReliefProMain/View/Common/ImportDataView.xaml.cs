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

                string dir1 = dirInfo + @"\a" + FileNameNoExt;
                if (Directory.Exists(dir1))
                {
                    Directory.Delete(dir1, true);
                }
                Directory.CreateDirectory(dir1);

                string dir2 = dirInfo + @"\b" + FileNameNoExt;
                if (Directory.Exists(dir2))
                {
                    Directory.Delete(dir2, true);
                }
                Directory.CreateDirectory(dir2);

                string dir3 = dirInfo + @"\c" + FileNameNoExt;
                if (Directory.Exists(dir3))
                {
                    Directory.Delete(dir3, true);
                }
                Directory.CreateDirectory(dir3);

                string dir4 = dirInfo + @"\d" + FileNameNoExt;
                if (Directory.Exists(dir4))
                {
                    Directory.Delete(dir4, true);
                }
                Directory.CreateDirectory(dir4);


                curprzFile = dir + @"\" + selectedFileName;
                System.IO.File.Copy(selectedFile, curprzFile, true);
                if (System.IO.File.Exists(dbPlantFile) == true)
                {
                    //version = ProIIFactory.GetProIIVerison(curprzFile, dir);
                    version = ProIIFactory.GetProIIhs2Verison(curprzFile, dir);
                    if (version != "9.1" && version != "9.2" && version != "9.0" && version != "9.3" && version != "8.3")
                    //if (version != "9.1" && version != "9.2" )
                    {
                        MessageBox.Show("This version is not supported!", "Message Box");
                        return;
                    }
                    using (var helper = new NHibernateHelper(dbPlantFile))
                    {
                        ISession Session = helper.GetCurrentSession();
                        ProIIEqTypeDAL db = new ProIIEqTypeDAL();
                        eqTypeList = db.GetAllList(Session);
                    }

                    btnCancel.IsEnabled = false;
                    btnOK.IsEnabled = false;
                    btnImport.IsEnabled = false;

                    try
                    {

                        reader = ProIIFactory.CreateReader(version);
                        reader.InitProIIReader(curprzFile);
                        total = reader.GetAllEqAndStreamTotal(eqTypeList, ref eqList, ref streamList);
                        SplashScreenManager.Show(total);
                        eqCount = eqList.Count;
                        StreamCount = streamList.Count;

                        for (int i = 1; i <= eqList.Count; i++)
                        {
                            ProIIEqData eq = eqList[i - 1];
                            reader.GetEqInfo(eq.EqType, eq.EqName, ref eqListData);
                            int percents = (i * 100) / total;
                            SplashScreenManager.SentMsgToScreen("Importing " + percents.ToString() + "%");
                        }
                        reader.ReleaseProIIReader();

                        int test = StreamCount / 4;

                        //OnNumberClear += new EventHandler(Thread_OnNumberClear);

                        //threadOne = new Thread(delegate() { Run(1, test); });//两个线程共同做一件事情         
                        //threadTwo = new Thread(delegate() { Run(test + 1, StreamCount); }); ;//两个线程共同做一件事情 
                        //threadOne.Start();
                        //threadTwo.Start();
                        string curprzFile1 = dir1 + @"\" + selectedFileName;
                        string curprzFile2 = dir2 + @"\" + selectedFileName;
                        string curprzFile3 = dir3 + @"\" + selectedFileName;
                        string curprzFile4 = dir4 + @"\" + selectedFileName;
                        System.IO.File.Copy(selectedFile, curprzFile1, true);
                        System.IO.File.Copy(selectedFile, curprzFile2, true);
                        System.IO.File.Copy(selectedFile, curprzFile3, true);
                        System.IO.File.Copy(selectedFile, curprzFile4, true);

                        IProIIReader reader1 = ProIIFactory.CreateReader(version);


                        reader1.InitProIIReader(curprzFile1);
                        IProIIReader reader2 = ProIIFactory.CreateReader(version);
                        reader2.InitProIIReader(curprzFile2);
                        IProIIReader reader3 = ProIIFactory.CreateReader(version);
                        reader3.InitProIIReader(curprzFile3);
                        IProIIReader reader4 = ProIIFactory.CreateReader(version);
                        reader4.InitProIIReader(curprzFile4);

                        for (int i = 1; i <= test; i++)
                        {
                            streamList1.Add(streamList[i - 1]);
                        }
                        for (int i = test + 1; i <= 2 * test; i++)
                        {
                            streamList2.Add(streamList[i - 1]);
                        }
                        for (int i = 2 * test + 1; i <= 3 * test; i++)
                        {
                            streamList3.Add(streamList[i - 1]);
                        }
                        for (int i = 3 * test + 1; i <= StreamCount; i++)
                        {
                            streamList4.Add(streamList[i - 1]);
                        }

                        Task task1 = new Task(delegate() { Run(reader1, streamList1, streamListData1, 1, test); });
                        Task task2 = new Task(delegate() { Run(reader2, streamList2, streamListData2, test + 1, 2 * test); });
                        Task task3 = new Task(delegate() { Run(reader3, streamList3, streamListData3, 2 * test + 1, 3 * test); });
                        Task task4 = new Task(delegate() { Run(reader4, streamList4, streamListData4, 3 * test + 1, StreamCount); });
                        task1.Start();
                        task2.Start();
                        task3.Start();
                        task4.Start();
                        Task.WaitAll(task1, task2, task3, task4);
                        //Task.WaitAll(task1,task2);
                        reader1.ReleaseProIIReader();
                        reader2.ReleaseProIIReader();
                        reader3.ReleaseProIIReader();
                        reader4.ReleaseProIIReader();

                        File.Delete(curprzFile1);
                        File.Delete(curprzFile2);
                        File.Delete(curprzFile3);
                        File.Delete(curprzFile4);


                        Directory.Delete(dir1, true);
                        Directory.Delete(dir2, true);
                        Directory.Delete(dir3, true);
                        Directory.Delete(dir4, true);
                        //threadTwo.Abort();
                        //threadOne.Abort();
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

                        streamListData = streamListData.Union(streamListData1).ToList();
                        streamListData = streamListData.Union(streamListData2).ToList();
                        streamListData = streamListData.Union(streamListData3).ToList();
                        streamListData = streamListData.Union(streamListData4).ToList();
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

        Thread threadOne;
        Thread threadTwo;
        int StreamCount = 0;
        int streamCountIndex = 0;
        int eqCount = 0;
        int total = 0;
        ArrayList list = new ArrayList();
        IList<ProIIEqData> eqList = new List<ProIIEqData>();
        IList<string> streamList = new List<string>();
        IList<string> streamList1 = new List<string>();
        IList<string> streamList2 = new List<string>();
        IList<string> streamList3 = new List<string>();
        IList<string> streamList4 = new List<string>();
        IProIIReader reader;
        private event EventHandler OnNumberClear;

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

        //执行完成之后，停止所有线程 
        void Thread_OnNumberClear(object sender, EventArgs e)
        {
            try
            {


            }
            catch (Exception ex)
            {
            }
            finally
            {


            }
        }
        private void MetroWindow_Closing_1(object sender, CancelEventArgs e)
        {

        }


        private void Read90Data(string version,string przFile)
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
