using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using NHibernate;
using ReliefProCommon.CommonLib;
using ReliefProLL;
using ReliefProMain.Models.ReactorLoops;
using ReliefProModel.ReactorLoops;
using ReliefProDAL.ReactorLoops;
using UOMLib;
using ReliefProModel;
using System.IO;
using ProII;
using System.Windows;
using ReliefProDAL;

namespace ReliefProMain.ViewModel.ReactorLoops
{
    public class GeneralFailureCommonVM : ViewModelBase
    {
        public ICommand OKCMD { get; set; }
        public ICommand CoolingRunCaseSimulationCMD { get; set; }
        public ICommand CoolingLaunchSimulatorCMD { get; set; }

        public ICommand ElectricRunCaseSimulationCMD { get; set; }
        public ICommand ElectricLaunchSimulatorCMD { get; set; }

        private ISession SessionPS;
        private ISession SessionPF;
        private SourceFile SourceFileInfo;
        private string DirPlant;
        private string DirProtectedSystem;

        public GeneralFailureCommonModel model { get; set; }
        private GeneralFailureCommonBLL generalBLL;
        private void InitCMD()
        {
            OKCMD = new DelegateCommand<object>(Save);
            CoolingRunCaseSimulationCMD = new DelegateCommand<object>(CoolingRunCaseSimulation);
            CoolingLaunchSimulatorCMD = new DelegateCommand<object>(CoolingLaunchSimulator);

            ElectricRunCaseSimulationCMD = new DelegateCommand<object>(ElectricRunCaseSimulation);
            ElectricLaunchSimulatorCMD = new DelegateCommand<object>(ElectricLaunchSimulator);
        }
        private void InitPage()
        {
            UOMLib.UOMEnum uomEnum = UOMSingle.UomEnums.FirstOrDefault(p => p.SessionPlant == this.SessionPF);
            model.ReliefLoadUnit = uomEnum.UserMassRate;
            model.ReliefPressureUnit = uomEnum.UserPressure;
            model.ReliefTemperatureUnit = uomEnum.UserTemperature;
        }
        /// <summary>
        /// 
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

            generalBLL = new GeneralFailureCommonBLL(SessionPS, SessionPF);
            GeneralFailureCommon commonModel = new GeneralFailureCommon();
            if (GeneralType == 0)
                commonModel = generalBLL.GetGeneralCoolingWaterFailureModel(ScenarioID);
            else if (GeneralType == 1)
                commonModel = generalBLL.GetGeneralElectricPowerFailureModel(ScenarioID);

            model.dbmodel = generalBLL.ReadConvert(commonModel);
            if (commonModel.ID > 0)
            {
                model.lstUtilityHX = new List<GeneralFailureHXModel>();
                IList<GeneralFailureCommonDetail> lstCommonDetail = generalBLL.GetGeneralFailureCommonDetail(commonModel.ID,1);
                model.lstUtilityHX = lstCommonDetail.Select(p => new GeneralFailureHXModel { HXName = p.HXName, Stop = p.Stop, DutyFactor = p.DutyFactor, ReactorType = p.ReactorType }).ToList();

                model.lstNetworkHX = new List<GeneralFailureHXModel>();
                IList<GeneralFailureCommonDetail> lstCommonDetail2 = generalBLL.GetGeneralFailureCommonDetail(commonModel.ID,3);
                model.lstNetworkHX = lstCommonDetail2.Select(p => new GeneralFailureHXModel { HXName = p.HXName, Stop = p.Stop, DutyFactor = p.DutyFactor, ReactorType = p.ReactorType }).ToList();

            }
            else
            {
                model.lstUtilityHX = GetInitHXs(1);
                model.lstNetworkHX = GetInitHXs(3);
            }
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
            string rpPrzFile = DirProtectedSystem + @"\myrp\myrp.prz";
            string rpInpDir = DirProtectedSystem + @"\myrp";
            string caseDir = DirProtectedSystem + @"\GeneralElectricPowerFailure";
            PROIIFileOperator.DecompressProIIFile(rpPrzFile, rpInpDir);
            string inpFile = rpInpDir + @"\myrp.inp";
            string[] lines = System.IO.File.ReadAllLines(inpFile);
            StringBuilder sb = new StringBuilder();
            ProIIEqDataDAL eqdatadal = new ProIIEqDataDAL();
            List<InpPosInfo> list = new List<InpPosInfo>();

