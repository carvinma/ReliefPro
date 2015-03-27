using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using System.Linq;
using System.Windows;
using Microsoft.Practices.Prism.Commands;
using NHibernate;

using ReliefProMain.Models.ReactorLoops;
using ReliefProModel.ReactorLoops;
using ReliefProDAL;
using ReliefProModel;
using ReliefProMain.View;
using System.IO;
using ProII;
using System.Diagnostics;
using Microsoft.Win32;
using ReliefProDAL.ReactorLoops;
using ReliefProCommon.CommonLib;
using ReliefProCommon.Enum;

namespace ReliefProMain.ViewModel.ReactorLoops
{
    public class ReactorLoopSimulationVM:ViewModelBase
    {
        public ICommand OKCommand { get; set; }
        public ICommand CheckDataCommand { get; set; }
        public ICommand LaunchSimulatorCommand { get; set; }
        public ICommand RunSimulationCommand { get; set; }
        private string newInpFile;
        private string newPrzFile;
        private string przVersion;
        private string przFile;
        private string przFileName;
        private List<string> hxs;
        private List<string> flashs;
        private int ReactorLoopID;
        private ISession SessionPlant;
        private ISession SessionProtectedSystem;
        ReactorLoopEqDiffDAL eqDiffDAL;
        public List<ReactorLoopEqDiff> lst;
        private ObservableCollection<ReactorLoopEqDiffModel> _eqDiffs;
        public ObservableCollection<ReactorLoopEqDiffModel> EqDiffs
        {
            get { return _eqDiffs; }
            set
            {
                _eqDiffs = value;
                this.NotifyPropertyChanged("EqDiffs");
            }
        }

        public bool IsSolved;
        public bool IsMatched;
        private string _SimulationResult;
        public string SimulationResult
        {
            get { return _SimulationResult; }
            set
            {
                _SimulationResult = value;
                this.NotifyPropertyChanged("SimulationResult");
            }
        }

        private string _MatchResult = ColorBorder.blue.ToString();
        public string MatchResult
        {
            get { return _MatchResult; }
            set
            {
                _MatchResult = value;
                this.NotifyPropertyChanged("MatchResult");
            }
        }

        private string _IsSolved_Color;
        public string IsSolved_Color
        {
            get { return _IsSolved_Color; }
            set
            {
                _IsSolved_Color = value;
                this.NotifyPropertyChanged("IsSolved_Color");
            }
        }

        private string _IsMatched_Color = ColorBorder.blue.ToString();
        public string IsMatched_Color
        {
            get { return _IsMatched_Color; }
            set
            {
                _IsMatched_Color = value;
                this.NotifyPropertyChanged("IsMatched_Color");
            }
        }



        private void InitCMD()
        {
            OKCommand = new DelegateCommand<object>(OK);
            CheckDataCommand = new DelegateCommand<object>(CheckData);
            LaunchSimulatorCommand = new DelegateCommand<object>(LaunchSimulator);
            RunSimulationCommand = new DelegateCommand<object>(RunSimulation);
        }
        
        public ReactorLoopSimulationVM(int ReactorLoopID,string newInpFile,string sourcePrzFile,string sourcePrzVersion,List<string> hxNames,List<string> flashNames,ISession sessionProtectedSystem,ISession sessionPlant,bool isSolved,bool isMatched,List<ReactorLoopEqDiff> lstDiff)
        {
            InitCMD();
            lst = new List<ReactorLoopEqDiff>();
            EqDiffs = new ObservableCollection<ReactorLoopEqDiffModel>();
            przVersion = sourcePrzVersion;
            this.newInpFile = newInpFile;
            this.newPrzFile = newInpFile.Substring(0,newInpFile.Length-3) + @"prz";
            hxs = hxNames;
            flashs = flashNames;
            SessionPlant = sessionPlant;
            SessionProtectedSystem=sessionProtectedSystem;
            przFile = sourcePrzFile;
            przFileName = System.IO.Path.GetFileName(sourcePrzFile);
            this.ReactorLoopID=ReactorLoopID;
            
            eqDiffDAL = new ReactorLoopEqDiffDAL();
            if (ReactorLoopID > 0)
            {
                IList<ReactorLoopEqDiff> list = eqDiffDAL.GetList(SessionProtectedSystem, ReactorLoopID);
                foreach (ReactorLoopEqDiff diff in list)
                {
                    ReactorLoopEqDiffModel m = new ReactorLoopEqDiffModel(diff);
                    EqDiffs.Add(m);
                }
                IsSolved = isSolved;
                IsMatched = isMatched;
                GetMessage();
            }
            else
            {
                MatchResult = string.Empty;
                SimulationResult = string.Empty;
                if (lstDiff!=null)
                {
                    IsSolved = isSolved;
                    IsMatched = isMatched;
                    GetMessage();
                    foreach (ReactorLoopEqDiff diff in lstDiff)
                    {
                        ReactorLoopEqDiffModel m = new ReactorLoopEqDiffModel(diff);
                        EqDiffs.Add(m);
                    }
                }
            }
        }

