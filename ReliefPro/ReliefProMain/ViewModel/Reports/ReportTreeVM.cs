using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows;
using ReliefProMain.ViewModel.Trees;
using System.Windows.Input;
using ReliefProMain.Commands;
using ReliefProMain.View.Reports;
using System.IO;
using UOMLib;
using ReliefProBLL.Common;
using NHibernate;
using ReliefProDAL;
using ReliefProModel;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;

namespace ReliefProMain.ViewModel.Reports
{
    public class ReportTreeVM : ViewModelBase
    {
        public string CurrentPlantPath;
        public ReportTreeVM(string plantName, string dirPlant)
        {
            PlantCollection = new ObservableCollection<PlantVM>();
            CurrentPlantPath = dirPlant;
            PlantVM plantvm = new PlantVM(plantName, dirPlant);

            PlantCollection.Add(plantvm);
        }

        private ObservableCollection<PlantVM> _PlantCollection;
        public ObservableCollection<PlantVM> PlantCollection
        {
            get { return _PlantCollection; }
            set
            {
                if (_PlantCollection != value)
                {
                    _PlantCollection = value;
                    OnPropertyChanged("PlantCollection");
                }
            }
        }

        private ICommand _PlantSummaryCommand;
        public ICommand PlantSummaryCommand
        {
            get
            {
                if (_PlantSummaryCommand == null)
                {
                    _PlantSummaryCommand = new RelayCommand(PlantSummary);
                }
                return _PlantSummaryCommand;
            }
        }
        private void PlantSummary(object obj)
        {
            PlantSummaryVM vm = new PlantSummaryVM(PlantCollection);
            //string dbPlantFile = CurrentPlantPath + @"\plant.mdb";
            //List<Tuple<int, List<string>>> UnitPath = new List<Tuple<int, List<string>>>();
            //List<UnitVM> list = GetCheckedUnits();
            //if (list.Count > 0)
            //{
            //    foreach (UnitVM uvm in list)
            //    {
            //        List<string> ReportPath = new List<string>();
            //        ReportPath.Add(dbPlantFile);
            //        string dirUnit = CurrentPlantPath + @"\" + uvm.UnitName;
            //        foreach (PSVM p in uvm.PSCollection)
            //        {
            //            if (p.IsChecked)
            //            {
            //                ReportPath.Add(dirUnit + @"\" + p.PSName + @"\protectedsystem.mdb");
            //            }
            //        }
            //        Tuple<int, List<string>> t = new Tuple<int, List<string>>(uvm.ID, ReportPath);
            //        UnitPath.Add(t);

            //    }

            //    PlantSummaryView view = new PlantSummaryView();
            //    PlantSummaryVM vm = new PlantSummaryVM(UnitPath);
            //    view.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            //    view.WindowState = WindowState.Maximized;
            //    view.DataContext = vm;
            //    view.ShowDialog();
            //}
        }
        private ICommand _UnitSummaryCommand;
        public ICommand UnitSummaryCommand
        {
            get
            {
                if (_UnitSummaryCommand == null)
                {
                    _UnitSummaryCommand = new RelayCommand(UnitSummary);
                }
                return _UnitSummaryCommand;
            }
        }
        private void UnitSummary(object obj)
        {
            PUsummaryVM vm = new PUsummaryVM(PlantCollection);

            //string dbPlantFile = CurrentPlantPath + @"\plant.mdb";
            //UnitVM uvm = GetSingleCheckedUnit();
            //if (uvm != null)
            //{
            //    List<string> ReportPath = new List<string>();
            //    ReportPath.Add(dbPlantFile);
            //    string unitPath = CurrentPlantPath + @"\" + uvm.UnitName;
            //    foreach (PSVM p in uvm.PSCollection)
            //    {
            //        if (p.IsChecked)
            //        {
            //            ReportPath.Add(unitPath + @"\" + p.PSName + @"\protectedsystem.mdb");
            //        }
            //    }
               
                //PUsummaryView view = new PUsummaryView();
                //PUsummaryVM vm = new PUsummaryVM(uvm.ID, ReportPath);
                //view.WindowState = WindowState.Maximized;
                //view.DataContext = vm;
                //view.ShowDialog();
            //}
        }

