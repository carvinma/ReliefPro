using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using NHibernate;
using ReliefProCommon.CommonLib;
using ReliefProMain.Models.ReactorLoops;
using ReliefProModel.ReactorLoops;
using ReliefProDAL.ReactorLoops;
using UOMLib;
using ReliefProModel;
using System.IO;
using ProII;
using System.Windows;
using ReliefProDAL;
using ReliefProBLL;

namespace ReliefProMain.ViewModel.ReactorLoops
{
    public class GeneralFailureCommonVM : ViewModelBase
    {
        public ICommand OKCMD { get; set; }
        public ICommand CoolingRunCaseSimulationCMD { get; set; }
        public ICommand CoolingLaunchSimulatorCMD { get; set; }

        public ICommand ElectricRunCaseSimulationCMD { get; set; }
        public ICommand ElectricLaunchSimulatorCMD { get; set; }

        public ICommand LossOfColdFeedRunCaseSimulationCMD { get; set; }
        public ICommand LossOfColdFeedLaunchSimulatorCMD { get; set; }

        private ISession SessionPS;
        private ISession SessionPF;
        private SourceFile SourceFileInfo;
        private string DirPlant;
        private string DirProtectedSystem;
        private string coldVaporStream;
        private CustomStream compressorH2Stream;
        public GeneralFailureCommonModel model { get; set; }
        private GeneralFailureCommonBLL generalBLL; 
        private string rpPrzFile ;
        private string rpInpDir;
        private string caseDir;
        private string casePrzFile;
        private string caseInpFile;
        private int GeneralType;
        private double reliefPressure;
        double PHHPS_relief;
        double PCHPS_relief;
        double PEffluent_relief;
        string[] lines;
        List<InpPosInfo> list;
        private ReactorLoop rl;
        private void InitCMD()
        {
            OKCMD = new DelegateCommand<object>(Save);
            CoolingRunCaseSimulationCMD = new DelegateCommand<object>(CoolingRunCaseSimulation);
            CoolingLaunchSimulatorCMD = new DelegateCommand<object>(CoolingLaunchSimulator);

            ElectricRunCaseSimulationCMD = new DelegateCommand<object>(ElectricRunCaseSimulation);
            ElectricLaunchSimulatorCMD = new DelegateCommand<object>(ElectricLaunchSimulator);

            LossOfColdFeedRunCaseSimulationCMD = new DelegateCommand<object>(LossOfColdFeedRunCaseSimulation);
            LossOfColdFeedLaunchSimulatorCMD = new DelegateCommand<object>(LossOfColdFeedLaunchSimulator);
        }
        private void InitPage()
        {
            UOMLib.UOMEnum uomEnum = UOMSingle.UomEnums.FirstOrDefault(p => p.SessionPlant == this.SessionPF);
            model.ReliefLoadUnit = uomEnum.UserMassRate;
            model.ReliefPressureUnit = uomEnum.UserPressure;
            model.ReliefTemperatureUnit = uomEnum.UserTemperature;
        }

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
        /// vm构造函数
        /// </summary>
        /// <param name="ScenarioID"></param>
        /// <param name="SessionPS"></param>
        /// <param name="SessionPF"></param>
        /// <param name="GeneralType">0-GeneralCoolingWaterFailure,1-GeneralElectricPowerFailure</param>
        public GeneralFailureCommonVM(int ScenarioID, SourceFile SourceFileInfo, ISession SessionPS, ISession SessionPF, string DirPlant, string DirProtectedSystem, int GeneralType)
        {
            model = new GeneralFailureCommonModel();
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;
            this.SourceFileInfo = SourceFileInfo;
            this.DirPlant = DirPlant;
            this.DirProtectedSystem = DirProtectedSystem;
            InitCMD();
            InitPage();
            PSVBLL psvbll=new PSVBLL(SessionPS);
            reliefPressure = psvbll.GetReliefPressure();
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

            ProIIStreamData streamData = streamDataDAL.GetModel(SessionPF, rl.CompressorH2Stream, SourceFileInfo.FileName);
            compressorH2Stream = ProIIToDefault.ConvertProIIStreamToCustomStream(streamData);
            this.GeneralType = GeneralType;
            generalBLL = new GeneralFailureCommonBLL(SessionPS, SessionPF);
            GeneralFailureCommon commonModel = new GeneralFailureCommon();
            if (GeneralType == 0)
            {
                commonModel = generalBLL.GetGeneralCoolingWaterFailureModel(ScenarioID);
                caseDir = DirProtectedSystem + @"\CoolingWaterFailure";
                casePrzFile = DirProtectedSystem + @"\CoolingWaterFailure\a.prz";
                caseInpFile = DirProtectedSystem + @"\CoolingWaterFailure\a.inp";
                
            }
            else if (GeneralType == 1)
            {
                commonModel = generalBLL.GetGeneralElectricPowerFailureModel(ScenarioID);
                caseDir = DirProtectedSystem + @"\GeneralElectricPowerFailure";
                casePrzFile = DirProtectedSystem + @"\GeneralElectricPowerFailure\a.prz";
                caseInpFile = DirProtectedSystem + @"\GeneralElectricPowerFailure\a.inp";
            }
            else if (GeneralType == 2)
            {
                commonModel = generalBLL.GetLossofLiquidFeedModel(ScenarioID);
                caseDir = DirProtectedSystem + @"\LossOfColdFeed";
                casePrzFile = DirProtectedSystem + @"\LossOfColdFeed\a.prz";
                caseInpFile = DirProtectedSystem + @"\LossOfColdFeed\a.inp";
            }
            commonModel.ScenarioID = ScenarioID;
            commonModel.GeneralType = GeneralType;
            model.dbmodel = generalBLL.ReadConvert(commonModel);
            if (commonModel.ID > 0)
            {
                model.lstUtilityHX = new List<GeneralFailureHXModel>();
                IList<GeneralFailureCommonDetail> lstCommonDetail = generalBLL.GetGeneralFailureCommonDetail(commonModel.ID,1);
                model.lstUtilityHX = lstCommonDetail.Select(p => new GeneralFailureHXModel { HXName = p.HXName, Stop = p.Stop, DutyFactor = p.DutyFactor, ReactorType = p.ReactorType }).ToList();

                model.lstProcessHX = new List<GeneralFailureHXModel>();
                IList<GeneralFailureCommonDetail> lstCommonDetail2 = generalBLL.GetGeneralFailureCommonDetail(commonModel.ID, 0);
                model.lstProcessHX = lstCommonDetail2.Select(p => new GeneralFailureHXModel { HXName = p.HXName, Stop = p.Stop, DutyFactor = p.DutyFactor, ReactorType = p.ReactorType }).ToList();

                model.lstNetworkHX = new List<GeneralFailureHXModel>();
                IList<GeneralFailureCommonDetail> lstCommonDetail3 = generalBLL.GetGeneralFailureCommonDetail(commonModel.ID,3);
                model.lstNetworkHX = lstCommonDetail3.Select(p => new GeneralFailureHXModel { HXName = p.HXName, Stop = p.Stop, DutyFactor = p.DutyFactor, ReactorType = p.ReactorType }).ToList();
                if (model.IsSolved)
                {
                    SimulationResult = "Simulation resolved.";
                }
                else
                {
                    SimulationResult = "Simulation not resolved.";
                }
            }
            else
            {
                model.lstUtilityHX = GetInitHXs(1);
                model.lstProcessHX = GetInitHXs(0);
                model.lstNetworkHX = GetInitHXs(3);
                SimulationResult = string.Empty;
            }

            rpPrzFile = DirProtectedSystem + @"\myrp\myrp.prz";
            rpInpDir = DirProtectedSystem + @"\myrp";
            PROIIFileOperator.DecompressProIIFile(rpPrzFile, rpInpDir);
            string rpInpFile = rpInpDir + @"\myrp.inp";
            lines = System.IO.File.ReadAllLines(rpInpFile);
        }
        