        private void CheckData(object obj)
        {
            //IProIIImport import = ProIIFactory.CreateProIIImport(przVersion);
            ProIICalculate proiicalc = new ProIICalculate(przVersion);
            int ImportResult = -1;
            int RunResult = -1;
            newPrzFile = proiicalc.ImportProIIKeyWordFile(newInpFile, ref ImportResult, ref RunResult);
            if (ImportResult == 1 || ImportResult == 2)
            {
                MessageBox.Show("Data is right!", "Message Box");
                if (RunResult == 1 || RunResult == 2)
                {
                    IsSolved = true;                   
                    return;
                }
                else
                {
                    IsSolved = false;
                    return;
                }
                
            }
            else
            {
                MessageBox.Show("inp file is error!", "Message Box");
                return;
            }
            
        }

        Process caseProcess;
        private void LaunchSimulator(object obj)
        {
            if (string.IsNullOrEmpty(newPrzFile))
            {
                MessageBox.Show("Please Check Data first.", "Message Box",MessageBoxButton.OK,MessageBoxImage.Warning);
                return;
            }
            if (File.Exists(newPrzFile))
            {
                if (caseProcess == null || caseProcess.HasExited)
                {
                    caseProcess = ProIIHelper.Run(przVersion, newPrzFile);
                }
                else
                {
                    MessageBox.Show("this file was opened.", "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            else
            {
                if (File.Exists(newInpFile))
                {
                    if (MessageBox.Show("Dou you want to open keyword file?", "Message Box", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        ProIIHelper.Open(newInpFile);
                    }
                }
                else
                {
                    MessageBox.Show("Please run simulation first.", "Message Box");
                }
            }
            if (!IsSolved)
            {
                MessageBox.Show("prz File is error.", "Message Box");
                return;
            }
            ProIIHelper.Run(przVersion, newPrzFile);
        }
        private void RunSimulation(object obj)
        {
            if (string.IsNullOrEmpty(newPrzFile))
            {
                MessageBox.Show("Please Check Data first.","Message Box");
                return;
            }
            if (!IsSolved)
            {
                MessageBox.Show("prz File is error.", "Message Box");
                return;
            }
            //run prz file ,then compare hx duty
            EqDiffs.Clear();
            lst.Clear();
            SplashScreenManager.Show();
            SplashScreenManager.SentMsgToScreen("Reading Data From ProII Files");
            double diff = 1;
            bool b = true;
            ProIIEqDataDAL dal = new ProIIEqDataDAL();
            //IProIIReader reader = ProIIFactory.CreateReader(przVersion);
            //reader.InitProIIReader(newPrzFile);

            ProIIReader reader = new ProIIReader(przVersion, newPrzFile);
            
            SplashScreenManager.SentMsgToScreen("Calculation finished");
            SplashScreenManager.Close();
            foreach (string s in hxs)
            {
                ProIIEqData eq1 = dal.GetModel(SessionPlant, przFileName, s);
                ProIIEqData eq2 = reader.GetEqInfo("Hx", s);
                double d = 0;
                if (double.Parse(eq1.DutyCalc)!= 0)
                {
                    d = 100 * Math.Abs(double.Parse(eq1.DutyCalc) - double.Parse(eq2.DutyCalc)) / Math.Abs(double.Parse(eq1.DutyCalc));
                }
                d = Math.Round(d, 10);
                ReactorLoopEqDiff eqdiff = new ReactorLoopEqDiff();
                eqdiff.CurrentDuty = double.Parse(eq2.DutyCalc) * 3600;
                eqdiff.Diff = d;
                eqdiff.EqName = s;
                eqdiff.EqType = "HX";
                eqdiff.OrginDuty = double.Parse(eq1.DutyCalc) * 3600;
                eqdiff.ReactorLoopID = ReactorLoopID;
                ReactorLoopEqDiffModel m = new ReactorLoopEqDiffModel(eqdiff);
                EqDiffs.Add(m);
                lst.Add(eqdiff);
                b = b && (d < 1);

            }
            foreach (string s in flashs)
            {
                ProIIStreamDataDAL proiisdal = new ProIIStreamDataDAL();
                ProIIEqData eq1 = dal.GetModel(SessionPlant, przFileName, s);
                ProIIEqData eq2 = reader.GetEqInfo("Flash", s);
                double d = 0;
                string[] products = eq1.ProductData.Split(',');
                CustomStream vapor1 = null;
                foreach (string p in products)
                {
                    ProIIStreamData sd = proiisdal.GetModel(SessionPlant, p, przFileName);
                    if (sd.VaporFraction == "1")
                    {
                        if (sd.VaporFraction == "1")
                        {
                            vapor1 = ProIIToDefault.ConvertProIIStreamToCustomStream(sd);
                            break;
                        }
                    }
                }
                if (vapor1 != null)
                {
                    ProIIStreamData proiivapor = reader.GetStreamInfo(vapor1.StreamName);
                    CustomStream vapor2 = ProIIToDefault.ConvertProIIStreamToCustomStream(proiivapor);
                    d = 100 * Math.Abs(vapor1.WeightFlow - vapor2.WeightFlow) / vapor1.WeightFlow;
                    d = Math.Round(d, 10);
                    ReactorLoopEqDiff eqdiff = new ReactorLoopEqDiff();
                    eqdiff.CurrentDuty = double.Parse(eq2.DutyCalc) * 3600;
                    eqdiff.Diff = d;
                    eqdiff.EqName = s;
                    eqdiff.EqType = "Flash";
                    eqdiff.OrginDuty = double.Parse(eq1.DutyCalc) * 3600;
                    eqdiff.ReactorLoopID = ReactorLoopID;
                    ReactorLoopEqDiffModel m = new ReactorLoopEqDiffModel(eqdiff);
                    EqDiffs.Add(m);
                    lst.Add(eqdiff);
                    b = b && (d < 1);
                }
            }
            //reader.ReleaseProIIReader();
            IsMatched = b;
            GetMessage();

        }
        private void OK(object obj)
        {
            if (caseProcess != null && !caseProcess.HasExited)
            {
                MessageBox.Show("this file is opened,please close it first.", "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (obj != null)
            {
                if (string.IsNullOrEmpty(newPrzFile))
                {
                    MessageBox.Show("Please Check Data first.", "Message Box",MessageBoxButton.OK,MessageBoxImage.Warning);
                    return;
                }

                if (!File.Exists(newPrzFile))
                {
                    MessageBox.Show("Please run simulation first.", "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!IsSolved)
                {
                    MessageBox.Show("Simulation not solved.", "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                System.Windows.Window wd = obj as System.Windows.Window;
                wd.DialogResult = true;
            }
        }

        void GetMessage()
        {
            if (IsSolved)
            {
                SimulationResult = "Simulation Solved.";
                IsSolved_Color = ColorBorder.blue.ToString();
            }
            else
            {
                SimulationResult = "Simulation Unsolved.";
                IsSolved_Color = ColorBorder.red.ToString();
            }
            if (IsMatched)
            {
                MatchResult = "Base Case Simulation Matches Original Model";
                IsMatched_Color = ColorBorder.blue.ToString();
            }
            else
            {
                MatchResult = "Base Case Simulation doesn't Match Original Model";
                IsMatched_Color = ColorBorder.red.ToString();
            }
        }





    }

}
