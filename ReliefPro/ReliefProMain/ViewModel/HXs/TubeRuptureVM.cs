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
using ReliefProModel.HXs;
using ReliefProDAL.HXs;

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
        private CustomStream csHigh;
        private double reliefPressure;
        private CustomStream csVapor;
        private CustomStream csLiquid;
        UOMLib.UOMEnum uomEnum;
        public TubeRuptureModel model { set; get; }
        TubeRuptureDAL dal = new TubeRuptureDAL();
        public TubeRuptureVM(int ScenarioID, SourceFile sourceFileInfo, ISession SessionPS, ISession SessionPF, string dirPlant, string dirProtectedSystem)
        {
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;
            this.SourceFileInfo = sourceFileInfo;
            DirPlant = dirPlant;
            DirProtectedSystem = dirProtectedSystem;
            CalcCMD = new DelegateCommand<object>(CalcResult);
            OKCMD = new DelegateCommand<object>(Save);
            if (ScenarioID == 0)
            {
                TubeRupture dbmodel = new TubeRupture();
                model = new TubeRuptureModel(dbmodel);
            }
            else
            {

                TubeRupture dbmodel = dal.GetModelByScenarioID(SessionPS, ScenarioID);
                model = new TubeRuptureModel(dbmodel);
                
            }
            BasicUnit BU;
            BasicUnitDAL dbBU = new BasicUnitDAL();
            IList<BasicUnit> list = dbBU.GetAllList(SessionPF);

            BU = list.Where(s => s.IsDefault == 1).Single();
            uomEnum = new UOMLib.UOMEnum(SessionPF);
            model.ReliefLoadUnit = uomEnum.UserMassRate;
            model.ReliefTemperatureUnit = uomEnum.UserTemperature;
            model.ReliefPressureUnit = uomEnum.UserPressure;
            model.ODUnit = uomEnum.UserLength;
            ReadConvert();
            
        }

        public void CalcResult(object obj)
        {
            
            CustomStreamBLL csbll=new CustomStreamBLL(SessionPF,SessionPS);
            ObservableCollection <CustomStream> feeds = csbll.GetStreams(SessionPS, false);
            csHigh = feeds[0];
            if (csHigh.Pressure < feeds[1].Pressure)
                csHigh = feeds[1];

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
            string content = PROIIFileOperator.getUsableContent(csHigh.StreamName, tempdir);
            IFlashCalculate fcalc = ProIIFactory.CreateFlashCalculate(SourceFileInfo.FileVersion);
            string tray1_f = fcalc.Calculate(content, 1, "0", 3, "0", csHigh, vapor, liquid, dirLatent, ref ImportResult, ref RunResult);
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
                    csVapor = ProIIToDefault.ConvertProIIStreamToCustomStream(proIIVapor);
                    csLiquid = ProIIToDefault.ConvertProIIStreamToCustomStream(proIILiquid);


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
            double d = UnitConvert.Convert(uomEnum.UserLength, model.ODUnit,  model.OD);
            double p1=csHigh.Pressure;
            double p2=reliefPressure;
            double rmass=0;
            double k=0;
            bool b = false;
            double pcf = 0;
            b = Algorithm.CheckCritial(p1, p2, k, ref pcf);
            switch (calcType)
            {
                case 0:
                    rmass = csLiquid.BulkDensityAct;
                    model.ReliefLoad = Algorithm.CalcWL(d, p1, p2, rmass);
                    model.ReliefMW = csLiquid.BulkMwOfPhase;
                    model.ReliefPressure = csLiquid.Pressure;
                    model.ReliefTemperature = csLiquid.Temperature;
                    break;
                case 1:
                    k = csVapor.BulkCPCVRatio; 
                    rmass=csVapor.BulkDensityAct;
                    if (b)
                    {
                        model.ReliefLoad = Algorithm.CalcWv(d, p1, rmass, k);
                    }
                    else
                    {
                        model.ReliefLoad = Algorithm.CalcWvSecond(d, p1, rmass, k);
                    }
                    model.ReliefMW = csVapor.BulkMwOfPhase;
                    model.ReliefPressure = csVapor.Pressure;
                    model.ReliefTemperature = csVapor.Temperature;
                    break;
                case 2:
                    //再做一次闪蒸，求出
                    if (b)
                    {
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
                        string content = PROIIFileOperator.getUsableContent(csHigh.StreamName, tempdir);
                        IFlashCalculate fcalc = ProIIFactory.CreateFlashCalculate(SourceFileInfo.FileVersion);
                        string tray1_f = fcalc.Calculate(content, 1, "0", 3, "0", csHigh, vapor, liquid, dirLatent, ref ImportResult, ref RunResult);
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
                                CustomStream csVapor2 = ProIIToDefault.ConvertProIIStreamToCustomStream(proIIVapor);
                                CustomStream csLiquid2 = ProIIToDefault.ConvertProIIStreamToCustomStream(proIILiquid);

                                double Rv = csVapor2.WeightFlow / csHigh.WeightFlow;
                                double KL = Algorithm.CalcKL(p1, csLiquid2.Pressure, csLiquid2.BulkDensityAct);
                                double Kv = Algorithm.CalcKv(p1, csVapor2.Pressure, csVapor2.BulkDensityAct);
                                model.ReliefLoad = Algorithm.CalcWH(Rv, Kv, KL, d);
                                model.ReliefMW = csVapor2.BulkMwOfPhase;
                                model.ReliefPressure = csVapor2.Pressure;
                                model.ReliefTemperature = csVapor2.Temperature;
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
                    else
                    {
                        double Rv=csVapor.WeightFlow/csHigh.WeightFlow;
                        double KL=Algorithm.CalcKL(p1,csLiquid.Pressure,csLiquid.BulkDensityAct);
                        double Kv = Algorithm.CalcKv(p1, csVapor.Pressure, csVapor.BulkDensityAct);
                        model.ReliefLoad = Algorithm.CalcWH(Rv, Kv, KL,d);
                        model.ReliefMW = csVapor.BulkMwOfPhase;
                        model.ReliefPressure = csVapor.Pressure;
                        model.ReliefTemperature = csVapor.Temperature;
                    }
                    break;

            }

        }

        private void Save(object obj)
        {
            if (!model.CheckData()) return; 
            if (obj != null)
            {
                System.Windows.Window wd = obj as System.Windows.Window;
                if (wd != null)
                {
                    WriteConvert();
                    dal.Update(model.dbmodel, SessionPS);
                    SessionPS.Flush();
                    wd.DialogResult = true;
                }
            }
        }

        private void ReadConvert()
        {
            model.OD = UnitConvert.Convert(UOMEnum.Length, model.ODUnit, model.OD);
            model.ReliefLoad = UnitConvert.Convert(UOMEnum.MassRate, model.ReliefLoadUnit, model.ReliefLoad);
            model.ReliefTemperature = UnitConvert.Convert(UOMEnum.Temperature, model.ReliefTemperatureUnit, model.ReliefTemperature);
            model.ReliefPressure = UnitConvert.Convert(UOMEnum.Pressure, model.ReliefPressureUnit, model.ReliefPressure);
        }
        private void WriteConvert()
        {
            model.dbmodel.OD = UnitConvert.Convert(model.ODUnit, UOMLib.UOMEnum.Length.ToString(), model.OD);
            model.dbmodel.ReliefMW = model.ReliefMW;
            model.dbmodel.ReliefLoad = UnitConvert.Convert(model.ReliefLoadUnit, UOMLib.UOMEnum.MassRate.ToString(), model.ReliefLoad);
            model.dbmodel.ReliefTemperature = UnitConvert.Convert(model.ReliefTemperatureUnit, UOMLib.UOMEnum.Temperature.ToString(), model.ReliefTemperature);
            model.dbmodel.ReliefPressure = UnitConvert.Convert(model.ReliefPressureUnit, UOMLib.UOMEnum.Pressure.ToString(), model.ReliefPressure);
        }

    }
}