        private List<GeneralFailureHXModel> GetInitHXs(int reactorType)
        {
            List<GeneralFailureHXModel> list = new List<GeneralFailureHXModel>();
            ReactorLoopDAL dal = new ReliefProDAL.ReactorLoops.ReactorLoopDAL();
            IList<ReactorLoopDetail> details = dal.GetReactorLoopDetail(SessionPS, reactorType);
            foreach (ReactorLoopDetail d in details)
            {
                GeneralFailureHXModel m = new GeneralFailureHXModel();
                m.HXName = d.DetailInfo;
                m.Stop = false;
                m.DutyFactor = 0;
                m.ReactorType = reactorType;
                list.Add(m);
            }
            return list;
        }

        private void CoolingRunCaseSimulation(object obj)
        {
            Simulation();
        }
        private void CoolingLaunchSimulator(object obj)
        {
            ProIIHelper.Run(SourceFileInfo.FileVersion, casePrzFile);
        }
        
        private void ElectricRunCaseSimulation(object obj)
        {
            Simulation();
        }
        private void ElectricLaunchSimulator(object obj)
        {
            ProIIHelper.Run(SourceFileInfo.FileVersion, casePrzFile);
        }

        private void LossOfColdFeedRunCaseSimulation(object obj)
        {
            Simulation();
        }
        private void LossOfColdFeedLaunchSimulator(object obj)
        {
            LaunchSimulator();
        }

