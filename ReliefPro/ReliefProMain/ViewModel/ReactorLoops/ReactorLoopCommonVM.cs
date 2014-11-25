using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using NHibernate;

using ReliefProMain.Models.ReactorLoops;
using UOMLib;
using ReliefProModel;
using ReliefProCommon.CommonLib;
using ReliefProModel.ReactorLoops;
using ReliefProDAL.ReactorLoops;
using System.IO;
using ProII;
using System.Windows;
using ReliefProDAL;
using ReliefProBLL;

namespace ReliefProMain.ViewModel.ReactorLoops
{
    public class ReactorLoopCommonVM:ViewModelBase
    {
        public ICommand CalcCMD { get; set; }
        public ICommand OKCMD { get; set; }

        public ICommand RunCaseSimulationCMD { get; set; }
        public ICommand LaunchSimulatorCMD { get; set; }

        private ISession SessionPS;
        private ISession SessionPF;
        private SourceFile SourceFileInfo;
        private string DirPlant;
        private string DirProtectedSystem;
        public ReactorLoopCommonModel model { get; set; }
        private ReactorLoopBLL reactorBLL;
        private int reactorType;
        ReactorLoop rl;
        private double reliefPressure;
        private string coldVaporStream;
        private CustomStream compressorH2Stream;
        private string newPrzFile;
        private string newInpFile;

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



        /// <summary>
        /// 0-ReactorLoopBlockedOutlet,1-LossOfReactorQuench,2-LossOfColdFeed
        /// </summary>
        /// <param name="ScenarioID"></param>
        /// <param name="SessionPS"></param>
        /// <param name="SessionPF"></param>
        /// <param name="ReactorType"> 0-ReactorLoopBlockedOutlet,1-LossOfReactorQuench</param>
        public ReactorLoopCommonVM(int ScenarioID, SourceFile SourceFileInfo, ISession SessionPS, ISession SessionPF, string DirPlant, string DirProtectedSystem, int ReactorType)
        {
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;
            this.SourceFileInfo = SourceFileInfo;
            this.DirPlant = DirPlant;
            this.DirProtectedSystem = DirProtectedSystem;
            reactorType = ReactorType;
            OKCMD = new DelegateCommand<object>(Save);
            CalcCMD = new DelegateCommand<object>(CalcResult);
            RunCaseSimulationCMD = new DelegateCommand<object>(RunCaseSimulation);
            LaunchSimulatorCMD = new DelegateCommand<object>(LaunchSimulator);

            PSVBLL psvbll = new PSVBLL(SessionPS);
            reliefPressure = psvbll.GetReliefPressure();

            reactorBLL = new ReactorLoopBLL(SessionPS, SessionPF);
            var blockModel = reactorBLL.GetBlockedOutletModel(ScenarioID, reactorType);
            blockModel = reactorBLL.ReadConvertBlockedOutletModel(blockModel);
            
            ReactorLoopDAL rldal = new ReactorLoopDAL();
            rl = rldal.GetModel(SessionPS);
            ProIIEqDataDAL eqDataDAL = new ProIIEqDataDAL();
            ProIIEqData eqData = eqDataDAL.GetModel(SessionPF, SourceFileInfo.FileName, rl.ColdHighPressureSeparator);
            string[] chcsList = eqData.ProductData.Split(',');

            ProIIStreamDataDAL streamDataDAL = new ProIIStreamDataDAL();
            foreach (string s in chcsList)
            {
                ProIIStreamData ps = streamDataDAL.GetModel(SessionPF, s, SourceFileInfo.FileName);
                if (ps.VaporFraction == "1")
                {
                    coldVaporStream = s;
                    break;
                }
            }
            model = new ReactorLoopCommonModel(blockModel);
            model.dbmodel.ScenarioID = ScenarioID;
            if (model.dbmodel.ID == 0)
            {
                CustomStreamDAL csdal = new CustomStreamDAL();
                CustomStream cset1 = csdal.GetModel(SessionPS, rl.EffluentStream);
                CustomStream cset2 = csdal.GetModel(SessionPS, rl.EffluentStream2);
                model.EffluentTemperature = 0;
                if (cset1 != null)
                {
                    model.EffluentTemperature = cset1.Temperature;
                }
                if (cset2 != null)
                {
                    model.EffluentTemperature2 = cset2.Temperature;
                }
                SimulationResult = string.Empty;
            }
            else
            {
                if (model.IsSolved)
                {
                    SimulationResult = "Simulation resolved.";
                }
                else
                {
                    SimulationResult = "Simulation not resolved.";
                }
            }
            UOMLib.UOMEnum uomEnum = UOMSingle.UomEnums.FirstOrDefault(p => p.SessionPlant == this.SessionPF);
            model.EffluentTemperatureUnit = uomEnum.UserTemperature;
            model.EffluentTemperature2Unit = uomEnum.UserTemperature;
            model.MaxGasRateUnit = uomEnum.UserMassRate;
            model.TotalPurgeRateUnit = uomEnum.UserMassRate;
            model.ReliefLoadUnit = uomEnum.UserMassRate;
            model.ReliefTemperatureUnit = uomEnum.UserTemperature;
            model.ReliefPressureUnit = uomEnum.UserPressure;
        }
        private void WriteConvertModel()
        {
            model.dbmodel.EffluentTemperature = UnitConvert.Convert(model.EffluentTemperatureUnit, UOMLib.UOMEnum.Temperature.ToString(), model.EffluentTemperature);
            model.dbmodel.EffluentTemperature2 = UnitConvert.Convert(model.EffluentTemperature2Unit, UOMLib.UOMEnum.Temperature.ToString(), model.EffluentTemperature2);
            model.dbmodel.MaxGasRate = UnitConvert.Convert(model.MaxGasRateUnit, UOMLib.UOMEnum.MassRate.ToString(), model.MaxGasRate);
            model.dbmodel.TotalPurgeRate = UnitConvert.Convert(model.TotalPurgeRateUnit, UOMLib.UOMEnum.MassRate.ToString(), model.TotalPurgeRate);
            model.dbmodel.ReliefMW = model.ReliefMW;
            model.dbmodel.ReliefLoad = UnitConvert.Convert(model.ReliefLoadUnit, UOMLib.UOMEnum.MassRate.ToString(), model.ReliefLoad);
            model.dbmodel.ReliefTemperature = UnitConvert.Convert(model.ReliefTemperatureUnit, UOMLib.UOMEnum.Temperature.ToString(), model.ReliefTemperature);
            model.dbmodel.ReliefPressure = UnitConvert.Convert(model.ReliefPressureUnit, UOMLib.UOMEnum.Pressure.ToString(), model.ReliefPressure);
            model.dbmodel.ReliefCpCv = model.ReliefCpCv;
            model.dbmodel.ReliefZ = model.ReliefZ;
            model.dbmodel.ReactorType = reactorType;
            model.dbmodel.IsSolved = model.IsSolved;
        }