            ReactorLoopDAL rldal = new ReactorLoopDAL();
            ReactorLoop rl = rldal.GetModel(SessionPS);
            InpPosInfo spi1 = GetStreamPosInfo(lines, rl.CompressorH2Stream, "TEMPERATURE=", "1e-8");
            InpPosInfo spi2 = GetStreamPosInfo(lines, rl.InjectionWaterStream, "TEMPERATURE=", "1e-8");

            foreach (GeneralFailureHXModel m in model.lstUtilityHX)
            {
                if (m.Stop)
                {
                    ProIIEqData eqdata = eqdatadal.GetModel(SessionPF, SourceFileInfo.FileName, m.HXName.ToUpper());
                    double duty = double.Parse(eqdata.DutyCalc) * m.DutyFactor;//不需要转换单位了。因为它将用于proii文件中
                    InpPosInfo spi = GetHxPosInfo(lines, m.HXName, "Duty=", duty.ToString());
                    list.Add(spi);
                }
            }
            if (model.RecycleCompressorFailure)
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    if (spi1 != null && i == spi1.start)
                    {
                        sb.Append(spi1.NewInfo);
                        i = spi1.end;
                    }
                    else if (spi2 != null && i == spi2.start)
                    {
                        sb.Append(spi2.NewInfo);
                        i = spi2.end;
                    }
                    else
                    {
                        string line = lines[i];
                        sb.Append(line).Append("\r\n");
                    }
                }
            }
            else
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    bool b = false;
                    if (spi2 != null && i == spi2.start)
                    {
                        sb.Append(spi2.NewInfo);
                        i = spi2.end;
                        b = true;
                    }
                    
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
            }

            //保存inpdata 到文件。
            string newInpDir = DirProtectedSystem + @"\GeneralElectricPowerFailure";
            if (!Directory.Exists(newInpDir))
            {
                Directory.CreateDirectory(newInpDir);
            }
            string newInpFile = newInpDir + @"\a.inp";
            File.Create(newInpFile).Close();
            File.WriteAllText(newInpFile, sb.ToString());
            //导入后，生成prz文件。
            IProIIImport import = ProIIFactory.CreateProIIImport(SourceFileInfo.FileVersion);
            int ImportResult = -1;
            int RunResult = -1;
            string newPrzFile = import.ImportProIIINP(newInpFile, out ImportResult, out RunResult);
            if (ImportResult == 1 || ImportResult == 2)
            {
                if (RunResult == 1 || RunResult == 2)
                {                    
                    IProIIReader reader = ProIIFactory.CreateReader(SourceFileInfo.FileVersion);
                    reader.InitProIIReader(newPrzFile);
                    if (!string.IsNullOrEmpty(rl.ColdReactorFeedStream))
                    {
                        ProIIStreamData proiiStream = reader.GetSteamInfo(rl.ColdReactorFeedStream);
                        CustomStream cs = ProIIToDefault.ConvertProIIStreamToCustomStream(proiiStream);
                        model.ReliefLoad = cs.WeightFlow;
                        model.ReliefMW = cs.BulkMwOfPhase;
                        model.ReliefTemperature = cs.Temperature;
                        model.ReliefPressure = cs.Pressure;
                        model.ReliefCpCv = cs.BulkCPCVRatio;
                        model.ReliefZ = cs.VaporZFmKVal;
                    }
                    else if (!string.IsNullOrEmpty(rl.ColdReactorFeedStream2))
                    {
                        ProIIStreamData proiiStream = reader.GetSteamInfo(rl.ColdReactorFeedStream2);
                        CustomStream cs = ProIIToDefault.ConvertProIIStreamToCustomStream(proiiStream);
                        model.ReliefLoad = cs.WeightFlow;
                        model.ReliefMW = cs.BulkMwOfPhase;
                        model.ReliefTemperature = cs.Temperature;
                        model.ReliefPressure = cs.Pressure;
                        model.ReliefCpCv = cs.BulkCPCVRatio;
                        model.ReliefZ = cs.VaporZFmKVal;
                    }
                    reader.ReleaseProIIReader();
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
        private void CoolingLaunchSimulator(object obj)
        { 
        }
        private void ElectricRunCaseSimulation(object obj)
        {

            string rpPrzFile = DirProtectedSystem + @"\myrp\myrp.prz";
            string rpInpDir = DirProtectedSystem + @"\myrp";
            string caseDir = DirProtectedSystem + @"\GeneralElectricPowerFailure";
            PROIIFileOperator.DecompressProIIFile(rpPrzFile, rpInpDir);
            string inpFile = rpInpDir + @"\myrp.inp";
            string[] lines = System.IO.File.ReadAllLines(inpFile);
            StringBuilder sb = new StringBuilder();
            ProIIEqDataDAL eqdatadal = new ProIIEqDataDAL();
            List<InpPosInfo> list = new List<InpPosInfo>();
            foreach (GeneralFailureHXModel m in model.lstNetworkHX)
            {
                if (m.Stop)
                {
                    ProIIEqData eqdata = eqdatadal.GetModel(SessionPF, SourceFileInfo.FileName, m.HXName.ToUpper());
                    double duty = double.Parse(eqdata.DutyCalc) * m.DutyFactor;//不需要转换单位了。因为它将用于proii文件中
                    InpPosInfo spi = GetHxPosInfo(lines, m.HXName, "Duty=", duty.ToString());
                    list.Add(spi);
                }
            }
            foreach (GeneralFailureHXModel m in model.lstUtilityHX)
            {
                if (m.Stop)
                {
                    ProIIEqData eqdata = eqdatadal.GetModel(SessionPF, SourceFileInfo.FileName, m.HXName.ToUpper());
                    double duty = double.Parse(eqdata.DutyCalc) * m.DutyFactor;//不需要转换单位了。因为它将用于proii文件中
                    InpPosInfo spi = GetHxPosInfo(lines, m.HXName, "Duty=", duty.ToString());
                    list.Add(spi);
                }
            }
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

            //保存inpdata 到文件。
            string newInpDir = DirProtectedSystem + @"\GeneralElectricPowerFailure";
            if (!Directory.Exists(newInpDir))
            {
                Directory.CreateDirectory(newInpDir);
            }
            string newInpFile = newInpDir + @"\a.inp";
            File.Create(newInpFile).Close();
            File.WriteAllText(newInpFile, sb.ToString());
            //导入后，生成prz文件。
            IProIIImport import = ProIIFactory.CreateProIIImport(SourceFileInfo.FileVersion);
            int ImportResult = -1;
            int RunResult = -1;
            string newPrzFile = import.ImportProIIINP(newInpFile, out ImportResult, out RunResult);
            if (ImportResult == 1 || ImportResult == 2)
            {
                if (RunResult == 1 || RunResult == 2)
                {
                    ReactorLoopDAL rldal = new ReactorLoopDAL();
                    ReactorLoop rl = rldal.GetModel(SessionPS);

                    IProIIReader reader = ProIIFactory.CreateReader(SourceFileInfo.FileVersion);
                    reader.InitProIIReader(newPrzFile);
                    if (!string.IsNullOrEmpty(rl.ColdReactorFeedStream))
                    {
                        ProIIStreamData proiiStream = reader.GetSteamInfo(rl.ColdReactorFeedStream);
                        CustomStream cs = ProIIToDefault.ConvertProIIStreamToCustomStream(proiiStream);
                        model.ReliefLoad = cs.WeightFlow;
                        model.ReliefMW = cs.BulkMwOfPhase;
                        model.ReliefTemperature = cs.Temperature;
                        model.ReliefPressure = cs.Pressure;
                        model.ReliefCpCv = cs.BulkCPCVRatio;
                        model.ReliefZ = cs.VaporZFmKVal;
                    }
                    else if (!string.IsNullOrEmpty(rl.ColdReactorFeedStream2))
                    {
                        ProIIStreamData proiiStream = reader.GetSteamInfo(rl.ColdReactorFeedStream2);
                        CustomStream cs = ProIIToDefault.ConvertProIIStreamToCustomStream(proiiStream);
                        model.ReliefLoad = cs.WeightFlow;
                        model.ReliefMW = cs.BulkMwOfPhase;
                        model.ReliefTemperature = cs.Temperature;
                        model.ReliefPressure = cs.Pressure;
                        model.ReliefCpCv = cs.BulkCPCVRatio;
                        model.ReliefZ = cs.VaporZFmKVal;
                    }
                    reader.ReleaseProIIReader();
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
        private void ElectricLaunchSimulator(object obj)
        { 
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
                    var lstCommonDetail2 = model.lstNetworkHX.Select(p => new GeneralFailureCommonDetail
                    {
                        ID = 0,
                        HXName = p.HXName,
                        Stop = p.Stop,
                        DutyFactor = p.DutyFactor,
                        ReactorType = p.ReactorType
                    }).ToList();

                    lstCommonDetail.Union(lstCommonDetail2);
                    generalBLL.Save(model.dbmodel, lstCommonDetail);
                    wd.DialogResult = true;
                }
            }
        }

        /// <summary>
        /// 获取新的hx的信息。
        /// </summary>
        /// <param name="hxName"></param>
        private string GetNewHXInfo(string hxName)
        {
            string hxInfo = string.Empty;

            return hxInfo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="hxName"></param>
        /// <param name="rewriteAttr">Duty</param>
        /// <param name="rewriteValue"></param>
        /// <param name="spi"></param>
        private InpPosInfo GetHxPosInfo(string[] lines, string hxName, string rewriteAttr, string rewriteValue)
        {
            if (string.IsNullOrEmpty(hxName))
            {
                return null;
            }
            StringBuilder sb = new StringBuilder();
            List<string> list = new List<string>();
            InpPosInfo spi = new InpPosInfo();
            for (int i = 2; i < lines.Length; i++)
            {
                string line = lines[i];
                string key = "HX   UID=" + hxName.ToUpper();
                if (line.Contains(key))
                {
                    spi.start = i;
                    break;
                }
            }

            bool b = false;
            string attrvalue = string.Empty;
            for (int i = spi.start; i < lines.Length; i++)
            {
                string line = lines[i];
                string key1 = "HX   UID=";
                string key2 = "END";
                 string key = "HX   UID=" + hxName.ToUpper();
                 if ((line.Contains(key1) || line.Contains(key2)) && !line.Contains(key))
                {
                    spi.end = i - 1;
                    if (!b)
                    {
                        attrvalue = "OPER " + rewriteAttr + rewriteValue;
                    }
                    break;
                }
                else if (line.Contains("OPER "))
                {
                    b = true;
                    attrvalue = "OPER " + rewriteAttr + rewriteValue;
                    list.Add(line);
                }
                else
                {
                    list.Add(line);
                }
            }


            for (int i = 0; i < list.Count; i++)
            {
                sb.Append(list[i]).Append("\r\n");
            }
            if (!b)
            {
                sb.Append(attrvalue).Append("\r\n");
            }
            spi.Name = hxName;
            spi.NewInfo = sb.ToString();
            return spi;
        }

        private InpPosInfo GetStreamPosInfo(string[] lines, string streamName, string rewriteAttr, string rewriteValue)
        {
            if (double.Parse(rewriteValue) == 0)
            {
                rewriteValue = "1e-8";
            }


            if (string.IsNullOrEmpty(streamName))
            {
                return null;
            }
            StringBuilder sb = new StringBuilder();
            List<string> list = new List<string>();
            InpPosInfo spi = new InpPosInfo();
            for (int i = 2; i < lines.Length; i++)
            {
                string line = lines[i];
                string key = "PROPERTY STREAM=" + streamName.ToUpper();
                if (line.Contains(key))
                {
                    spi.start = i;
                    break;
                }
            }

            bool b = false;
            string attrvalue = string.Empty;
            for (int i = spi.start; i < lines.Length; i++)
            {
                string line = lines[i];
                string key1 = "PROPERTY STREAM=";
                string key2 = "UNIT OPERATIONS";
                string key = "PROPERTY STREAM=" + streamName.ToUpper();
                if ((line.Contains(key1) || line.Contains(key2)) && !line.Contains(key))
                {
                    spi.end = i - 1;
                    if (!b)
                    {
                        attrvalue = rewriteAttr + rewriteValue;
                    }
                    break;
                }
                else if (line.Contains(rewriteAttr.ToUpper()))
                {
                    b = true;
                    string oldValue = string.Empty;
                    string newValue = rewriteAttr + rewriteValue;

                    int s = line.IndexOf(rewriteAttr);
                    string sub = line.Substring(s);
                    s = sub.IndexOf(",");
                    oldValue = sub.Substring(0, s);
                    string newLine = line.Replace(oldValue, newValue);
                    list.Add(newLine);
                }
                else
                {
                    list.Add(line);

                }
            }
            if (b)
            {
                sb.Append(list[0]).Append("\r\n");
            }
            else
            {
                sb.Append(list[0]).Append(@",");
                sb.Append(attrvalue).Append("\r\n");
            }
            for (int i = 1; i < list.Count; i++)
            {
                sb.Append(list[i]).Append("\r\n");
            }
            spi.Name = streamName;
            spi.NewInfo = sb.ToString();
            return spi;
        }

    }
}