        private void WriteConvert()
        {
            model.dbmodel.ReliefLoad = UnitConvert.Convert(model.ReliefLoadUnit, UOMEnum.MassRate, model.ReliefLoad);
            model.dbmodel.ReliefPressure = UnitConvert.Convert(model.ReliefPressureUnit, UOMEnum.Pressure, model.ReliefPressure);
            model.dbmodel.ReliefTemperature = UnitConvert.Convert(model.ReliefTemperatureUnit, UOMEnum.Temperature, model.ReliefTemperature);
        }

        private void Save(object obj)
        {
            if (obj != null)
            {
                System.Windows.Window wd = obj as System.Windows.Window;
                if (wd != null)
                {
                    WriteConvert();
                    var lstCommonDetail = model.lstUtilityHX.Select(p => new GeneralFailureCommonDetail
                    {
                        ID = 0,
                        HXName = p.HXName,
                        Stop = p.Stop,
                        DutyFactor = p.DutyFactor,
                        ReactorType=p.ReactorType
                    }).ToList();
                    var lstCommonDetail2 = model.lstProcessHX.Select(p => new GeneralFailureCommonDetail
                    {
                        ID = 0,
                        HXName = p.HXName,
                        Stop = p.Stop,
                        DutyFactor = p.DutyFactor,
                        ReactorType = p.ReactorType
                    }).ToList();
                    var lstCommonDetail3 = model.lstNetworkHX.Select(p => new GeneralFailureCommonDetail
                    {
                        ID = 0,
                        HXName = p.HXName,
                        Stop = p.Stop,
                        DutyFactor = p.DutyFactor,
                        ReactorType = p.ReactorType
                    }).ToList();
                    
                    lstCommonDetail = lstCommonDetail.Union(lstCommonDetail2).ToList();
                    lstCommonDetail = lstCommonDetail.Union(lstCommonDetail3).ToList();
                    
                    generalBLL.Save(model.dbmodel, lstCommonDetail);
                    wd.DialogResult = true;
                }
            }
        }
        
        private void GetHXDutyInfo(string[] lines,ref List<InpPosInfo> list)
        {
            ProIIEqDataDAL eqdatadal = new ProIIEqDataDAL();
            foreach (GeneralFailureHXModel m in model.lstProcessHX)
            {
                double duty = 1e-6;
                if (m.Stop)
                {
                    ProIIEqData eqdata = eqdatadal.GetModel(SessionPF, SourceFileInfo.FileName, m.HXName.ToUpper());
                    duty = double.Parse(eqdata.DutyCalc) * m.DutyFactor;//不需要转换单位了。因为它将用于proii文件中
                    if (duty == 0)
                        duty = 1e-6;
                    InpPosInfo spi = PROIIFileOperator.GetHxPosInfo(lines, m.HXName, "Duty(kw)=", (duty * 1e-6).ToString());
                    list.Add(spi);
                }
            }
            foreach (GeneralFailureHXModel m in model.lstNetworkHX)
            {
                double duty = 1e-6;
                if (m.Stop)
                {
                    ProIIEqData eqdata = eqdatadal.GetModel(SessionPF, SourceFileInfo.FileName, m.HXName.ToUpper());
                    duty = double.Parse(eqdata.DutyCalc) * m.DutyFactor;//不需要转换单位了。因为它将用于proii文件中
                    if (duty == 0)
                        duty = 1e-6;
                    InpPosInfo spi = PROIIFileOperator.GetHxPosInfo(lines, m.HXName, "Duty(kw)=", (duty * 1e-6).ToString());
                    list.Add(spi);
                }
            }
            foreach (GeneralFailureHXModel m in model.lstUtilityHX)
            {
                double duty = 1e-6;
                if (m.Stop)
                {
                    ProIIEqData eqdata = eqdatadal.GetModel(SessionPF, SourceFileInfo.FileName, m.HXName.ToUpper());
                    duty = double.Parse(eqdata.DutyCalc) * m.DutyFactor;//不需要转换单位了。因为它将用于proii文件中
                    if (duty == 0)
                        duty = 1e-6;
                    InpPosInfo spi = PROIIFileOperator.GetHxPosInfo(lines, m.HXName, "Duty(kw)=", (duty * 1e-6).ToString());
                    list.Add(spi);
                }
            }            
        }

