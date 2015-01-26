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
using ReliefProCommon.Enum;

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
        List<string> strHighFeeds = new List<string>();
        List<CustomStream> lstHighFeeds = new List<CustomStream>();

        private double reliefPressure;
        private CustomStream csVapor;
        private CustomStream csLiquid;
        UOMLib.UOMEnum uomEnum;
        public TubeRuptureModel model { set; get; }
        TubeRuptureDAL dal = new TubeRuptureDAL();
        HeatExchanger hx = new HeatExchanger();
        double k = 0;
        int ScenarioID;
        string HeatMethod = string.Empty;
        public TubeRuptureVM(int ScenarioID, SourceFile sourceFileInfo, ISession SessionPS, ISession SessionPF, string dirPlant, string dirProtectedSystem)
        {
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;
            this.ScenarioID = ScenarioID;
            this.SourceFileInfo = sourceFileInfo;
            DirPlant = dirPlant;
            DirProtectedSystem = dirProtectedSystem;
            CalcCMD = new DelegateCommand<object>(CalcResult);
            OKCMD = new DelegateCommand<object>(Save);
            if (ScenarioID == 0)
            {
                TubeRupture dbmodel = new TubeRupture();
                dbmodel.OD_Color = ColorBorder.red.ToString();
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
            uomEnum = UOMSingle.UomEnums.FirstOrDefault(p => p.SessionPlant == this.SessionPF);
            model.ReliefLoadUnit = uomEnum.UserMassRate;
            model.ReliefTemperatureUnit = uomEnum.UserTemperature;
            model.ReliefPressureUnit = uomEnum.UserPressure;
            model.ODUnit = uomEnum.UserFineLength;
            ReadConvert();

            HeatExchangerDAL hxDAL = new HeatExchangerDAL();
            hx = hxDAL.GetModel(SessionPS);

        }

        public void CalcResult(object obj)
        {
            if (model.OD == 0)
            {
                MessageBox.Show("OD could not be zero.","Message Box",MessageBoxButton.OK,MessageBoxImage.Error);
                return;
            }

            int errorType = 0;
            SplashScreenManager.Show(10);
            
            CustomStreamDAL csdal = new CustomStreamDAL();
            CustomStream csTube = null;
            CustomStream csShell = null;
            List<CustomStream> mixTubeFeeds = new List<CustomStream>();
            List<CustomStream> mixShellFeeds = new List<CustomStream>();
            List<string> strTubeFeeds = new List<string>();
            List<string> strShellFeeds = new List<string>();

            strHighFeeds = new List<string>();
            lstHighFeeds = new List<CustomStream>();
            if (hx.TubeFeedStreams.Contains(","))
            {
                csTube = MixFeed(1,ref strTubeFeeds,ref mixTubeFeeds);
            }
            else
            {
                csTube = csdal.GetModel(SessionPS, hx.TubeFeedStreams);
                mixTubeFeeds.Add(csTube);
                strTubeFeeds.Add(hx.TubeFeedStreams);
            }
            if (hx.ShellFeedStreams.Contains(","))
            {
                csShell = MixFeed(2,ref strShellFeeds,ref mixShellFeeds);
            }
            else
            {
                csShell = csdal.GetModel(SessionPS, hx.ShellFeedStreams);
                mixShellFeeds.Add(csShell);
                strShellFeeds.Add(hx.ShellFeedStreams);
            }

            
            if (csShell.Pressure > csTube.Pressure)
            {
                csHigh = csShell;
                strHighFeeds = strShellFeeds;
                lstHighFeeds = mixShellFeeds;
            }
            else
            {
                csHigh = csTube;
                strHighFeeds = strTubeFeeds;
                lstHighFeeds = mixTubeFeeds;
            }
            SplashScreenManager.SentMsgToScreen("Calculation in progress......  10%");
            PSVDAL psvDAL = new PSVDAL();
            PSV psv = psvDAL.GetModel(SessionPS);
            double pressure = psv.Pressure;

            //valid 验证
            if (csHigh.Pressure < psv.Pressure)
            {
                SplashScreenManager.Close();
                MessageBox.Show("High Pressure is lower than pressure of psv", "Message Box");
                return;
            }

            string FileFullPath = DirPlant + @"\" + SourceFileInfo.FileNameNoExt + @"\" + SourceFileInfo.FileName;
            reliefPressure = pressure * psv.ReliefPressureFactor;
            string tempdir = DirProtectedSystem + @"\temp\";
            string dirLatent = tempdir + "TubeRupture1_"+ScenarioID.ToString();
            if (Directory.Exists(dirLatent))
                Directory.Delete(dirLatent,true);
            Directory.CreateDirectory(dirLatent);
            string gd = Guid.NewGuid().ToString();
            string vapor = "S_" + gd.Substring(0, 5).ToUpper();
            string liquid = "S_" + gd.Substring(gd.Length - 5, 5).ToUpper();
            int ImportResult = 0;
            int RunResult = 0;
            SplashScreenManager.SentMsgToScreen("Calculation in progress......  20%");
            PROIIFileOperator.DecompressProIIFile(FileFullPath, tempdir);
            string[] files = Directory.GetFiles(tempdir, "*.inp");
            string sourceFile = files[0];
            string[] lines = System.IO.File.ReadAllLines(sourceFile);
            HeatMethod = ProIIMethod.GetHeatMethod(lines,hx.HXName);
            string content = PROIIFileOperator.getUsableContent(strHighFeeds, lines);
            IFlashCalculate fcalc = ProIIFactory.CreateFlashCalculate(SourceFileInfo.FileVersion);
            string tray1_f = fcalc.Calculate(content, 1, reliefPressure.ToString(), 5, "0", HeatMethod,lstHighFeeds, vapor, liquid, dirLatent, ref ImportResult, ref RunResult);
            if (ImportResult == 1 || ImportResult == 2)
            {
                if (RunResult == 1 || RunResult == 2)
                {
                    IProIIReader reader = ProIIFactory.CreateReader(SourceFileInfo.FileVersion);
                    reader.InitProIIReader(tray1_f);
                    ProIIStreamData proIIVapor = reader.GetSteamInfo(vapor);
                    ProIIStreamData proIILiquid = reader.GetSteamInfo(liquid);
                    
                    ProIIEqData flash = reader.GetEqInfo("Flash", "F_1");
                    

                    string inpFile = dirLatent + @"\a.inp";
                    lines = File.ReadAllLines(inpFile);
                    int pureStreamCount = 0;

                    foreach (CustomStream cs in lstHighFeeds)
                    {
                        ProIIStreamData proIIFeedCopy = reader.GetSteamInfo(cs.StreamName);
                        CustomStream csOld = cs;
                        CustomStream csNew = ProIIToDefault.ConvertProIIStreamToCustomStream(proIIFeedCopy); ;
                        if (ProIIMethod.IsPureComposition(csOld))   //如果是单组份
                        {
                            if (Math.Abs(csOld.VaporFraction - csNew.VaporFraction) >= 0.01) //组分误差》0.01的话
                            {
                                if ((1 - csOld.VaporFraction) < 1e-5)
                                {
                                    //  修改为dew  PHASE=L,&
                                    for (int j = 3; j < lines.Length; j++)
                                    {
                                        if (lines[j].Contains("PROPERTY STREAM=" + csOld.StreamName + ","))
                                        {
                                            lines[j + 2] = "PHASE=V,&";
                                            pureStreamCount++;
                                            break;
                                        }
                                    }

                                }
                                else if (csOld.VaporFraction < 1e-5)
                                {
                                    //修改为泡点  PHASE=V,&
                                    for (int j = 3; j < lines.Length; j++)
                                    {
                                        if (lines[j].Contains("PROPERTY STREAM=" + csOld.StreamName + ","))
                                        {
                                            lines[j + 2] = "PHASE=L,&";
                                            pureStreamCount++;
                                            break;
                                        }
                                    }

                                }
                                else
                                {
                                    //PHASE=M, LFRACTION=0.2&   0.2=1-vaporFraction;
                                    for (int j = 3; j < lines.Length; j++)
                                    {
                                        if (lines[j].Contains("PROPERTY STREAM=" + csOld.StreamName + ","))
                                        {
                                            double LFRACTION = 1 - csOld.VaporFraction;
                                            lines[j + 2] = "PHASE=M,LFRACTION=" + LFRACTION + "&";
                                            pureStreamCount++;
                                            break;
                                        }
                                    }


                                }
                            }

                        }
                    }
                    reader.ReleaseProIIReader();

                    if (pureStreamCount > 0)
                    {
                        File.WriteAllLines(inpFile, lines);
                        IProIIImport proiiimport = ProIIFactory.CreateProIIImport(SourceFileInfo.FileVersion);
                        int importresult = 0;
                        int runresult = 0;
                        tray1_f = proiiimport.ImportProIIINP(inpFile, ref importresult, ref runresult);

                        reader = ProIIFactory.CreateReader(SourceFileInfo.FileVersion);
                        reader.InitProIIReader(tray1_f);
                        proIIVapor = reader.GetSteamInfo(vapor);
                        proIILiquid = reader.GetSteamInfo(liquid);
                        reader.ReleaseProIIReader();
                    }
                    csVapor = ProIIToDefault.ConvertProIIStreamToCustomStream(proIIVapor);
                    csLiquid = ProIIToDefault.ConvertProIIStreamToCustomStream(proIILiquid);
                    SplashScreenManager.SentMsgToScreen("Calculation in progress......  30%");
                    k = csVapor.BulkCPCVRatio;
                    SplashScreenManager.SentMsgToScreen("Calculation in progress......  40%");
                    double error = Math.Abs(csVapor.WeightFlow / csHigh.WeightFlow);
                    if (error < 1e-8) //L
                    {
                        Calc(0);
                    }
                    else if (Math.Abs(error - 1) < 1e-8) //V
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
                    errorType = 1;
                }
            }
            else
            {
                errorType = 2;
            }
            SplashScreenManager.Close();
            if (errorType == 1)
            {
                MessageBox.Show("Prz file is error", "Message Box");
            }
            else if (errorType == 2)
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
            double d = UnitConvert.Convert(model.ODUnit, "in", model.OD);
            double p1 =UnitConvert.Convert(UOMEnum.Pressure, "Mpa", csHigh.Pressure);
            double p2 = UnitConvert.Convert(UOMEnum.Pressure, "Mpa",reliefPressure);
            double rmass = 0;

            bool b = false;
            double pcf = 0;
            b = Algorithm.CheckCritial(p1, p2, k, ref pcf);
            switch (calcType)
            {
                case 0:
                    SplashScreenManager.SentMsgToScreen("Calculation in progress......  50%");
                    rmass = csLiquid.BulkDensityAct;
                    model.ReliefLoad = Algorithm.CalcWL(d, p1, p2, rmass);
                    if (model.ReliefLoad < 0)
                        model.ReliefLoad = 0;

                    model.ReliefLoad = 0;
                    //model.ReliefLoad = UnitConvert.Convert(UOMEnum.MassRate, model.ReliefLoadUnit, model.ReliefLoad); //这是液相的泄放量，而系统现在都是气相的泄放量
                    model.ReliefMW = csLiquid.BulkMwOfPhase;
                    model.ReliefPressure =UnitConvert.Convert(UOMEnum.Pressure, model.ReliefPressureUnit, reliefPressure);
                    model.ReliefTemperature = UnitConvert.Convert(UOMEnum.Temperature, model.ReliefTemperatureUnit, csLiquid.Temperature);
                    model.ReliefCpCv = csLiquid.BulkCPCVRatio;
                    model.ReliefZ = csLiquid.VaporZFmKVal;
                    SplashScreenManager.SentMsgToScreen("Calculation in progress......  60%");
                    break;
                case 1:
                    k = csVapor.BulkCPCVRatio;
                    rmass = csVapor.BulkDensityAct;
                    if (b)
                    {
                        model.ReliefLoad = Algorithm.CalcWv(d, p1, rmass, k); //临界流
                    }
                    else
                    {
                        model.ReliefLoad = Algorithm.CalcWvSecond(d, p1, p2, rmass);//非临界流
                    }
                    SplashScreenManager.SentMsgToScreen("Calculation in progress......  50%");
                    if (model.ReliefLoad < 0)
                        model.ReliefLoad = 0;
                    model.ReliefLoad = UnitConvert.Convert(UOMEnum.MassRate, model.ReliefLoadUnit, model.ReliefLoad);
                    model.ReliefMW = csVapor.BulkMwOfPhase;
                    model.ReliefPressure = UnitConvert.Convert(UOMEnum.Pressure, model.ReliefPressureUnit, reliefPressure);
                    model.ReliefTemperature = UnitConvert.Convert(UOMEnum.Temperature, model.ReliefTemperatureUnit, csVapor.Temperature);
                    model.ReliefCpCv = csVapor.BulkCPCVRatio;
                    model.ReliefZ = csVapor.VaporZFmKVal;
                    SplashScreenManager.SentMsgToScreen("Calculation in progress......  60%");
                    break;
                case 2:
                    //再做一次闪蒸，求出
                    if (b)
                    {
                        string FileFullPath = DirPlant + @"\" + SourceFileInfo.FileNameNoExt + @"\" + SourceFileInfo.FileName;
                        string tempdir = DirProtectedSystem + @"\temp\";
                        string dirLatent = tempdir + "TubeRupture2_"+ScenarioID.ToString();
                        if (Directory.Exists(dirLatent))
                            Directory.Delete(dirLatent, true);
                        Directory.CreateDirectory(dirLatent);
                        string gd = Guid.NewGuid().ToString();
                        string vapor = "S_" + gd.Substring(0, 5).ToUpper();
                        string liquid = "S_" + gd.Substring(gd.Length - 5, 5).ToUpper();
                        int ImportResult = 0;
                        int RunResult = 0;
                        PROIIFileOperator.DecompressProIIFile(FileFullPath, tempdir);
                        string content = PROIIFileOperator.getUsableContent(strHighFeeds, tempdir);
                        IFlashCalculate fcalc = ProIIFactory.CreateFlashCalculate(SourceFileInfo.FileVersion);
                        string tray1_f = fcalc.Calculate(content, 1, pcf.ToString(), 5, "0", HeatMethod, lstHighFeeds, vapor, liquid, dirLatent, ref ImportResult, ref RunResult);
                        if (ImportResult == 1 || ImportResult == 2)
                        {
                            if (RunResult == 1 || RunResult == 2)
                            {
                                SplashScreenManager.SentMsgToScreen("Calculation in progress......  50%");
                                IProIIReader reader = ProIIFactory.CreateReader(SourceFileInfo.FileVersion);
                                reader.InitProIIReader(tray1_f);
                                ProIIStreamData proIIVapor = reader.GetSteamInfo(vapor);
                                ProIIStreamData proIILiquid = reader.GetSteamInfo(liquid);
                                ProIIEqData flash = reader.GetEqInfo("Flash", "F_1");
                                

                                string inpFile = dirLatent + @"\a.inp";
                                string[]  lines= File.ReadAllLines(inpFile);
                                int pureStreamCount = 0;

                                foreach (CustomStream cs in lstHighFeeds)
                                {
                                    ProIIStreamData proIIFeedCopy = reader.GetSteamInfo(cs.StreamName);
                                    CustomStream csOld = lstHighFeeds[0];
                                    CustomStream csNew = ProIIToDefault.ConvertProIIStreamToCustomStream(proIIFeedCopy); ;
                                    if (ProIIMethod.IsPureComposition(csOld))   //如果是单组份
                                    {
                                        if (Math.Abs(csOld.VaporFraction - csNew.VaporFraction) >= 0.01) //组分误差》0.01的话
                                        {
                                            if ((1 - csOld.VaporFraction) < 1e-5)
                                            {
                                                //  修改为dew  PHASE=L,&
                                                for (int j = 3; j < lines.Length; j++)
                                                {
                                                    if (lines[j].Contains("PROPERTY STREAM=" + csOld.StreamName + ","))
                                                    {
                                                        lines[j + 2] = "PHASE=V,&";
                                                        pureStreamCount++;
                                                        break;
                                                    }
                                                }

                                            }
                                            else if (csOld.VaporFraction < 1e-5)
                                            {
                                                //修改为泡点  PHASE=V,&
                                                for (int j = 3; j < lines.Length; j++)
                                                {
                                                    if (lines[j].Contains("PROPERTY STREAM=" + csOld.StreamName + ","))
                                                    {
                                                        lines[j + 2] = "PHASE=L,&";
                                                        pureStreamCount++;
                                                        break;
                                                    }
                                                }

                                            }
                                            else
                                            {
                                                //PHASE=M, LFRACTION=0.2&   0.2=1-vaporFraction;
                                                for (int j = 3; j < lines.Length; j++)
                                                {
                                                    if (lines[j].Contains("PROPERTY STREAM=" + csOld.StreamName + ","))
                                                    {
                                                        double LFRACTION = 1 - csOld.VaporFraction;
                                                        lines[j + 2] = "PHASE=M,LFRACTION=" + LFRACTION + "&";
                                                        pureStreamCount++;
                                                        break;
                                                    }
                                                }


                                            }
                                        }

                                    }
                                }
                                reader.ReleaseProIIReader();

                                if (pureStreamCount > 0)
                                {
                                    File.WriteAllLines(inpFile, lines);
                                    IProIIImport proiiimport = ProIIFactory.CreateProIIImport(SourceFileInfo.FileVersion);
                                    int importresult = 0;
                                    int runresult = 0;
                                    tray1_f = proiiimport.ImportProIIINP(inpFile, ref importresult, ref runresult);

                                    reader = ProIIFactory.CreateReader(SourceFileInfo.FileVersion);
                                    reader.InitProIIReader(tray1_f);
                                    proIIVapor = reader.GetSteamInfo(vapor);
                                    proIILiquid = reader.GetSteamInfo(liquid);
                                    reader.ReleaseProIIReader();
                                }


                                CustomStream csVapor2 = ProIIToDefault.ConvertProIIStreamToCustomStream(proIIVapor);
                                CustomStream csLiquid2 = ProIIToDefault.ConvertProIIStreamToCustomStream(proIILiquid);

                                double tmpP2 = UnitConvert.Convert(UOMEnum.Pressure, "Mpa", csLiquid2.Pressure);
                                SplashScreenManager.SentMsgToScreen("Calculation in progress......  60%");
                                double Rv = csVapor2.WeightFlow / csHigh.WeightFlow;
                                double KL = Algorithm.CalcKL(p1, tmpP2, csLiquid2.BulkDensityAct);
                                double Kv = Algorithm.CalcKv(p1, csVapor2.BulkDensityAct, k);
                                model.ReliefLoad = Algorithm.CalcWH(Rv, KL, Kv, d);
                                if (model.ReliefLoad < 0)
                                    model.ReliefLoad = 0;
                                model.ReliefLoad = UnitConvert.Convert(UOMEnum.MassRate, model.ReliefLoadUnit, model.ReliefLoad);
                                model.ReliefMW = csVapor2.BulkMwOfPhase;
                                model.ReliefPressure = UnitConvert.Convert(UOMEnum.Pressure, model.ReliefPressureUnit, reliefPressure);
                                model.ReliefTemperature = UnitConvert.Convert(UOMEnum.Temperature, model.ReliefTemperatureUnit, csVapor2.Temperature);
                                model.ReliefCpCv = csVapor2.BulkCPCVRatio;
                                model.ReliefZ = csVapor2.VaporZFmKVal;
                                SplashScreenManager.SentMsgToScreen("Calculation in progress......  70%");
                            }

                            else
                            {
                                //MessageBox.Show("Prz file is error", "Message Box");
                            }
                        }
                        else
                        {
                            //MessageBox.Show("inp file is error", "Message Box");

                        }
                    }
                    else
                    {
                        SplashScreenManager.SentMsgToScreen("Calculation in progress......  50%");
                        double tmpP21 = UnitConvert.Convert(UOMEnum.Pressure, "Mpa", csLiquid.Pressure);
                        double tmpP22 = UnitConvert.Convert(UOMEnum.Pressure, "Mpa", csVapor.Pressure);
                        double Rv = csVapor.WeightFlow / csHigh.WeightFlow;
                        double KL = Algorithm.CalcKL(p1, tmpP21, csLiquid.BulkDensityAct);
                        double Kv = Algorithm.CalcKvSecond(p1,tmpP22, csVapor.BulkDensityAct);
                        model.ReliefLoad = Algorithm.CalcWH(Rv, KL,Kv,  d);

                        model.ReliefLoad = UnitConvert.Convert(UOMEnum.MassRate, model.ReliefLoadUnit, model.ReliefLoad);
                        model.ReliefMW = csVapor.BulkMwOfPhase;
                        model.ReliefPressure = UnitConvert.Convert(UOMEnum.Pressure, model.ReliefPressureUnit, reliefPressure);
                        model.ReliefTemperature = UnitConvert.Convert(UOMEnum.Temperature, model.ReliefTemperatureUnit, csVapor.Temperature);
                        model.ReliefCpCv = csVapor.BulkCPCVRatio;
                        model.ReliefZ = csVapor.VaporZFmKVal;
                        SplashScreenManager.SentMsgToScreen("Calculation in progress......  60%");
                    }
                    break;

            }
            if (model.ReliefLoad < 0)
                model.ReliefLoad = 0;

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
                    model.dbmodel.ScenarioID = ScenarioID;
                    if (model.dbmodel.ID == 0)
                        dal.Add(model.dbmodel, SessionPS);
                    else
                        dal.Update(model.dbmodel, SessionPS);
                    SaveScenario(model.dbmodel);
                    SessionPS.Flush();
                    wd.DialogResult = true;
                }
            }
        }
        private void SaveScenario(TubeRupture model)
        {
            ScenarioDAL db = new ScenarioDAL();
            var sModel = db.GetModel(model.ScenarioID, SessionPS);
            sModel.ReliefLoad = model.ReliefLoad;
            sModel.ReliefPressure = model.ReliefPressure;
            sModel.ReliefTemperature = model.ReliefTemperature;
            sModel.ReliefMW = model.ReliefMW;
            sModel.ReliefCpCv = model.ReliefCpCv;
            sModel.ReliefZ = model.ReliefZ;
            db.Update(sModel, SessionPS);
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
            model.dbmodel.ReliefCpCv = model.ReliefCpCv;
            model.dbmodel.ReliefZ = model.ReliefZ;
            model.dbmodel.ReliefLoad = UnitConvert.Convert(model.ReliefLoadUnit, UOMLib.UOMEnum.MassRate.ToString(), model.ReliefLoad);
            model.dbmodel.ReliefTemperature = UnitConvert.Convert(model.ReliefTemperatureUnit, UOMLib.UOMEnum.Temperature.ToString(), model.ReliefTemperature);
            model.dbmodel.ReliefPressure = UnitConvert.Convert(model.ReliefPressureUnit, UOMLib.UOMEnum.Pressure.ToString(), model.ReliefPressure);
        }

        private bool CheckDataValid()
        {
            bool b = true;
            if (model.OD <= 0)
            {
                string message = Application.Current.FindResource("ZeroWarning").ToString();
                MessageBox.Show(message, "Message Box");
                return false;
            }
            return b;
        }

        private bool IsExistPureComposition(List<CustomStream> lst)
        {
            bool b=false;
            foreach (CustomStream cs in lst)
            {
                if (ProIIMethod.IsPureComposition(cs))
                {
                    b = true;
                    break;
                }
            }
            return b;
        }

        private List<CustomStream> CalcPureComposion(List<CustomStream> lstOld, List<CustomStream> lstNew)
        {
            List<CustomStream> lst = new List<CustomStream>();
            string tempdir = DirProtectedSystem + @"\temp\";
            IFlashCalculate fcalc = ProIIFactory.CreateFlashCalculate(SourceFileInfo.FileVersion);
            for (int i = 0; i < lstOld.Count; i++)
            {
                CustomStream csOld = lstOld[i];
                CustomStream csNew = lstNew[i];
                if (ProIIMethod.IsPureComposition(csOld))   //如果是单组份
                {
                    if (Math.Abs(csOld.VaporFraction - csNew.VaporFraction) >= 0.01) //组分误差》0.01的话
                    {
                        if (csOld.VaporFraction == 1)
                        {
                            // 闪蒸到dew
                            string gd = Guid.NewGuid().ToString();
                            string vapor = "S_" + gd.Substring(0, 5).ToUpper();
                            string liquid = "S_" + gd.Substring(gd.Length - 5, 5).ToUpper();
                            string[] files = Directory.GetFiles(tempdir, "*.inp");
                            string sourceFile = files[0];
                            string[] lines = System.IO.File.ReadAllLines(sourceFile);
                            HeatMethod = ProIIMethod.GetHeatMethod(lines, hx.HXName);


                            string content = PROIIFileOperator.getUsableContent(csOld.StreamName, tempdir);
                            int ImportResult1 = 0;
                            int RunResult1 = 0;
                            string dirDew = tempdir + "TubeRupture1_dirDew" + ScenarioID.ToString();
                            if (Directory.Exists(dirDew))
                                Directory.Delete(dirDew, true);
                            Directory.CreateDirectory(dirDew);
                            string resultfile = fcalc.Calculate(content, 1, reliefPressure.ToString(), 4, "",HeatMethod, csOld, vapor, liquid, dirDew, ref ImportResult1, ref RunResult1);
                            IProIIReader reader = ProIIFactory.CreateReader(SourceFileInfo.FileVersion);
                            reader.InitProIIReader(resultfile);
                            ProIIStreamData ProIINew2 = reader.GetSteamInfo(csOld.StreamName);
                            reader.ReleaseProIIReader();
                            CustomStream csNew2 = ProIIToDefault.ConvertProIIStreamToCustomStream(ProIINew2);
                            csOld = csNew2;
                        }
                        else if (csHigh.VaporFraction == 0)
                        {
                            //闪蒸到泡点
                            string gd = Guid.NewGuid().ToString();
                            string vapor = "S_" + gd.Substring(0, 5).ToUpper();
                            string liquid = "S_" + gd.Substring(gd.Length - 5, 5).ToUpper();

                            string[] files = Directory.GetFiles(tempdir, "*.inp");
                            string sourceFile = files[0];
                            string[] lines = System.IO.File.ReadAllLines(sourceFile);
                            HeatMethod = ProIIMethod.GetHeatMethod(lines, hx.HXName);
                            string content = PROIIFileOperator.getUsableContent(csOld.StreamName, tempdir);

                            int ImportResult1 = 0;
                            int RunResult1 = 0;
                            string dirDew = tempdir + "TubeRupture1_dirDew" + ScenarioID.ToString();
                            if (Directory.Exists(dirDew))
                                Directory.Delete(dirDew, true);
                            Directory.CreateDirectory(dirDew);
                            string resultfile = fcalc.Calculate(content, 1, reliefPressure.ToString(), 3, "",HeatMethod, csOld, vapor, liquid, dirDew, ref ImportResult1, ref RunResult1);
                            IProIIReader reader = ProIIFactory.CreateReader(SourceFileInfo.FileVersion);
                            reader.InitProIIReader(resultfile);
                            ProIIStreamData ProIINew2 = reader.GetSteamInfo(csOld.StreamName);
                            reader.ReleaseProIIReader();
                            CustomStream csNew2 = ProIIToDefault.ConvertProIIStreamToCustomStream(ProIINew2);
                            csOld = csNew2;
                        }
                       
                    }
                }
                lst.Add(csOld);
            }
            return lst;
        }



        private CustomStream MixFeed(int feedType, ref  List<string> strFeeds, ref List<CustomStream> mixFeeds)
        {
            string mixFeedType = "Tube";
            string mixFeedNames = hx.TubeFeedStreams;
           
            if (feedType == 2)
            {
                mixFeedType = "Shell";
                mixFeedNames = hx.ShellFeedStreams;
            }
            string dirMix = DirProtectedSystem + @"\mix_" + mixFeedType + ScenarioID.ToString();
            string sbcontent = string.Empty;
            string[] feeds = mixFeedNames.Split(',');
            CustomStreamDAL csdal = new CustomStreamDAL();
            foreach (string s in feeds)
            {
                CustomStream cs = csdal.GetModel(SessionPS, s);
                mixFeeds.Add(cs);
                strFeeds.Add(s);
            }
            string dir = DirPlant + @"\" + SourceFileInfo.FileNameNoExt;
            string[] sourceFiles = Directory.GetFiles(dir, "*.inp");
            string sourceFile = sourceFiles[0];
            string[] lines = System.IO.File.ReadAllLines(sourceFile);
            sbcontent = PROIIFileOperator.getUsableContent(strFeeds, lines);
            IMixCalculate mixcalc = ProIIFactory.CreateMixCalculate(SourceFileInfo.FileVersion);
            string mixProductName = Guid.NewGuid().ToString().Substring(0, 6);

            if (Directory.Exists(dirMix))
                Directory.Delete(dirMix, true);
            Directory.CreateDirectory(dirMix);
            
            int mixImportResult = 1;
            int mixRunResult = 1;
            string mixPrzFile = mixcalc.Calculate(sbcontent, mixFeeds, mixProductName, dirMix, ref mixImportResult, ref mixRunResult);

            if (mixImportResult == 1 || mixImportResult == 2)
            {
                if (mixRunResult == 1 || mixRunResult == 2)
                {
                    IProIIReader reader = ProIIFactory.CreateReader(SourceFileInfo.FileVersion);
                    reader.InitProIIReader(mixPrzFile);
                    ProIIStreamData proIIvapor = reader.GetSteamInfo(mixProductName);

                    string inpFile = dirMix + @"\a.inp";
                    lines= File.ReadAllLines(inpFile);
                    int pureStreamCount = 0;
                    for (int i = 0; i < mixFeeds.Count; i++)
                    {
                        CustomStream csOld = mixFeeds[i];
                        ProIIStreamData proiistream = reader.GetSteamInfo(csOld.StreamName);
                        CustomStream csNew = ProIIToDefault.ConvertProIIStreamToCustomStream(proiistream); ;
                        if (ProIIMethod.IsPureComposition(csOld))   //如果是单组份
                        {
                            if (Math.Abs(csOld.VaporFraction - csNew.VaporFraction) >= 0.01) //组分误差》0.01的话
                            {
                                if (csOld.VaporFraction == 1)
                                {
                                    //  修改为dew  PHASE=L,&
                                    for(int j=3;j<lines.Length;j++)
                                    {
                                        if (lines[i].Contains("PROPERTY STREAM=" + csOld.StreamName + ","))
                                        {
                                            lines[j + 2] = "PHASE=V,&";
                                            pureStreamCount++;
                                            break;
                                        }
                                    }
                                   
                                }
                                else if (csHigh.VaporFraction == 0)
                                {
                                    //修改为泡点  PHASE=V,&
                                    for (int j = 3; j < lines.Length; j++)
                                    {
                                        if (lines[i].Contains("PROPERTY STREAM=" + csOld.StreamName + ","))
                                        {
                                            lines[j + 2] = "PHASE=L,&";
                                            pureStreamCount++;
                                            break;
                                        }
                                    }
                                    
                                }
                                else
                                {
                                    //PHASE=M, LFRACTION=0.2&   0.2=1-vaporFraction;
                                    for (int j = 3; j < lines.Length; j++)
                                    {
                                        if (lines[i].Contains("PROPERTY STREAM=" + csOld.StreamName + ","))
                                        {
                                            double LFRACTION = 1 - csOld.VaporFraction;
                                            lines[i + 2] = "PHASE=M,LFRACTION=" + LFRACTION + "&";
                                            pureStreamCount++;
                                            break;
                                        }
                                    }
                                  
                                }
                            }

                        }

                    }
                    reader.ReleaseProIIReader();
                    if (pureStreamCount > 0)
                    {
                        File.WriteAllLines(inpFile,lines);
                        IProIIImport proiiimport=ProIIFactory.CreateProIIImport(SourceFileInfo.FileVersion);
                        int importresult=0;
                        int runresult=0;
                        mixPrzFile = proiiimport.ImportProIIINP(inpFile, ref importresult,ref runresult);
                        
                        reader = ProIIFactory.CreateReader(SourceFileInfo.FileVersion);
                        reader.InitProIIReader(mixPrzFile);
                        proIIvapor = reader.GetSteamInfo(mixProductName);
                        reader.ReleaseProIIReader();

                    }
                   
                    CustomStream mixCSProduct = ProIIToDefault.ConvertProIIStreamToCustomStream(proIIvapor);
                    return mixCSProduct;
                }
                else
                {
                    //MessageBox.Show("inp file is error", "Message Box");
                    return null;
                }
            }
            else
            {
                //MessageBox.Show("Prz file is error", "Message Box");
                return null;
            }

        }
    }
}