        /// <summary>
        /// 读取新prz文件的inp文件，然后设置对应物流线的信息，生成新的inp文件。
        /// </summary>
        /// <param name="obj"></param>
        private void RunCaseSimulation(object obj)
        {
            CalcResult(obj);
            //读取新构建的prz文件的inp文件。
            //string caseDir = "";
            //PROIIFileOperator.DecompressProIIFile(rlPrzFile, caseDir);
            //string inpFile = rlPrzFile.Substring(0, rlPrzFile.Length - 4) + "_backup.inp";

            //讲inp文件里的feed的RATE(WT)=0. 然后再导入inp文件，生成prz文件，查看它是否正确
        }

        private void LaunchSimulator(object obj)
        {
            ProIIHelper.Run(SourceFileInfo.FileVersion, newPrzFile);
        }
        private void CalcResult(object obj)
        {
            ReactorLoopDAL rldal = new ReactorLoopDAL();
            rl = rldal.GetModel(SessionPS);
            ProIIEqDataDAL eqDataDAL = new ProIIEqDataDAL();
            ProIIEqData eqData = eqDataDAL.GetModel(SessionPF, SourceFileInfo.FileName, rl.ColdHighPressureSeparator);
            int idx = eqData.ProductData.IndexOf(",");
            coldVaporStream = eqData.ProductData.Substring(0, idx);

            ProIIStreamDataDAL streamDataDAL = new ProIIStreamDataDAL();
            ProIIStreamData streamData = streamDataDAL.GetModel(SessionPF, rl.CompressorH2Stream, SourceFileInfo.FileName);
            compressorH2Stream = ProIIToDefault.ConvertProIIStreamToCustomStream(streamData);

            switch (reactorType)
            {
                case 0:
                    CalcBlocket();
                    break;
                case 1:
                    CalcLossOfReactorQuench();
                    break;
                case 2:
                   // CalcLossOfColdFeed(); 由于需要加入hx stop信息，从这迁移到GeneralFailureCommonVM里
                    break;


            }

        }
        private void CalcBlocket()
        {
            SplashScreenManager.Show(10);
            SplashScreenManager.SentMsgToScreen("Simulation starting ......1%");
            string rpPrzFile = DirProtectedSystem + @"\myrp\myrp.prz";
            string rpInpDir = DirProtectedSystem + @"\myrp";
            PROIIFileOperator.DecompressProIIFile(rpPrzFile, rpInpDir);
            string inpFile = rpInpDir + @"\myrp.inp";
            SplashScreenManager.SentMsgToScreen("Simulation ......10%");
            string[] lines = System.IO.File.ReadAllLines(inpFile);
            StringBuilder sb = new StringBuilder();
            ProIIEqDataDAL eqdatadal = new ProIIEqDataDAL();
            List<InpPosInfo> list = new List<InpPosInfo>();
            IProIIReader r = ProIIFactory.CreateReader(SourceFileInfo.FileVersion);
            r.InitProIIReader(rpPrzFile);
            ProIIStreamData proiicoldvaporstream = r.GetSteamInfo(coldVaporStream);
            compressorH2Stream = ProIIToDefault.ConvertProIIStreamToCustomStream(proiicoldvaporstream);
            ProIIEqData proiiPCHPS = r.GetEqInfo("Flash", rl.ColdHighPressureSeparator);
            ProIIEqData proiiHHPS = r.GetEqInfo("Flash", rl.HotHighPressureSeparator);
            ProIIStreamData effluent = r.GetSteamInfo(rl.EffluentStream);
            CustomStream csEffluent = ProIIToDefault.ConvertProIIStreamToCustomStream(effluent);
            double PCHPS_N = double.Parse(proiiPCHPS.PressCalc);
            double PHHPS_N = double.Parse(proiiHHPS.PressCalc);
            PCHPS_N = UnitConvert.Convert("Kpa", "MPag", PCHPS_N);
            PHHPS_N = UnitConvert.Convert("Kpa", "MPag", PHHPS_N);
            r.ReleaseProIIReader();
            double DeltP_Accu = reliefPressure - PCHPS_N;
            double PHHPS_relief = PHHPS_N + DeltP_Accu;
            double PCHPS_relief = PCHPS_N + DeltP_Accu;
            double PEffluent_relief = csEffluent.Pressure + DeltP_Accu;

            SplashScreenManager.SentMsgToScreen("Simulation ......20%");
            if (!string.IsNullOrEmpty(rl.EffluentStream2))
            {
                InpPosInfo spi = PROIIFileOperator.GetStreamPosInfo(lines, rl.EffluentStream2, "PRESSURE(MPAG)=", "Pressure(MPag)="+ PEffluent_relief.ToString());
                list.Add(spi);
            }
            if (!string.IsNullOrEmpty(rl.EffluentStream))
            {
                InpPosInfo spi = PROIIFileOperator.GetStreamPosInfo(lines, rl.EffluentStream, "PRESSURE(MPAG)=", "Pressure(MPag)="+ PEffluent_relief.ToString());
                list.Add(spi);
            }
            InpPosInfo spi2 = PROIIFileOperator.GetFlashPosInfo(lines, rl.ColdHighPressureSeparator, "Pressure(MPag)=", PCHPS_relief.ToString());
            InpPosInfo spi3 = PROIIFileOperator.GetFlashPosInfo(lines, rl.HotHighPressureSeparator, "Pressure(MPag)=", PHHPS_relief.ToString());
            list.Add(spi2);
            list.Add(spi3);
            model.ReliefLoad = model.MaxGasRate - model.TotalPurgeRate;
            SplashScreenManager.SentMsgToScreen("Simulation ......50%");
            for (int i = 0; i < lines.Length; i++)
            {
                bool b = false;
                foreach (InpPosInfo spi in list)
                {
                    if (spi != null && i == spi.start)
                    {
                        sb.Append(spi.NewInfo);
                        i = spi.end;
                        b = true;
                        break;
                    }
                }
                if (!b)
                {
                    string line = lines[i];
                    sb.Append(line).Append("\r\n");
                }
            }
            SplashScreenManager.SentMsgToScreen("Simulation ......70%");
            //保存inpdata 到文件。
            string newInpDir = DirProtectedSystem + @"\BlocketOutlet";
            if (!Directory.Exists(newInpDir))
            {
                Directory.CreateDirectory(newInpDir);
            }
            newInpFile = newInpDir + @"\a.inp";
            File.Create(newInpFile).Close();
            File.WriteAllText(newInpFile, sb.ToString());
            //导入后，生成prz文件。
            IProIIImport import = ProIIFactory.CreateProIIImport(SourceFileInfo.FileVersion);
            int ImportResult = -1;
            int RunResult = -1;
            newPrzFile = import.ImportProIIINP(newInpFile, ref ImportResult, ref RunResult);
            if (ImportResult == 1 || ImportResult == 2)
            {
                if (RunResult == 1 || RunResult == 2)
                {
                    SplashScreenManager.SentMsgToScreen("Simulation ......80%");
                    IProIIReader reader = ProIIFactory.CreateReader(SourceFileInfo.FileVersion);
                    reader.InitProIIReader(newPrzFile);
                    ProIIStreamData proiiStream = reader.GetSteamInfo(coldVaporStream);
                    CustomStream cs = ProIIToDefault.ConvertProIIStreamToCustomStream(proiiStream);
                    model.ReliefMW = cs.BulkMwOfPhase;
                    model.ReliefTemperature = cs.Temperature;
                    model.ReliefPressure = reliefPressure;
                    model.ReliefCpCv = cs.BulkCPCVRatio;
                    model.ReliefZ = cs.VaporZFmKVal;
                    if (model.ReliefLoad < 0)
                        model.ReliefLoad = 0;
                    reader.ReleaseProIIReader();
                    model.IsSolved = true;
                    SimulationResult = "Simulation resolved";
                    SplashScreenManager.SentMsgToScreen("Simulation finished");
                    SplashScreenManager.Close();
                }
                else
                {
                    model.IsSolved = false;
                    SimulationResult = "Simulation not resolved";
                    SplashScreenManager.Close();
                    MessageBox.Show("Prz file is error!", "Message Box");
                    return;
                }
            }
            else
            {
                SplashScreenManager.Close();
                MessageBox.Show("inp file is error!", "Message Box");
                return;
            }
        }

