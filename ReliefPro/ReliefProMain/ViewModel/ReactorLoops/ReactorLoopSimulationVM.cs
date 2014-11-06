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
        ISession SessionPlant;
        private ObservableCollection<ReactorLoopEqDiffModel> _eqDiff;
        public ObservableCollection<ReactorLoopEqDiffModel> EqDiff
        {
            get { return _eqDiff; }
            set
            {
                _eqDiff = value;
                this.NotifyPropertyChanged("EqDiff");
            }
        }
        private void InitCMD()
        {
            OKCommand = new DelegateCommand<object>(OK);
            CheckDataCommand = new DelegateCommand<object>(CheckData);
            LaunchSimulatorCommand = new DelegateCommand<object>(LaunchSimulator);
            RunSimulationCommand = new DelegateCommand<object>(RunSimulation);
        }
        
        public ReactorLoopSimulationVM(int ReactorLoopID,string newInpFile,string sourcePrzFile,string sourcePrzVersion,List<string> hxNames,ISession sessionPlant)
        {
            InitCMD();
            przVersion = sourcePrzVersion;
            this.newInpFile = newInpFile;
            hxs = hxNames;
            SessionPlant = sessionPlant;
            przFile = sourcePrzFile;
            przFileName = System.IO.Path.GetFileName(sourcePrzFile);
            this.ReactorLoopID=ReactorLoopID;
            if (ReactorLoopID > 0)
            {
                
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
                double d = Math.Abs(double.Parse(eq1.DutyCalc) - double.Parse(eq2.DutyCalc));
                if (d > diff)
                {
                    b = false;
                    break;
                }

                if (b)
                {
                    MessageBox.Show("OK", "Message Box");
                }
                else
                {
                    MessageBox.Show("Diff", "Message Box");
                }
            }
            reader.ReleaseProIIReader();
        }
        private void OK(object obj)
        {
            if (obj != null)
            {
                System.Windows.Window wd = obj as System.Windows.Window;

                wd.DialogResult = true;

            }
        }

        





    }

}