        private ICommand _PSSummaryCommand;
        public ICommand PSSummaryCommand
        {
            get
            {
                if (_PSSummaryCommand == null)
                {
                    _PSSummaryCommand = new RelayCommand(PSSummary);
                }
                return _PSSummaryCommand;
            }
        }
        private UnitVM GetSingleCheckedUnit()
        {
            foreach (PlantVM plantvm in PlantCollection)
            {
                foreach (UnitVM uvm in plantvm.UnitCollection)
                {
                    foreach (PSVM ps in uvm.PSCollection)
                    {
                        if (ps.IsChecked)
                        {
                            return uvm;
                        }
                    }
                    //if (uvm.IsChecked)
                    //{
                    //    curUnitVM = uvm;
                    //    break;
                    //}
                }
            }
            return null;

        }
        private List<UnitVM> GetCheckedUnits()
        {
            List<UnitVM> chkedUnit = new List<UnitVM>();
            foreach (PlantVM plantvm in PlantCollection)
            {
                foreach (UnitVM uvm in plantvm.UnitCollection)
                {
                    if (!chkedUnit.Exists(p => p.ID == uvm.ID))
                    {
                        if (uvm.IsChecked)
                        {
                            chkedUnit.Add(uvm);
                            //break;
                        }
                        else
                        {
                            foreach (PSVM ps in uvm.PSCollection)
                            {
                                if (ps.IsChecked)
                                {
                                    chkedUnit.Add(uvm);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return chkedUnit;

        }


        private List<PSVM> GetCheckedPS()
        {
            List<PSVM> list = new List<PSVM>();
            foreach (PlantVM plantvm in PlantCollection)
            {
                foreach (UnitVM uvm in plantvm.UnitCollection)
                {
                    foreach (PSVM ps in uvm.PSCollection)
                    {
                        if (ps.IsChecked)
                        {
                            list.Add(ps);
                        }
                    }
                  
                }
            }
            return list;

        }
        private void PSSummary(object obj)
        {
            SaveFileDialog sfDlg = new SaveFileDialog();
            sfDlg.Filter = "Excel Files | *.xls";
            sfDlg.DefaultExt = "xls";
            if (sfDlg.ShowDialog() == true)
            {
                string dbPlantFile = CurrentPlantPath + @"\plant.mdb";
                NHibernateHelper helperPlant = new NHibernateHelper(dbPlantFile);
                ISession SessionPlant = helperPlant.GetCurrentSession();
                List<PSVM> lstPSVM = GetCheckedPS();
                if (lstPSVM.Count == 0)
                {
                    MessageBox.Show("Please Select Protected System.","Message Box",MessageBoxButton.OK,MessageBoxImage.Warning);
                    return;
                }
                string filePath = sfDlg.FileName;
                string vsd = AppDomain.CurrentDomain.BaseDirectory.ToString() + @"template\SimTech-PRV_DataSheet_Model.xls";
                System.IO.File.Copy(vsd, filePath, true);

                Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
                Microsoft.Office.Interop.Excel.Workbook xlWorkBook = xlApp.Workbooks.Open(filePath, Type.Missing, false, Type.Missing,
                    Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                TreeUnitDAL unitdal = new TreeUnitDAL();                
                TreePSDAL psdal = new TreePSDAL();
                foreach (PSVM psvm in lstPSVM)
                {
                    TreePS tps = psdal.GetModel(psvm.ID, SessionPlant);
                    TreeUnit tunit = unitdal.GetModel(tps.UnitID, SessionPlant);
                    string dbProtectedSystemFile = CurrentPlantPath + @"\" + tunit.UnitName + @"\" + tps.PSName + @"\protectedsystem.mdb";

                    NHibernateHelper helperProtectedSystem = new NHibernateHelper(dbProtectedSystemFile);
                    ISession SessionProtectedSystem = helperProtectedSystem.GetCurrentSession();

                    ScenarioDAL scdal = new ScenarioDAL();
                    IList<Scenario> list = scdal.GetAllList(SessionProtectedSystem);

                    PSVDAL psvdal = new PSVDAL();
                    PSV psv = psvdal.GetModel(SessionProtectedSystem);

                    Microsoft.Office.Interop.Excel.Worksheet xlWorkSheetSource = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Sheets.get_Item(1);
                    xlWorkSheetSource.Copy(Type.Missing, xlWorkSheetSource);
                    Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Sheets.get_Item(2);
                    xlWorkSheet.Name = tps.PSName;

                    DirectoryInfo di = new DirectoryInfo(CurrentPlantPath);

                    xlWorkSheet.Cells[4,6] = di.Name;
                    xlWorkSheet.Cells[4,7] = tunit.UnitName;
                    xlWorkSheet.Cells[4,9] = tps.PSName;

                    xlWorkSheet.Cells[13,6] = psv.ValveNumber;
                    xlWorkSheet.Cells[10,11] = psv.Pressure;

                    int count = list.Count;
                    int row1 = 14;
                    int note1 = 42;
                    int row2 = 74;
                    int note2 = 102;
                    int row3 = 134;
                    int note3 = 162;
                    if (count >= 5 && count <= 9)
                    {
                        xlWorkSheet.Cells[4,66] = di.Name;
                        xlWorkSheet.Cells[4,67] = tunit.UnitName;
                        xlWorkSheet.Cells[4,69] = tps.PSName;

                        xlWorkSheet.Cells[13,66] = psv.ValveNumber;
                        xlWorkSheet.Cells[10,71] = psv.Pressure;

                    }
                    else if (count > 9)
                    {
                        xlWorkSheet.Cells[4,126] = di.Name;
                        xlWorkSheet.Cells[4,127] = tunit.UnitName;
                        xlWorkSheet.Cells[4,129] = tps.PSName;

                        xlWorkSheet.Cells[13,126] = psv.ValveNumber;
                        xlWorkSheet.Cells[10,131] = psv.Pressure;

                    }
                    string Compressibility = string.Empty;
                    int ct = 0;
                    for (int i = 1; i <= list.Count; i++)
                    {
                        Scenario sc = list[i - 1];
                        int col = ct % 5;
                        if (ct <= 4)
                        {
                            xlWorkSheet.Cells[5 + col * 2,row1] = sc.ScenarioName;
                            xlWorkSheet.Cells[5 + col * 2,row1 + 1] = 16;
                            if (sc.ScenarioName.ToString().Contains("Fire"))
                            {
                                xlWorkSheet.Cells[5 + col * 2,row1 + 1] = 21;
                            }
                            xlWorkSheet.Cells[5 + col * 2,row1 + 2] = sc.ReliefPressure;
                            xlWorkSheet.Cells[5 + col * 2,row1 + 3] = sc.ReliefTemperature;
                            xlWorkSheet.Cells[5 + col * 2,row1 + 4] = sc.ReliefLoad;
                            xlWorkSheet.Cells[5 + col * 2,row1 + 5] = sc.ReliefMW;
                            xlWorkSheet.Cells[5 + col * 2,row1 + 6] = sc.ReliefZ;
                            xlWorkSheet.Cells[5 + col * 2,row1 + 7] = sc.ReliefCpCv;
                            xlWorkSheet.Cells[3,note1 + ct] = "";
                            xlWorkSheet.Cells[3,note2 + ct] = "";
                            xlWorkSheet.Cells[3,note3 + ct] = "";
                        }
                        else if (ct >= 5 && ct <= 9)
                        {
                            xlWorkSheet.Cells[5 + col * 2,row2] = sc.ScenarioName.ToString();
                            xlWorkSheet.Cells[5 + col * 2,row2 + 1] = 16;
                            if (sc.ScenarioName.ToString().Contains("Fire"))
                            {
                                xlWorkSheet.Cells[5 + col * 2,row2 + 1] = 21;
                            }
                            xlWorkSheet.Cells[5 + col * 2,row2 + 2] = sc.ReliefPressure;
                            xlWorkSheet.Cells[5 + col * 2,row2 + 3] = sc.ReliefTemperature;
                            xlWorkSheet.Cells[5 + col * 2,row2 + 4] = sc.ReliefLoad;
                            xlWorkSheet.Cells[5 + col * 2,row2 + 5] = sc.ReliefMW;
                            xlWorkSheet.Cells[5 + col * 2,row2 + 6] = sc.ReliefZ;
                            xlWorkSheet.Cells[5 + col * 2,row2 + 7] = sc.ReliefCpCv;
                            xlWorkSheet.Cells[3,note1 + ct] = "";
                            xlWorkSheet.Cells[3,note2 + ct] = "";
                            xlWorkSheet.Cells[3,note3 + ct] = "";
                        }
                        else
                        {
                            xlWorkSheet.Cells[5 + col * 2,row3] = sc.ScenarioName.ToString();
                            xlWorkSheet.Cells[5 + col * 2,row3 + 1] = 16;
                            if (sc.ScenarioName.ToString().Contains("Fire"))
                            {
                                xlWorkSheet.Cells[5 + col * 2,row3 + 1] = 21;
                            }
                            xlWorkSheet.Cells[5 + col * 2,row3 + 2] = sc.ReliefPressure;
                            xlWorkSheet.Cells[5 + col * 2,row3 + 3] = sc.ReliefTemperature;
                            xlWorkSheet.Cells[5 + col * 2,row3 + 4] = sc.ReliefLoad;
                            xlWorkSheet.Cells[5 + col * 2,row3 + 5] = sc.ReliefMW;
                            xlWorkSheet.Cells[5 + col * 2,row3 + 6] = sc.ReliefZ;
                            xlWorkSheet.Cells[5 + col * 2,row3 + 7] = sc.ReliefCpCv;
                            xlWorkSheet.Cells[3,note1 + ct] = "";
                            xlWorkSheet.Cells[3,note2 + ct] = "";
                            xlWorkSheet.Cells[3,note3 + ct] = "";
                        }

                        ct++;

                    }
                    if (count <= 5)
                    {
                        Microsoft.Office.Interop.Excel.Range r = xlWorkSheet.Range[xlWorkSheet.Cells[2,122], xlWorkSheet.Cells[3,181]];
                        r.UnMerge();
                        r = xlWorkSheet.Range[xlWorkSheet.Cells[2,122], xlWorkSheet.Cells[14,181]];
                        r.Clear();

                        Microsoft.Office.Interop.Excel.Shape pic = xlWorkSheet.Shapes.Item(3) as Microsoft.Office.Interop.Excel.Shape;
                        pic.Delete();

                        r = xlWorkSheet.Range[xlWorkSheet.Cells[2,62], xlWorkSheet.Cells[3,121]];
                        r.UnMerge();
                        r = xlWorkSheet.Range[xlWorkSheet.Cells[2,62], xlWorkSheet.Cells[14,121]];
                        r.Clear();

                        pic = xlWorkSheet.Shapes.Item(2) as Microsoft.Office.Interop.Excel.Shape;
                        pic.Delete();


                    }
                    else if (count <= 10)
                    {
                        Microsoft.Office.Interop.Excel.Range r = xlWorkSheet.Range[xlWorkSheet.Cells[2,122], xlWorkSheet.Cells[3,181]];
                        r.UnMerge();
                        r = xlWorkSheet.Range[xlWorkSheet.Cells[2,122], xlWorkSheet.Cells[14,181]];
                        r.Clear();

                        Microsoft.Office.Interop.Excel.Shape pic = xlWorkSheet.Shapes.Item(3) as Microsoft.Office.Interop.Excel.Shape;
                        pic.Delete();
                    }
                    Microsoft.Office.Interop.Excel.Range rf = xlWorkSheet.Range[xlWorkSheet.Cells[1,1], xlWorkSheet.Cells[1,1]];
                    rf.Select();
                    
                    releaseObject(xlWorkSheet);
                }
                Microsoft.Office.Interop.Excel.Worksheet xlWorkSheetSource2 = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Sheets.get_Item(1);
                xlWorkSheetSource2.Visible = Microsoft.Office.Interop.Excel.XlSheetVisibility.xlSheetHidden;;
                xlWorkBook.Save();
                xlWorkBook.Close(true, Type.Missing, Type.Missing);


                xlApp.Quit();


               
                releaseObject(xlWorkBook);
                releaseObject(xlApp);

               
                System.Diagnostics.Process.Start(filePath);
            }
        }
        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                //obj = null;
            }
            catch (Exception ex)
            {
                //obj = null;
                //MessageBox.Show("Unable to release the Object " + ex.ToString());
            }
            finally
            {
                GC.ReRegisterForFinalize(obj);
                obj = null;
            }
        }


        //调用底层函数获取进程标示 
        [DllImport("User32.dll")]
        public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int ProcessId);
        private static void KillExcel(Microsoft.Office.Interop.Excel.Application theApp)
        {
            int id = 0;
            IntPtr intptr = new IntPtr(theApp.Hwnd);
            System.Diagnostics.Process p = null;
            try
            {
                GetWindowThreadProcessId(intptr, out id);
                p = System.Diagnostics.Process.GetProcessById(id);
                if (p != null)
                {
                    p.Kill();
                    p.Dispose();
                }
            }
            catch (Exception ex)
            {

            }
        }

    }
}