        /// <summary>
        /// 初始化模拟条件
        /// </summary>
        private void InitSimulation()
        {                        
            ProIIEqDataDAL eqdatadal = new ProIIEqDataDAL();
            list = new List<InpPosInfo>();
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
            PHHPS_relief = PHHPS_N + DeltP_Accu;
            PCHPS_relief = PCHPS_N + DeltP_Accu;
            PEffluent_relief = csEffluent.Pressure + DeltP_Accu;
        }

        /// <summary>
        /// 开始模拟
        /// </summary>
        private string Simulation(ref int ImportResult, ref int RunResult)
        {
            SplashScreenManager.Show(6);
            SplashScreenManager.SentMsgToScreen("Simulation Init...... 1%");
            StringBuilder sb = new StringBuilder();
            if (!Directory.Exists(caseDir))
            {
                Directory.CreateDirectory(caseDir);
            }
            InitSimulation();
            SplashScreenManager.SentMsgToScreen("Simulation ...... 10%");

            if (GeneralType == 0)
            {
                PrepareCoolingWaterFailure();
            }
            else if (GeneralType == 1)
            {
                PrepareGeneralElectricalFailure();
            }
            else
            {
                PrepareLossOfColdFeed();
            }
            SplashScreenManager.SentMsgToScreen("Simulation ...... 30%");
            
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


            File.Create(caseInpFile).Close();
            File.WriteAllText(caseInpFile, sb.ToString());
            //导入后，生成prz文件。
            IProIIImport import = ProIIFactory.CreateProIIImport(SourceFileInfo.FileVersion);
            casePrzFile = import.ImportProIIINP(caseInpFile, ref ImportResult, ref RunResult);
            SplashScreenManager.SentMsgToScreen("Simulation ...... 50%");
            return casePrzFile;
            
        }

