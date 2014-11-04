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
            //open prz 文件
            string proiiFilePath = newPrzFile;
            ProcessStartInfo psInfo = new ProcessStartInfo();
            //HKEY_LOCAL_MACHINE\SOFTWARE\SIMSCI\PRO/II
            RegistryKey rk = Registry.LocalMachine;
            RegistryKey softWare = rk.OpenSubKey("Software");
            RegistryKey simsci = softWare.OpenSubKey("SIMSCI");
            if (simsci != null && simsci.SubKeyCount != 0)
            {
                RegistryKey proii = simsci.OpenSubKey("PRO/II");
                if (proii != null && proii.SubKeyCount != 0)
                {
                    RegistryKey version = proii.OpenSubKey(przVersion);
                    if (version!=null&&version.SubKeyCount != 0)
                    {
                        psInfo.FileName = version.GetValue("SecDir").ToString() + @"\PROII.exe";
                        psInfo.Arguments = string.Format("/I='{0}' {1}", version.GetValue("SecIni").ToString(), proiiFilePath);
                        Process pro = Process.Start(psInfo);
                    }
                }
            }
            
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
