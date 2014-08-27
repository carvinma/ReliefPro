using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using NHibernate;
using ReliefProBLL;
using ReliefProMain.Models.HXs;
using UOMLib;
using ReliefProMain.Models;
using ReliefProModel;
using ReliefProDAL;
using System.IO;
using ReliefProCommon.CommonLib;
using ProII;
using System.Windows;
using System.Collections.ObjectModel;

namespace ReliefProMain.ViewModel.HXs
{
    public class TubeRuptureVM : ViewModelBase
    {
        public ICommand CalcCMD { get; set; }
        public ICommand OKCMD { get; set; }
        public SourceFile SourceFileInfo { get; set; }
        private ISession SessionPS;
        private ISession SessionPF;
        private string DirPlant { set; get; }
        private string DirProtectedSystem { set; get; }
        
        public TubeRuptureModel model { set; get; }
        public TubeRuptureVM(int ScenarioID, SourceFile sourceFileInfo, ISession SessionPS, ISession SessionPF, string dirPlant, string dirProtectedSystem)
        {
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;
            this.SourceFileInfo = sourceFileInfo;
            DirPlant = dirPlant;
            DirProtectedSystem = dirProtectedSystem;
            CalcCMD = new DelegateCommand<object>(CalcResult);
            OKCMD = new DelegateCommand<object>(Save);
        }

        public void CalcResult(object obj)
        {
            CustomStream cs = new CustomStream();//high压测流体
            CustomStreamBLL csbll=new CustomStreamBLL(SessionPF,SessionPS);
            ObservableCollection <CustomStream> feeds = csbll.GetStreams(SessionPS, false);
            cs = feeds[0];
            if (cs.Pressure < feeds[1].Pressure)
                cs = feeds[1];

            PSVDAL psvDAL = new PSVDAL();
            PSV psv = psvDAL.GetModel(SessionPS);
            double pressure = psv.Pressure;

            string FileFullPath = DirPlant + @"\" + SourceFileInfo.FileNameNoExt + @"\" +SourceFileInfo.FileName;
            double reliefFirePressure = pressure * psv.ReliefPressureFactor;
            string tempdir = DirProtectedSystem + @"\temp\";
            string dirLatent = tempdir + "TubeRupture";
            if (!Directory.Exists(dirLatent))
                Directory.CreateDirectory(dirLatent);
            string gd = Guid.NewGuid().ToString();
            string vapor = "S_" + gd.Substring(0, 5).ToUpper();
            string liquid = "S_" + gd.Substring(gd.Length - 5, 5).ToUpper();
            int ImportResult = 0;
            int RunResult = 0;
            PROIIFileOperator.DecompressProIIFile(FileFullPath, tempdir);
            string content = PROIIFileOperator.getUsableContent(cs.StreamName, tempdir);
            IFlashCalculate fcalc = ProIIFactory.CreateFlashCalculate(SourceFileInfo.FileVersion);
            string tray1_f = fcalc.Calculate(content, 1, "0", 3, "0", cs, vapor, liquid, dirLatent, ref ImportResult, ref RunResult);
            if (ImportResult == 1 || ImportResult == 2)
            {
                if (RunResult == 1 || RunResult == 2)
                {
                    IProIIReader reader = ProIIFactory.CreateReader(SourceFileInfo.FileVersion);
                    reader.InitProIIReader(tray1_f);
                    ProIIStreamData proIIVapor = reader.GetSteamInfo(vapor);
                    ProIIStreamData proIILiquid = reader.GetSteamInfo(liquid);
                    ProIIEqData flash = reader.GetEqInfo("Flash", "F_1");
                    reader.ReleaseProIIReader();
                    if (proIIVapor.VaporFraction == "0") //L
                    {
                        Calc(0);
                    }
                    else if (proIIVapor.VaporFraction == "1") //V
                    {
                        Calc(1);
                    }
                    else
                    {
                        Calc(2);
                    }


                }

                else
                {
                    MessageBox.Show("Prz file is error", "Message Box");
                }
            }
            else
            {
                MessageBox.Show("inp file is error", "Message Box");

            }



        }
        /// <summary>
        /// calcType  0全液相 L 1 全气相 V  2  混合 V/L
        /// </summary>
        /// <param name="calcType"></param>
        private void Calc(int calcType)
        {
            double d=0;
            double p1=0;
            double p2=0;
            double rmass=0;
            double k=0;
            bool b = false;
            double pcf = 0;
            switch (calcType)
            {
                case 0:
                    model.ReliefLoad = Algorithm.CalcWL(d, p1, p2, rmass);
                    break;
                case 1:
                    b = Algorithm.CheckCritial(p1, p2, k, ref pcf);
                    if (b)
                    {
                        model.ReliefLoad = Algorithm.CalcWv(d, p1, rmass, k);
                    }
                    else
                    {
                        model.ReliefLoad = Algorithm.CalcWvSecond(d, p1, rmass, k);
                    }
                    break;
                case 2:
                    //再做一次闪蒸，求出
                    CustomStream cs = new CustomStream(); //还是第一次做的物流线


                    string FileFullPath = DirPlant + @"\" + SourceFileInfo.FileNameNoExt + @"\" + SourceFileInfo.FileName;
                    double reliefFirePressure = pcf;
                    string tempdir = DirProtectedSystem + @"\temp\";
                    string dirLatent = tempdir + "TubeRupture2";
                    if (!Directory.Exists(dirLatent))
                        Directory.CreateDirectory(dirLatent);
                    string gd = Guid.NewGuid().ToString();
                    string vapor = "S_" + gd.Substring(0, 5).ToUpper();
                    string liquid = "S_" + gd.Substring(gd.Length - 5, 5).ToUpper();
                    int ImportResult = 0;
                    int RunResult = 0;
                    PROIIFileOperator.DecompressProIIFile(FileFullPath, tempdir);
                    string content = PROIIFileOperator.getUsableContent(cs.StreamName, tempdir);
                    IFlashCalculate fcalc = ProIIFactory.CreateFlashCalculate(SourceFileInfo.FileVersion);
                    string tray1_f = fcalc.Calculate(content, 1, "0", 3, "0", cs, vapor, liquid, dirLatent, ref ImportResult, ref RunResult);
                    if (ImportResult == 1 || ImportResult == 2)
                    {
                        if (RunResult == 1 || RunResult == 2)
                        {
                            IProIIReader reader = ProIIFactory.CreateReader(SourceFileInfo.FileVersion);
                            reader.InitProIIReader(tray1_f);
                            ProIIStreamData proIIVapor = reader.GetSteamInfo(vapor);
                            ProIIStreamData proIILiquid = reader.GetSteamInfo(liquid);
                            ProIIEqData flash = reader.GetEqInfo("Flash", "F_1");
                            reader.ReleaseProIIReader();
                            


                        }

                        else
                        {
                            MessageBox.Show("Prz file is error", "Message Box");
                        }
                    }
                    else
                    {
                        MessageBox.Show("inp file is error", "Message Box");

                    }




                    break;

            }

        }

        private void Save(object obj)
        {
            
            if (obj != null)
            {
                System.Windows.Window wd = obj as System.Windows.Window;
                if (wd != null)
                {
                    wd.DialogResult = true;
                }
            }
        }




    }
}
