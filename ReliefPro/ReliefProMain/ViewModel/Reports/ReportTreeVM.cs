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
using ReliefProLL;

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
            string dbPlantFile = CurrentPlantPath + @"\plant.mdb";
            List<Tuple<int, List<string>>> UnitPath = new List<Tuple<int, List<string>>>();
            List<UnitVM> list = GetCheckedUnits();
            if (list.Count > 0)
            {
                foreach (UnitVM uvm in list)
                {
                    List<string> ReportPath = new List<string>();
                    ReportPath.Add(dbPlantFile);
                    string dirUnit = CurrentPlantPath + @"\" + uvm.UnitName;
                    foreach (PSVM p in uvm.PSCollection)
                    {
                        if (p.IsChecked)
                        {
                            ReportPath.Add(dirUnit + @"\" + p.PSName + @"\protectedsystem.mdb");
                        }
                    }
                    Tuple<int, List<string>> t = new Tuple<int, List<string>>(uvm.ID, ReportPath);
                    UnitPath.Add(t);

                }

                PlantSummaryView view = new PlantSummaryView();
                PlantSummaryVM vm = new PlantSummaryVM(UnitPath);
                view.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                view.WindowState = WindowState.Maximized;
                view.DataContext = vm;
                view.ShowDialog();
            }
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
            GlobalDefaultBLL globalBLL = new GlobalDefaultBLL(TempleSession.Session);
            TempleSession.lstFlareSys = globalBLL.GetFlareSystem();

            string dbPlantFile = CurrentPlantPath + @"\plant.mdb";
            UnitVM uvm = GetSingleCheckedUnit();
            if (uvm != null)
            {
                List<string> ReportPath = new List<string>();
                ReportPath.Add(dbPlantFile);
                string unitPath = CurrentPlantPath + @"\" + uvm.UnitName;
                foreach (PSVM p in uvm.PSCollection)
                {
                    if (p.IsChecked)
                    {
                        ReportPath.Add(unitPath + @"\" + p.PSName + @"\protectedsystem.mdb");
                    }
                }
                PUsummaryView view = new PUsummaryView();
                PUsummaryVM vm = new PUsummaryVM(uvm.ID, ReportPath);
                view.WindowState = WindowState.Maximized;
                view.DataContext = vm;
                view.ShowDialog();
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
                            break;
                        }
                        foreach (PSVM ps in uvm.PSCollection)
                        {
                            if (ps.IsChecked)
                            {
                                chkedUnit.Add(uvm);
                            }
                        }
                    }
                }
            }
            return chkedUnit;

        }
    }
}
