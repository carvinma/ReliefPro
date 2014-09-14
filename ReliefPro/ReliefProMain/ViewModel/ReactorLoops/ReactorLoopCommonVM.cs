using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using NHibernate;
using ReliefProLL;
using ReliefProMain.Models.ReactorLoops;
using UOMLib;
using ReliefProModel;
using ReliefProCommon.CommonLib;
using ReliefProModel.ReactorLoops;
using ReliefProDAL.ReactorLoops;
using System.IO;
using ProII;
using System.Windows;

namespace ReliefProMain.ViewModel.ReactorLoops
{
    public class ReactorLoopCommonVM
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
        /// <summary>
        /// 0-ReactorLoopBlockedOutlet,1-LossOfReactorQuench,2-LossOfColdFeed
        /// </summary>
        /// <param name="ScenarioID"></param>
        /// <param name="SessionPS"></param>
        /// <param name="SessionPF"></param>
        /// <param name="ReactorType"> 0-ReactorLoopBlockedOutlet,1-LossOfReactorQuench,2-LossOfColdFeed</param>
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

            reactorBLL = new ReactorLoopBLL(SessionPS, SessionPF);
            var blockModel = reactorBLL.GetBlockedOutletModel(ScenarioID, reactorType);
            blockModel = reactorBLL.ReadConvertBlockedOutletModel(blockModel);

            model = new ReactorLoopCommonModel(blockModel);
            model.dbmodel.ScenarioID = ScenarioID;

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
            model.dbmodel.MaxGasRate = UnitConvert.Convert(model.MaxGasRateUnit, UOMLib.UOMEnum.MassRate.ToString(), model.MaxGasRate);
            model.dbmodel.TotalPurgeRate = UnitConvert.Convert(model.TotalPurgeRateUnit, UOMLib.UOMEnum.MassRate.ToString(), model.TotalPurgeRate);
            model.dbmodel.ReliefMW = model.ReliefMW;
            model.dbmodel.ReliefLoad = UnitConvert.Convert(model.ReliefLoadUnit, UOMLib.UOMEnum.MassRate.ToString(), model.ReliefLoad);
            model.dbmodel.ReliefTemperature = UnitConvert.Convert(model.ReliefTemperatureUnit, UOMLib.UOMEnum.Temperature.ToString(), model.ReliefTemperature);
            model.dbmodel.ReliefCpCv = model.ReliefCpCv;
            model.dbmodel.ReliefZ = model.ReliefZ;
            model.dbmodel.ReactorType = reactorType;
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

        }
        private void CalcResult(object obj)
        {
            switch (reactorType)
            {
                case 0:
                    CalcBlocket();
                    break;
                case 1:
                    CalcLossOfReactorQuench();
                    break;
                case 2:
                    CalcLossOfColdFeed();
                    break;


            }

        }
        private void CalcBlocket()
        {
            model.ReliefLoad = model.TotalPurgeRate - model.MaxGasRate;

        }

        private void CalcLossOfReactorQuench()
        {
            string rpPrzFile = DirProtectedSystem + @"\myrp\myrp.prz";
            string rpInpDir = DirProtectedSystem + @"\myrp";
            string caseDir = DirProtectedSystem + @"\LossOfReactorQuench";
            PROIIFileOperator.DecompressProIIFile(rpPrzFile, rpInpDir);
            string inpFile = rpInpDir + @"\myrp_backup.inp";
            string[] lines = System.IO.File.ReadAllLines(inpFile);
            StringBuilder sb = new StringBuilder();
            ReactorLoopDAL rldal = new ReactorLoopDAL();
            ReactorLoop rl = rldal.GetModel(SessionPS);
            InpPosInfo spi1 = GetStreamPosInfo(lines, rl.EffluentStream, "TEMPERATURE=", model.EffluentTemperature.ToString());
            InpPosInfo spi2 = GetStreamPosInfo(lines, rl.EffluentStream2, "TEMPERATURE=", model.EffluentTemperature2.ToString());
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

            //保存inpdata 到文件。
            string newInpDir = DirProtectedSystem + @"\LossOfReactorQuench";
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="streamName"></param>
        /// <param name="rewriteAttr">Temperature=,Rate(WT)=,</param>
        /// <param name="spi"></param>
        private InpPosInfo GetStreamPosInfo(string[] lines, string streamName, string rewriteAttr, string rewriteValue)
        {
            if (string.IsNullOrEmpty(streamName))
            {
                return null;
            }
            StringBuilder sb = new StringBuilder() ;
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

       


        private void CalcLossOfColdFeed()
        {
            string rpPrzFile = DirProtectedSystem + @"\myrp\myrp.prz";
            string rpInpDir = DirProtectedSystem + @"\myrp";
            string caseDir = DirProtectedSystem + @"\LossOfColdFeed";
            PROIIFileOperator.DecompressProIIFile(rpPrzFile, rpInpDir);
            string inpFile = rpInpDir + @"\myrp_backup.inp";
            string[] lines = System.IO.File.ReadAllLines(inpFile);
            StringBuilder sb = new StringBuilder();
            ReactorLoopDAL rldal=new ReactorLoopDAL();
            ReactorLoop rl = rldal.GetModel(SessionPS);
            InpPosInfo spi1 = GetStreamPosInfo(lines,rl.ColdReactorFeedStream,"TEMPERATURE=","1e-8");
            InpPosInfo spi2 = GetStreamPosInfo(lines, rl.ColdReactorFeedStream2, "TEMPERATURE=", "1e-8");
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

            //保存inpdata 到文件。
            string newInpDir = DirProtectedSystem + @"\LossOfColdFeed";
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


    public class InpPosInfo
    {
        public int start;
        public int end;
        public string Name;
        public string NewInfo;
    }
}
