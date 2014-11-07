using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using System.Linq;
using System.Windows;
using Microsoft.Practices.Prism.Commands;
using NHibernate;
using ReliefProLL;
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
        private int ReactorLoopID;
        private ISession SessionPlant;
        private ISession SessionProtectedSystem;
        ReactorLoopEqDiffDAL eqDiffDAL;
        private List<ReactorLoopEqDiff> lst;
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
        private void InitCMD()
        {
            OKCommand = new DelegateCommand<object>(OK);
            CheckDataCommand = new DelegateCommand<object>(CheckData);
            LaunchSimulatorCommand = new DelegateCommand<object>(LaunchSimulator);
            RunSimulationCommand = new DelegateCommand<object>(RunSimulation);
        }
        
        public ReactorLoopSimulationVM(int ReactorLoopID,string newInpFile,string sourcePrzFile,string sourcePrzVersion,List<string> hxNames,ISession sessionProtectedSystem,ISession sessionPlant)
        {
            InitCMD();
            lst = new List<ReactorLoopEqDiff>();
            EqDiffs = new ObservableCollection<ReactorLoopEqDiffModel>();
            przVersion = sourcePrzVersion;
            this.newInpFile = newInpFile;
            hxs = hxNames;
            SessionPlant = sessionPlant;
            SessionProtectedSystem=sessionProtectedSystem;
            przFile = sourcePrzFile;
            przFileName = System.IO.Path.GetFileName(sourcePrzFile);
            this.ReactorLoopID=ReactorLoopID;
            eqDiffDAL = new ReactorLoopEqDiffDAL();
            if (ReactorLoopID > 0)
            {                
                IList<ReactorLoopEqDiff> list= eqDiffDAL.GetList(SessionProtectedSystem, ReactorLoopID);
                foreach (ReactorLoopEqDiff diff in list)
                {
                    ReactorLoopEqDiffModel m = new ReactorLoopEqDiffModel(diff);
                    EqDiffs.Add(m);
                }
            }
        }

        private void CheckData(object obj)
        {
            IProIIImport import = ProIIFactory.CreateProIIImport(przVersion);
            int ImportResult = -1;
            int RunResult = -1;
            newPrzFile = import.ImportProIIINP(newInpFile, out ImportResult, out RunResult);
            if (ImportResult == 1 || ImportResult == 2)
            {
                if (RunResult == 1 || RunResult == 2)
                {
                    MessageBox.Show("Data is right.", "Message Box");
                    return;
                }

                else
                {
                    MessageBox.Show("Prz file is error!", "Message Box");
                    return;
                }
            }
            else
            {
                MessageBox.Show("inp file is error!", "Message Box");
                return;
            }
            
        }
        private void LaunchSimulator(object obj)
        {
            ProIIHelper.Run(przVersion, newPrzFile);
        }
        private void RunSimulation(object obj)
        {
            //run prz file ,then compare hx duty
            EqDiffs.Clear();
            lst.Clear();
            SplashScreenManager.Show();
            SplashScreenManager.SentMsgToScreen("Reading Data From ProII Files");
            double diff = 1;
            bool b = true;
            ProIIEqDataDAL dal = new ProIIEqDataDAL();
            IProIIReader reader = ProIIFactory.CreateReader(przVersion);
            reader.InitProIIReader(newPrzFile);
            SplashScreenManager.SentMsgToScreen("Calculation finished");
            SplashScreenManager.Close();
            foreach (string s in hxs)
            {
                ProIIEqData eq1 = dal.GetModel(SessionPlant, przFileName, s);
                ProIIEqData eq2 = reader.GetEqInfo("Hx", s);
                double d = Math.Abs(double.Parse(eq1.DutyCalc) - double.Parse(eq2.DutyCalc)) ;
                if (d > 1)
                {
                    ReactorLoopEqDiff eqdiff = new ReactorLoopEqDiff();
                    eqdiff.CurrentDuty = double.Parse(eq2.DutyCalc) * 3600;
                    eqdiff.Diff = d;
                    eqdiff.EqName = s;
                    eqdiff.EqType = "HX";
                    eqdiff.OrginDuty = double.Parse(eq1.DutyCalc) * 3600;
                    ReactorLoopEqDiffModel m = new ReactorLoopEqDiffModel(eqdiff);
                    EqDiffs.Add(m);
                    lst.Add(eqdiff);
                }
            }
            
            reader.ReleaseProIIReader();
        }
        private void OK(object obj)
        {
            if (obj != null)
            {                
                eqDiffDAL.Save(SessionProtectedSystem,ReactorLoopID,lst);
                System.Windows.Window wd = obj as System.Windows.Window;
                wd.DialogResult = true;
            }
        }

        





    }

}
