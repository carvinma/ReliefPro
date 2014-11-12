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


        private PSVM GetSingleCheckedPS()
        {
            foreach (PlantVM plantvm in PlantCollection)
            {
                foreach (UnitVM uvm in plantvm.UnitCollection)
                {
                    foreach (PSVM ps in uvm.PSCollection)
                    {
                        if (ps.IsChecked)
                        {
                            return ps;
                        }
                    }
                  
                }
            }
            return null;

        }
        private void PSSummary(object obj)
        {
            PSVM psvm = GetSingleCheckedPS();
            string dbPlantFile = CurrentPlantPath + @"\plant.mdb";
            NHibernateHelper helperPlant = new NHibernateHelper(dbPlantFile);
            ISession SessionPlant = helperPlant.GetCurrentSession();
            TreePSDAL psdal=new TreePSDAL();
            TreePS tps=psdal.GetModel(psvm.ID,SessionPlant);
            TreeUnitDAL unitdal=new TreeUnitDAL();
            TreeUnit tunit=unitdal.GetModel(tps.UnitID,SessionPlant);

            string dbProtectedSystemFile=CurrentPlantPath+@"\"+tunit.UnitName+@"\"+tps.PSName+@"\protectedsystem.mdb";

            NHibernateHelper helperProtectedSystem = new NHibernateHelper(dbProtectedSystemFile);
            ISession SessionProtectedSystem = helperProtectedSystem.GetCurrentSession();

            ScenarioDAL scdal = new ScenarioDAL();
            IList<Scenario> list = scdal.GetAllList(SessionProtectedSystem);



            string dirPS = System.IO.Path.GetDirectoryName(dbProtectedSystemFile);
            string filePath = dirPS + @"\ps.xlsx";
            string vsd = AppDomain.CurrentDomain.BaseDirectory.ToString() + @"template\SimTech-PRV_DataSheet_Model.xlsx";
            System.IO.File.Copy(vsd, filePath,true);

            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook xlWorkBook = xlApp.Workbooks.Open(filePath, Type.Missing, false, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Sheets.get_Item(1);
            //xlWorkSheet.Cells[5][15] = "test";
            int count = 0;
            int row1 = 14;
            int note1 = 42;
            int row2 = 74;
            int note2 = 102;
            int row3 = 134;
            int note3 = 162;
            string Compressibility = string.Empty;
            for (int i = 1; i <=list.Count; i++)
            {
                Scenario sc = list[i - 1];
                    int col = count % 5;
                    if (count <= 4)
                    {
                        xlWorkSheet.Cells[5 + col * 2][row1] = sc.ScenarioName;
                        xlWorkSheet.Cells[5 + col * 2][row1 + 1] = 16;
                        if (sc.ScenarioName.ToString().Contains("Fire"))
                        {
                            xlWorkSheet.Cells[5 + col * 2][row1 + 1] = 21;
                        }
                        xlWorkSheet.Cells[5 + col * 2][row1 + 2] = sc.ReliefPressure;
                        xlWorkSheet.Cells[5 + col * 2][row1 + 3] = sc.ReliefTemperature;
                        xlWorkSheet.Cells[5 + col * 2][row1 + 4] = sc.ReliefLoad;
                        xlWorkSheet.Cells[5 + col * 2][row1 + 5] = sc.ReliefMW;
                        xlWorkSheet.Cells[5 + col * 2][row1 + 6] = Compressibility;
                        xlWorkSheet.Cells[5 + col * 2][row1 + 7] = sc.ReliefCpCv;
                        xlWorkSheet.Cells[3][note1 + count] = "";
                        xlWorkSheet.Cells[3][note2 + count] = "";
                        xlWorkSheet.Cells[3][note3 + count] = "";
                    }
                    else if (count >= 5 && count <= 9)
                    {
                        xlWorkSheet.Cells[5 + col * 2][row2] = sc.ScenarioName.ToString();
                        xlWorkSheet.Cells[5 + col * 2][row2 + 1] = 16;
                        if (sc.ScenarioName.ToString().Contains("Fire"))
                        {
                            xlWorkSheet.Cells[5 + col * 2][row2 + 1] = 21;
                        }
                        xlWorkSheet.Cells[5 + col * 2][row2 + 2] = sc.ReliefPressure;
                        xlWorkSheet.Cells[5 + col * 2][row2 + 3] = sc.ReliefTemperature;
                        xlWorkSheet.Cells[5 + col * 2][row2 + 4] = sc.ReliefLoad;
                        xlWorkSheet.Cells[5 + col * 2][row2 + 5] = sc.ReliefMW;
                        xlWorkSheet.Cells[5 + col * 2][row2 + 6] = Compressibility;
                        xlWorkSheet.Cells[5 + col * 2][row2 + 7] = sc.ReliefCpCv;
                        xlWorkSheet.Cells[3][note1 + count] = "";
                        xlWorkSheet.Cells[3][note2 + count] = "";
                        xlWorkSheet.Cells[3][note3 + count] = "";
                    }
                    else
                    {
                        xlWorkSheet.Cells[5 + col * 2][row3] = sc.ScenarioName.ToString();
                        xlWorkSheet.Cells[5 + col * 2][row3 + 1] = 16;
                        if (sc.ScenarioName.ToString().Contains("Fire"))
                        {
                            xlWorkSheet.Cells[5 + col * 2][row3 + 1] = 21;
                        }
                        xlWorkSheet.Cells[5 + col * 2][row3 + 2] = sc.ReliefPressure;
                        xlWorkSheet.Cells[5 + col * 2][row3 + 3] = sc.ReliefTemperature;
                        xlWorkSheet.Cells[5 + col * 2][row3 + 4] = sc.ReliefLoad;
                        xlWorkSheet.Cells[5 + col * 2][row3 + 5] = sc.ReliefMW;
                        xlWorkSheet.Cells[5 + col * 2][row3 + 6] = Compressibility;
                        xlWorkSheet.Cells[5 + col * 2][row3 + 7] = sc.ReliefCpCv;
                        xlWorkSheet.Cells[3][note1 + count] = "";
                        xlWorkSheet.Cells[3][note2 + count] = "";
                        xlWorkSheet.Cells[3][note3 + count] = "";
                    }

                    count++;
                
            }
            if (count <= 5)
            {
                Microsoft.Office.Interop.Excel.Range r = xlWorkSheet.Range[xlWorkSheet.Cells[2][122], xlWorkSheet.Cells[3][181]];
                r.UnMerge();
                r = xlWorkSheet.Range[xlWorkSheet.Cells[2][122], xlWorkSheet.Cells[14][181]];
                r.Clear();

                Microsoft.Office.Interop.Excel.Shape pic = xlWorkSheet.Shapes.Item(3) as Microsoft.Office.Interop.Excel.Shape;
                pic.Delete();

                r = xlWorkSheet.Range[xlWorkSheet.Cells[2][62], xlWorkSheet.Cells[3][121]];
                r.UnMerge();
                r = xlWorkSheet.Range[xlWorkSheet.Cells[2][62], xlWorkSheet.Cells[14][121]];
                r.Clear();

                pic = xlWorkSheet.Shapes.Item(2) as Microsoft.Office.Interop.Excel.Shape;
                pic.Delete();


            }
            else if (count <= 10)
            {
                Microsoft.Office.Interop.Excel.Range r = xlWorkSheet.Range[xlWorkSheet.Cells[2][122], xlWorkSheet.Cells[3][181]];
                r.UnMerge();
                r = xlWorkSheet.Range[xlWorkSheet.Cells[2][122], xlWorkSheet.Cells[14][181]];
                r.Clear();

                Microsoft.Office.Interop.Excel.Shape pic = xlWorkSheet.Shapes.Item(3) as Microsoft.Office.Interop.Excel.Shape;
                pic.Delete();
            }
            Microsoft.Office.Interop.Excel.Range rf= xlWorkSheet.Range[xlWorkSheet.Cells[1][1],xlWorkSheet.Cells[1][1]];
            rf.Select();
            xlWorkBook.Save();
            xlWorkBook.Close(true, Type.Missing, Type.Missing);


            xlApp.Quit();


            releaseObject(xlWorkSheet);
            releaseObject(xlWorkBook);
            releaseObject(xlApp);

            System.Diagnostics.Process.Start(filePath); 

        }
        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                MessageBox.Show("Unable to release the Object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }

    }
}