        private void CalcLossOfReactorQuench()
        {
            SplashScreenManager.Show(10);
            SplashScreenManager.SentMsgToScreen("Simulation starting ......1%");
            string rpPrzFile = DirProtectedSystem + @"\myrp\myrp.prz";
            string rpInpDir = DirProtectedSystem + @"\myrp";
            PROIIFileOperator.DecompressProIIFile(rpPrzFile, rpInpDir);
            string inpFile = rpInpDir + @"\myrp.inp";
            string[] lines = System.IO.File.ReadAllLines(inpFile);
            SplashScreenManager.SentMsgToScreen("Simulation ......10%");
            StringBuilder sb = new StringBuilder();
            ProIIEqDataDAL eqdatadal = new ProIIEqDataDAL();
            List<InpPosInfo> list = new List<InpPosInfo>();
            IProIIReader r = ProIIFactory.CreateReader(SourceFileInfo.FileVersion);
            r.InitProIIReader(rpPrzFile);
            ProIIStreamData proiicoldvaporstream = r.GetSteamInfo(coldVaporStream);
            compressorH2Stream = ProIIToDefault.ConvertProIIStreamToCustomStream(proiicoldvaporstream);
            ProIIEqData proiiPCHPS = r.GetEqInfo("Flash", rl.ColdHighPressureSeparator);
            ProIIEqData proiiHHPS = r.GetEqInfo("Flash", rl.HotHighPressureSeparator);
            ProIIStreamData effluent = r.GetSteamInfo(rl.EffluentStream);
            CustomStream csEffluent = ProIIToDefault.ConvertProIIStreamToCustomStream(effluent);
            double PCHPS_N = double.Parse(proiiPCHPS.PressCalc);
            double PHHPS_N = double.Parse(proiiHHPS.PressCalc);
            PCHPS_N = UnitConvert.Convert("Kpa", "MPag", PCHPS_N);
            PHHPS_N = UnitConvert.Convert("Kpa", "MPag", PHHPS_N);
            r.ReleaseProIIReader();
            double DeltP_Accu = reliefPressure - PCHPS_N;
            double PHHPS_relief = PHHPS_N + DeltP_Accu;
            double PCHPS_relief = PCHPS_N + DeltP_Accu;
            double PEffluent_relief = csEffluent.Pressure + DeltP_Accu;
            SplashScreenManager.SentMsgToScreen("Simulation ......20%");
            if (!string.IsNullOrEmpty(rl.EffluentStream))
            {
                InpPosInfo spi = PROIIFileOperator.GetStreamPosInfo2(lines, rl.EffluentStream, "PRESSURE(MPAG)="+PEffluent_relief, "TEMPERATURE(C)="+ model.EffluentTemperature.ToString());
                list.Add(spi);
            }
            if (!string.IsNullOrEmpty(rl.EffluentStream2))
            {
                InpPosInfo spi = PROIIFileOperator.GetStreamPosInfo2(lines, rl.EffluentStream2, "PRESSURE(MPAG)="+PEffluent_relief, "TEMPERATURE(C)="+ model.EffluentTemperature2.ToString());
                list.Add(spi);
            }

            InpPosInfo spi2 = PROIIFileOperator.GetFlashPosInfo(lines, rl.ColdHighPressureSeparator, "Pressure(MPag)=", PCHPS_relief.ToString());
            InpPosInfo spi3 = PROIIFileOperator.GetFlashPosInfo(lines, rl.HotHighPressureSeparator, "Pressure(MPag)=", PHHPS_relief.ToString());
            list.Add(spi2);
            list.Add(spi3);
            SplashScreenManager.SentMsgToScreen("Simulation ......30%");
            for (int i = 0; i < lines.Length; i++)
            {
                bool b = false;
                foreach (InpPosInfo spi in list)
                {
                    if (spi != null && i == spi.start)
                    {
                        sb.Append(spi.NewInfo);
                        i = spi.end;
                        b = true;
                        break;
                    }
                }
                if (!b)
                {
                    string line = lines[i];
                    sb.Append(line).Append("\r\n");
                }
            }
            SplashScreenManager.SentMsgToScreen("Simulation ......40%");
            //保存inpdata 到文件。
            string newInpDir = DirProtectedSystem + @"\LossOfReactorQuench";
            if (!Directory.Exists(newInpDir))
            {
                Directory.CreateDirectory(newInpDir);
            }
            newInpFile = newInpDir + @"\a.inp";
            File.Create(newInpFile).Close();
            File.WriteAllText(newInpFile, sb.ToString());
            SplashScreenManager.SentMsgToScreen("Simulation ......50%");
            //导入后，生成prz文件。
            IProIIImport import = ProIIFactory.CreateProIIImport(SourceFileInfo.FileVersion);
            int ImportResult = -1;
            int RunResult = -1;
            newPrzFile = import.ImportProIIINP(newInpFile, ref ImportResult, ref RunResult);
            if (ImportResult == 1 || ImportResult == 2)
            {
                if (RunResult == 1 || RunResult == 2)
                {
                    SplashScreenManager.SentMsgToScreen("Simulation ......70%");
                    IProIIReader reader = ProIIFactory.CreateReader(SourceFileInfo.FileVersion);
                    reader.InitProIIReader(newPrzFile);
                    ProIIStreamData proiiStream = reader.GetSteamInfo(coldVaporStream);
                    reader.ReleaseProIIReader();
                    CustomStream cs = ProIIToDefault.ConvertProIIStreamToCustomStream(proiiStream);
                    model.ReliefLoad = 1.1*cs.BulkDensityAct*(cs.WeightFlow/cs.BulkDensityAct-compressorH2Stream.WeightFlow/compressorH2Stream.BulkDensityAct);
                    model.ReliefMW = cs.BulkMwOfPhase;
                    model.ReliefTemperature = cs.Temperature;
                    model.ReliefPressure = cs.Pressure;
                    model.ReliefCpCv = cs.BulkCPCVRatio;
                    model.ReliefZ = cs.VaporZFmKVal;
                    if (model.ReliefLoad < 0)
                        model.ReliefLoad = 0;
                    model.IsSolved = true;
                    SimulationResult = "Simulation resolved";
                    SplashScreenManager.SentMsgToScreen("Simulation finished");
                    SplashScreenManager.Close();
                }
                else
                {
                    model.IsSolved = false;
                    SimulationResult = "Simulation not resolved";
                    SplashScreenManager.Close();
                    MessageBox.Show("Prz file is error!", "Message Box");
                    return;
                }
            }
            else
            {
                SplashScreenManager.Close();
                MessageBox.Show("inp file is error!", "Message Box");
                return;
            }



        }

       
        private void Save(object obj)
        {
            if (obj != null)
            {
                System.Windows.Window wd = obj as System.Windows.Window;
                if (wd != null)
                {
                    WriteConvertModel();
                    reactorBLL.SaveBlockedOutlet(model.dbmodel);
                    wd.DialogResult = true;
                }
            }
        }
    }


    
}