        private void LaunchSimulator()
        {
            if (File.Exists(casePrzFile))
            {
                ProIIHelper.Run(SourceFileInfo.FileVersion, casePrzFile);
            }
            else
            {
                if (File.Exists(caseInpFile))
                {
                    if (MessageBox.Show("Dou you want to open keyword file?", "Message Box", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        ProIIHelper.Open(caseInpFile);
                    }
                }
                else
                {
                    MessageBox.Show("Please run simulation first.", "Message Box");
                }
            }
        }

        /// <summary>
        /// 根据GenralElec 工况对应的各个source的情况，来设置rate
        /// </summary>
        private void PrepareGeneralElectricalFailure()
        {
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

            SourceDAL srDal = new SourceDAL();
            SystemScenarioFactorDAL factorDal = new SystemScenarioFactorDAL();
            Source sr = srDal.GetModel(rl.CompressorH2Stream, SessionPS);
            SystemScenarioFactor ssfactor = factorDal.GetSystemScenarioFactor(SessionPF, "4", sr.SourceType);
            if (ssfactor.GeneralElectricPowerFailure == "0")
            {
                InpPosInfo spi = PROIIFileOperator.GetStreamPosInfo(lines, rl.CompressorH2Stream, "RATE(WT)=", "RATE(WT)=1e-6");
                list.Add(spi);
            }

            sr = srDal.GetModel(rl.ColdReactorFeedStream, SessionPS);
            ssfactor = factorDal.GetSystemScenarioFactor(SessionPF, "4", sr.SourceType);
            if (ssfactor.GeneralElectricPowerFailure == "0")
            {
                InpPosInfo spi = PROIIFileOperator.GetStreamPosInfo(lines, rl.ColdReactorFeedStream, "RATE(WT)=", "RATE(WT)=1e-6");
                list.Add(spi);
            }

            if (!string.IsNullOrEmpty(rl.ColdReactorFeedStream2))
            {
                sr = srDal.GetModel(rl.ColdReactorFeedStream2, SessionPS);
                ssfactor = factorDal.GetSystemScenarioFactor(SessionPF, "4", sr.SourceType);
                if (ssfactor.GeneralElectricPowerFailure == "0")
                {
                    InpPosInfo spi = PROIIFileOperator.GetStreamPosInfo(lines, rl.ColdReactorFeedStream2, "RATE(WT)=", "RATE(WT)=1e-6");
                    list.Add(spi);
                }
            }
            sr = srDal.GetModel(rl.InjectionWaterStream, SessionPS);
            ssfactor = factorDal.GetSystemScenarioFactor(SessionPF, "4", sr.SourceType);
            if (ssfactor.GeneralElectricPowerFailure == "0")
            {
                InpPosInfo spi = PROIIFileOperator.GetStreamPosInfo(lines, rl.InjectionWaterStream, "RATE(WT)=", "RATE(WT)=1e-6");
                list.Add(spi);
            }
            InpPosInfo spi2 = PROIIFileOperator.GetFlashPosInfo(lines, rl.ColdHighPressureSeparator, "Pressure(MPag)=", PCHPS_relief.ToString());
            InpPosInfo spi3 = PROIIFileOperator.GetFlashPosInfo(lines, rl.HotHighPressureSeparator, "Pressure(MPag)=", PHHPS_relief.ToString());
            list.Add(spi2);
            list.Add(spi3);
            GetHXDutyInfo(lines, ref list);
        }

        /// <summary>
        /// 根据coolingWater 工况对应的各个source的情况，来设置rate,同时考虑外来的2个条件。
        /// </summary>
        private void PrepareCoolingWaterFailure()
        {
            SourceDAL srDal = new SourceDAL();
            SystemScenarioFactorDAL factorDal = new SystemScenarioFactorDAL();
            if (model.RecycleCompressorFailure)
            {
                InpPosInfo spi = PROIIFileOperator.GetStreamPosInfo(lines, rl.CompressorH2Stream, "RATE(WT)=", "RATE(WT)=1e-6");
                list.Add(spi);
            }
            else
            {
                Source sr = srDal.GetModel(rl.CompressorH2Stream, SessionPS);
                SystemScenarioFactor ssfactor = factorDal.GetSystemScenarioFactor(SessionPF, "4", sr.SourceType);
                if (ssfactor.CoolingWaterFailure == "0")
                {
                    InpPosInfo spi = PROIIFileOperator.GetStreamPosInfo(lines, rl.CompressorH2Stream, "RATE(WT)=", "RATE(WT)=1e-6");
                    list.Add(spi);
                }
            }

            if (model.CalcInjectionWaterStream)
            {
                InpPosInfo spi = PROIIFileOperator.GetStreamPosInfo(lines, rl.InjectionWaterStream, "RATE(WT)=", "RATE(WT)=1e-6");
                list.Add(spi);
            }
            else
            {
                Source sr = srDal.GetModel(rl.InjectionWaterStream, SessionPS);
                SystemScenarioFactor ssfactor = factorDal.GetSystemScenarioFactor(SessionPF, "4", sr.SourceType);
                if (ssfactor.CoolingWaterFailure == "0")
                {
                    InpPosInfo spi = PROIIFileOperator.GetStreamPosInfo(lines, rl.InjectionWaterStream, "RATE(WT)=", "RATE(WT)=1e-6");
                    list.Add(spi);
                }
            }

            Source srCold = srDal.GetModel(rl.ColdReactorFeedStream, SessionPS);
            SystemScenarioFactor ssColdfactor = factorDal.GetSystemScenarioFactor(SessionPF, "4", srCold.SourceType);
            if (ssColdfactor.CoolingWaterFailure == "0")
            {
                InpPosInfo spi = PROIIFileOperator.GetStreamPosInfo(lines, rl.ColdReactorFeedStream, "RATE(WT)=", "RATE(WT)=1e-6");
                list.Add(spi);
            }

            if (!string.IsNullOrEmpty(rl.ColdReactorFeedStream2))
            {
                srCold = srDal.GetModel(rl.ColdReactorFeedStream2, SessionPS);
                ssColdfactor = factorDal.GetSystemScenarioFactor(SessionPF, "4", srCold.SourceType);
                if (ssColdfactor.CoolingWaterFailure == "0")
                {
                    InpPosInfo spi = PROIIFileOperator.GetStreamPosInfo(lines, rl.ColdReactorFeedStream2, "RATE(WT)=", "RATE(WT)=1e-6");
                    list.Add(spi);
                }
            }
            InpPosInfo spi2 = PROIIFileOperator.GetFlashPosInfo(lines, rl.ColdHighPressureSeparator, "Pressure(MPag)=", PCHPS_relief.ToString());
            InpPosInfo spi3 = PROIIFileOperator.GetFlashPosInfo(lines, rl.HotHighPressureSeparator, "Pressure(MPag)=", PHHPS_relief.ToString());
            list.Add(spi2);
            list.Add(spi3);
            GetHXDutyInfo(lines, ref list);

        }

        /// <summary>
        /// LossOfColdFeed 就是停掉ColdReactorFeedStream,ColdReactorFeedStream2
        /// </summary>
        private void PrepareLossOfColdFeed()
        {
            if (!string.IsNullOrEmpty(rl.EffluentStream2))
            {
                InpPosInfo spi = PROIIFileOperator.GetStreamPosInfo(lines, rl.EffluentStream2, "PRESSURE(MPAG)=", "Pressure(MPag)="+ PEffluent_relief.ToString());
                list.Add(spi);
            }
            if (!string.IsNullOrEmpty(rl.EffluentStream))
            {
                InpPosInfo spi = PROIIFileOperator.GetStreamPosInfo(lines, rl.EffluentStream, "PRESSURE(MPAG)=", "Pressure(MPag)="+PEffluent_relief.ToString());
                list.Add(spi);
            }

            if (!string.IsNullOrEmpty(rl.ColdReactorFeedStream))
            {
                InpPosInfo spi = PROIIFileOperator.GetStreamPosInfo(lines, rl.ColdReactorFeedStream, "RATE(WT)=", "RATE(WT)=1e-6");
                list.Add(spi);
            }

            if (!string.IsNullOrEmpty(rl.ColdReactorFeedStream2))
            {
                InpPosInfo spi = PROIIFileOperator.GetStreamPosInfo(lines, rl.ColdReactorFeedStream2, "RATE(WT)=", "RATE(WT)=1e-6");
                list.Add(spi);
            }

            InpPosInfo spi2 = PROIIFileOperator.GetFlashPosInfo(lines, rl.ColdHighPressureSeparator, "Pressure(MPag)=", PCHPS_relief.ToString());
            InpPosInfo spi3 = PROIIFileOperator.GetFlashPosInfo(lines, rl.HotHighPressureSeparator, "Pressure(MPag)=", PHHPS_relief.ToString());
            list.Add(spi2);
            list.Add(spi3);            
            GetHXDutyInfo(lines, ref list);
        }

        private void Simulation()
        {
            int ImportResult = 1;
            int RunResult = 1;
            if (File.Exists(casePrzFile))
            {
                if (MessageBox.Show("Do you want to rewrite?", "Message Box", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    //覆盖
                    Simulation(ref ImportResult, ref RunResult);
                }
                else
                {
                    return;
                }
            }
            else
            {
                Simulation(ref ImportResult, ref RunResult);
            }

            if (ImportResult == 1 || ImportResult == 2)
            {
                if (RunResult == 1 || RunResult == 2)
                {
                    IProIIReader reader = ProIIFactory.CreateReader(SourceFileInfo.FileVersion);
                    reader.InitProIIReader(casePrzFile);
                    ProIIStreamData proiiStream = reader.GetSteamInfo(coldVaporStream);
                    CustomStream cs = ProIIToDefault.ConvertProIIStreamToCustomStream(proiiStream);

                    SplashScreenManager.SentMsgToScreen("Simulation reading ......70%");
                    double v1 = cs.WeightFlow / cs.BulkDensityAct;
                    double v2 = compressorH2Stream.WeightFlow / compressorH2Stream.BulkDensityAct;
                    model.ReliefLoad = 1.1 * cs.BulkDensityAct * (v1 - v2);
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
    }
}
