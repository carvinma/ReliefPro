using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;
using System.IO;
using Microsoft.Practices.Prism.Commands;
using NHibernate;
using ReliefProBLL;
using ReliefProMain.Models.HXs;
using UOMLib;
using ReliefProModel;
using ReliefProCommon.CommonLib;
using ProII;
using ReliefProDAL;
using System.Collections.ObjectModel;
using ReliefProDAL.HXs;
using ReliefProModel.HXs;
using ReliefProMain.View.HXs;

namespace ReliefProMain.ViewModel.HXs
{
    public class HXBlockedInVM : ViewModelBase
    {
        public ICommand CalcCMD { get; set; }
        public ICommand OKCMD { get; set; }
        private ISession SessionPS;
        private ISession SessionPF;
        private string DirPlant;
        private string DirProtectedSystem;
        public SourceFile SourceFileInfo { get; set; }
        public string FileFullPath { get; set; }
        public HXBlockedInModel model { get; set; }
        private HXBLL hxBLL;
        CustomStream normalHotInlet = null;
        CustomStream normalColdInlet = new CustomStream();//单个
        List<CustomStream> normalColdInletList = new List<CustomStream>();
        List<string> lstFeed = new List<string>();
        CustomStream normalColdOutlet = new CustomStream();
        CustomStream normalHotOutlet = new CustomStream();
        PSVDAL psvDAL ;
        PSV psv;
        double reliefPressure;
        double criticalPressure;
        double criticalTemperature;
        double cricondenbarPressure;
        double cricondenbarTemperature;
        public bool IsColdIn = true;
        int ScenarioID;
        string HeatMethod = string.Empty;
        HeatExchanger heathx = new HeatExchanger();
        public HXBlockedInVM(int ScenarioID, SourceFile sourceFileInfo, ISession SessionPS, ISession SessionPF, string dirPlant, string dirProtectedSystem)
        {
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;
            this.ScenarioID = ScenarioID;
            DirPlant = dirPlant;
            DirProtectedSystem = dirProtectedSystem;
            DirPlant = dirPlant;
            DirProtectedSystem = dirProtectedSystem;
            SourceFileInfo = sourceFileInfo;
            psvDAL = new PSVDAL();
            psv = psvDAL.GetModel(SessionPS);
            FileFullPath = DirPlant + @"\" + sourceFileInfo.FileNameNoExt + @"\" + sourceFileInfo.FileName;
            OKCMD = new DelegateCommand<object>(Save);
            CalcCMD = new DelegateCommand<object>(CalcResult);

            hxBLL = new HXBLL(SessionPS, SessionPF);
            var blockModel = hxBLL.GetHXBlockedOutletModel(ScenarioID);
            blockModel = hxBLL.ReadConvertHXBlockedOutletModel(blockModel);

            model = new HXBlockedInModel(blockModel);
            model.dbmodel.ScenarioID = ScenarioID;

            //判断冷测进出，

            CustomStreamDAL csdal = new CustomStreamDAL();
            HeatExchangerDAL heatexdal = new HeatExchangerDAL();
            heathx = heatexdal.GetModel(SessionPS);
            string coldFeed = string.Empty;
            if (psv.LocationDescription == "Shell")
            {
                coldFeed = heathx.ShellFeedStreams;
            }
            else
            {
                coldFeed = heathx.TubeFeedStreams;
            }
            if (heathx.ColdInlet != coldFeed)
            {
                IsColdIn = false;
                return;
            }

            if (heathx.ColdInlet.Contains(","))
            {
                //需要做mixer
                int ImportResult = 0;
                int RunResult = 0;
                string tempdir = DirProtectedSystem + @"\temp\";
                string dirMix = tempdir + "BlockedInlet_Mix";
                if (Directory.Exists(dirMix))
                    Directory.Delete(dirMix,true);
                Directory.CreateDirectory(dirMix);

                string[] coldfeeds = heathx.ColdInlet.Split(',');
                foreach (string s in coldfeeds)
                {
                    CustomStream cs=csdal.GetModel(SessionPS,s);
                    normalColdInletList.Add(cs);
                }
                lstFeed = coldfeeds.ToList();

                string sbcontent = string.Empty;
                string[] files = Directory.GetFiles(tempdir, "*.inp");
                string sourceFile = files[0];
                string[] lines = System.IO.File.ReadAllLines(sourceFile);

                sbcontent = PROIIFileOperator.getUsableContent(coldfeeds.ToList(), lines);

                IMixCalculate mixcalc = ProIIFactory.CreateMixCalculate(SourceFileInfo.FileVersion);
                string mixProduct = Guid.NewGuid().ToString().Substring(0, 6);
                string tray1_f = mixcalc.Calculate(sbcontent, normalColdInletList, mixProduct, dirMix, ref ImportResult, ref RunResult);
                if (ImportResult == 1 || ImportResult == 2)
                {
                    if (RunResult == 1 || RunResult == 2)
                    {
                        IProIIReader reader = ProIIFactory.CreateReader(SourceFileInfo.FileVersion);
                        reader.InitProIIReader(tray1_f);
                        ProIIStreamData proIIProduct = reader.GetSteamInfo(mixProduct);
                        normalColdInlet = ProIIToDefault.ConvertProIIStreamToCustomStream(proIIProduct);
                        reader.ReleaseProIIReader();
                        //normalColdInletList.Add(normalColdInlet);
                    }
                }
            }
            else
            {
                normalColdInlet = csdal.GetModel(SessionPS, heathx.ColdInlet);
                normalColdInletList.Add(normalColdInlet);
                lstFeed.Add(heathx.ColdInlet);

            }
            normalColdOutlet = csdal.GetModel(SessionPS, heathx.ColdOutlet);
            normalHotInlet = csdal.GetModel(SessionPS, heathx.HotInlet);
            normalHotOutlet = csdal.GetModel(SessionPS, heathx.HotOutlet);
            HeatExchangerDAL heatdal = new HeatExchangerDAL();
            HeatExchanger heat = heatdal.GetModel(SessionPS);
            model.NormalDuty = heat.Duty;
            model.NormalColdInletTemperature = normalColdInlet.Temperature;
            model.NormalColdOutletTemperature = normalColdOutlet.Temperature;
            if (normalHotInlet != null)
            {
                model.NormalHotTemperature = normalHotInlet.Temperature;
            }
            model.ColdStream = heathx.ColdInlet;


            UOMLib.UOMEnum uomEnum = UOMSingle.UomEnums.FirstOrDefault(p => p.SessionPlant == this.SessionPF);
            model.NormalDutyUnit = uomEnum.UserEnthalpyDuty;
            model.NormalHotTemperatureUnit = uomEnum.UserTemperature;
            model.NormalColdInletTemperatureUnit = uomEnum.UserTemperature;
            model.NormalColdOutletTemperatureUnit = uomEnum.UserTemperature;
            model.LatentPointUnit = uomEnum.UserSpecificEnthalpy;
            model.ReliefLoadUnit = uomEnum.UserMassRate;
            model.ReliefTemperatureUnit = uomEnum.UserTemperature;
            model.ReliefPressureUnit = uomEnum.UserPressure;
            ReadConvertModel();
        }

        private void ReadConvertModel()
        {
            model.ColdStream = model.ColdStream;
            model.NormalDuty = UnitConvert.Convert(UOMLib.UOMEnum.EnthalpyDuty.ToString(),model.NormalDutyUnit,  model.NormalDuty);
            model.NormalHotTemperature = UnitConvert.Convert( UOMLib.UOMEnum.Temperature.ToString(),model.NormalHotTemperatureUnit, model.NormalHotTemperature);
            model.NormalColdInletTemperature = UnitConvert.Convert( UOMLib.UOMEnum.Temperature.ToString(), model.NormalColdInletTemperatureUnit,model.NormalColdInletTemperature);
            model.NormalColdOutletTemperature = UnitConvert.Convert(UOMLib.UOMEnum.Temperature.ToString(),model.NormalColdOutletTemperatureUnit,  model.NormalColdOutletTemperature);
            model.LatentPoint = UnitConvert.Convert(UOMLib.UOMEnum.SpecificEnthalpy.ToString(),model.LatentPointUnit,  model.LatentPoint);
            model.ReliefLoad = UnitConvert.Convert(UOMLib.UOMEnum.MassRate.ToString(),model.ReliefLoadUnit,  model.ReliefLoad);
            model.ReliefTemperature = UnitConvert.Convert(UOMLib.UOMEnum.Temperature.ToString(),model.ReliefTemperatureUnit,  model.ReliefTemperature);
            model.ReliefPressure = UnitConvert.Convert(UOMLib.UOMEnum.Pressure.ToString(),model.ReliefPressureUnit,  model.ReliefPressure);
        }

        private void WriteConvertModel()
        {
            model.dbmodel.ColdStream = model.ColdStream;
            model.dbmodel.NormalDuty = UnitConvert.Convert(model.NormalDutyUnit, UOMLib.UOMEnum.EnthalpyDuty.ToString(), model.NormalDuty);
            model.dbmodel.NormalHotTemperature = UnitConvert.Convert(model.NormalHotTemperatureUnit, UOMLib.UOMEnum.Temperature.ToString(), model.NormalHotTemperature);
            model.dbmodel.NormalColdInletTemperature = UnitConvert.Convert(model.NormalColdInletTemperatureUnit, UOMLib.UOMEnum.Temperature.ToString(), model.NormalColdInletTemperature);
            model.dbmodel.NormalColdOutletTemperature = UnitConvert.Convert(model.NormalColdOutletTemperatureUnit, UOMLib.UOMEnum.Temperature.ToString(), model.NormalColdOutletTemperature);
            model.dbmodel.LatentPoint = UnitConvert.Convert(model.LatentPointUnit, UOMLib.UOMEnum.SpecificEnthalpy.ToString(), model.LatentPoint);
            model.dbmodel.ReliefMW = model.ReliefMW;
            model.dbmodel.ReliefLoad = UnitConvert.Convert(model.ReliefLoadUnit, UOMLib.UOMEnum.MassRate.ToString(), model.ReliefLoad);
            model.dbmodel.ReliefTemperature = UnitConvert.Convert(model.ReliefTemperatureUnit, UOMLib.UOMEnum.Temperature.ToString(), model.ReliefTemperature);
            model.dbmodel.ReliefPressure = UnitConvert.Convert(model.ReliefPressureUnit, UOMLib.UOMEnum.Pressure.ToString(), model.ReliefPressure);
            model.dbmodel.ReliefCpCv = model.ReliefCpCv;
            model.dbmodel.ReliefZ = model.ReliefZ;
        }
        
        private void CalcResult(object obj)
        {
            //if (!model.CheckData()) return; 
            try
            {
                SplashScreenManager.Show(10);
                SplashScreenManager.SentMsgToScreen("Calculation is in progress, please wait…");
 
                double pressure = psv.Pressure;
                reliefPressure = pressure * psv.ReliefPressureFactor;

                if (normalColdInlet.VaporFraction == 1)
                {
                    // gas expansion  全气相的不做。 condition  1
                    //MethodGasExp();
                    model.ReliefLoad = UnitConvert.Convert(UOMEnum.MassRate,model.ReliefLoadUnit, 0);
                    model.ReliefMW = normalColdInlet.BulkMwOfPhase;
                    model.ReliefPressure =  UnitConvert.Convert(UOMEnum.Pressure,model.ReliefPressureUnit,reliefPressure);
                    model.ReliefTemperature = UnitConvert.Convert(UOMEnum.Temperature, model.ReliefTemperatureUnit, normalColdInlet.Temperature);
                    model.ReliefZ = normalColdInlet.VaporZFmKVal;
                    model.ReliefCpCv = normalColdInlet.BulkCPCVRatio;
                }
                else
                {
                    int citicalResult = CalcBlockedInCriticalPressure(lstFeed);

                    if (citicalResult == 2) //no critical point
                    {
                        if (normalColdInlet.VaporFraction == 0)
                        {
                            //liquid  and flash Prelief ,bubble
                           bool b= MethodBubble1();
                           if (!b)
                           {
                               MethodBubbleFail();                              
                           }
                        }
                        else
                        {
                            //vapor--liquid and flash prelief tuty=qnormal
                            MethodDuty2();
                        }
                    }
                    else
                    {
                        if (reliefPressure < criticalPressure)
                        {
                            //flash bubble
                           bool b= MethodBubble1();
                           if (!b)
                           {
                               if (normalColdInlet.VaporFraction == 0) //liquid
                               {
                                   MethodBubbleFail();
                               }
                               else
                               {
                                   MethodDuty2();  //vap-liquid
                               }
                           }
                        }
                        else
                        {
                            if (model.NormalHotTemperature<criticalTemperature)
                            {
                               bool b= MethodBubble1();
                               if (!b)
                               {
                                   MethodCritical3();
                               }
                            }
                            else
                            {
                                MethodCritical3();
                            }
                        }
                    }
                }
                if (model.ReliefLoad < 0)
                    model.ReliefLoad = 0;

                SplashScreenManager.SentMsgToScreen("Calculation finished");

            }
            catch (Exception ex)
            { }
            finally
            {
                SplashScreenManager.Close();
            }
        }

        private bool MethodBubble1()
        {
            double tAvg = 0.5 * (normalColdInlet.Temperature + normalColdOutlet.Temperature);            
            double Q = model.NormalDuty;
            SplashScreenManager.SentMsgToScreen("Calculation is in progress, please wait…");
            string tempdir = DirProtectedSystem + @"\temp\";
            string dirLatent = tempdir + "BlockedIn_Bubble"+ScenarioID.ToString();
            if (Directory.Exists(dirLatent))
                Directory.Delete(dirLatent, true);
            Directory.CreateDirectory(dirLatent);
            string gd = Guid.NewGuid().ToString();
            string vapor = "S_" + gd.Substring(0, 5).ToUpper();
            string liquid = "S_" + gd.Substring(gd.Length - 5, 5).ToUpper();
            int ImportResult = 0;
            int RunResult = 0;
            PROIIFileOperator.DecompressProIIFile(FileFullPath, tempdir);
            string[] sourceFiles = Directory.GetFiles(tempdir, "*.inp");
            string sourceFile = sourceFiles[0];
            string[] lines = System.IO.File.ReadAllLines(sourceFile);
            HeatMethod = ProIIMethod.GetHeatMethod(lines, heathx.HXName);
            SplashScreenManager.SentMsgToScreen("Calculation is in progress, please wait…");
            List<string> coldList = new List<string>();
            foreach (CustomStream cs in normalColdInletList)
            {
                coldList.Add(cs.StreamName);
            }
            string content = PROIIFileOperator.getUsableContent(coldList, tempdir);
            SplashScreenManager.SentMsgToScreen("Calculation is in progress, please wait…");
            IFlashCalculate fcalc = ProIIFactory.CreateFlashCalculate(SourceFileInfo.FileVersion);
            string tray1_f = fcalc.Calculate(content, 1, reliefPressure.ToString(), 3, "0",HeatMethod, normalColdInletList, vapor, liquid, dirLatent, ref ImportResult, ref RunResult);
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
                    CustomStream liquidcs = ProIIToDefault.ConvertProIIStreamToCustomStream(proIILiquid);
                    CustomStream vaporcs = ProIIToDefault.ConvertProIIStreamToCustomStream(proIIVapor);
                    double latent = vaporcs.SpEnthalpy - liquidcs.SpEnthalpy;
                    //double tcoldbprelief =  double.Parse(flash.TempCalc);//转换单位

                    double tcoldbprelief = UnitConvert.Convert("K", "C", double.Parse(flash.TempCalc));
                    model.LatentPoint = latent;
                    if (latent > 0)
                    {
                        model.ReliefLoad = Q / latent * (model.NormalHotTemperature - tcoldbprelief) / (model.NormalHotTemperature - tAvg);
                        if (model.ReliefLoad < 0 || tcoldbprelief > model.NormalHotTemperature)
                            model.ReliefLoad = 0;
                        model.ReliefMW = vaporcs.BulkMwOfPhase;
                        model.ReliefPressure = reliefPressure;
                        model.ReliefTemperature = vaporcs.Temperature;
                        model.ReliefCpCv = vaporcs.BulkCPCVRatio;
                        model.ReliefZ = vaporcs.VaporZFmKVal;
                        return true;
                    }
                    else 
                    {
                        return false;
                    }
                    
                }

                else
                {
                    //MessageBox.Show("Prz file is error", "Message Box");
                    return false;
                }
            }
            else
            {
                //MessageBox.Show("inp file is error", "Message Box");
                return false;

            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void MethodDuty2()
        {
            double tAvg = 0.5 * (normalColdInlet.Temperature + normalColdOutlet.Temperature);            
            double Q = model.NormalDuty;
            SplashScreenManager.SentMsgToScreen("Calculation is in progress, please wait…");
            string tempdir = DirProtectedSystem + @"\temp\";
            string dirLatent = tempdir + "BlockedIn_duty" + ScenarioID.ToString() ;
            if (Directory.Exists(dirLatent))
                Directory.Delete(dirLatent, true);
            Directory.CreateDirectory(dirLatent);
            string gd = Guid.NewGuid().ToString();
            string vapor = "S_" + gd.Substring(0, 5).ToUpper();
            string liquid = "S_" + gd.Substring(gd.Length - 5, 5).ToUpper();
            int ImportResult = 0;
            int RunResult = 0;
            PROIIFileOperator.DecompressProIIFile(FileFullPath, tempdir);
            string[] sourceFiles = Directory.GetFiles(tempdir, "*.inp");
            string sourceFile = sourceFiles[0];
            string[] lines = System.IO.File.ReadAllLines(sourceFile);
            HeatMethod = ProIIMethod.GetHeatMethod(lines, heathx.HXName);
            SplashScreenManager.SentMsgToScreen("Calculation is in progress, please wait…");
            List<string> coldList = new List<string>();
            foreach (CustomStream cs in normalColdInletList)
            {
                coldList.Add(cs.StreamName);
            }
            string content = PROIIFileOperator.getUsableContent(coldList, tempdir);
            SplashScreenManager.SentMsgToScreen("Calculation is in progress, please wait…");
            IFlashCalculate fcalc = ProIIFactory.CreateFlashCalculate(SourceFileInfo.FileVersion);

            double normalduty=UnitConvert.Convert(model.NormalDutyUnit,"KJ/hr",model.NormalDuty);
            normalduty = normalduty / Math.Pow(10, 6);
            string tray1_f = fcalc.Calculate(content, 1, reliefPressure.ToString(), 5, normalduty.ToString(),heathx.HXName, normalColdInletList, vapor, liquid, dirLatent, ref ImportResult, ref RunResult);
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
                    CustomStream liquidcs = ProIIToDefault.ConvertProIIStreamToCustomStream(proIILiquid);
                    CustomStream vaporcs = ProIIToDefault.ConvertProIIStreamToCustomStream(proIIVapor);
                    double latent = vaporcs.SpEnthalpy - liquidcs.SpEnthalpy;
                    //double tcoldbprelief =  double.Parse(flash.TempCalc);//转换单位

                    double tcoldbprelief = UnitConvert.Convert("K", "C", double.Parse(flash.TempCalc));
                    model.LatentPoint = latent;
                    if (latent > 0)
                    {
                        model.ReliefLoad = vaporcs.WeightFlow;
                        if (model.ReliefLoad < 0 )
                            model.ReliefLoad = 0;

                        model.ReliefLoad = UnitConvert.Convert(UOMEnum.MassRate, model.ReliefLoadUnit, model.ReliefLoad);
                        model.ReliefMW = vaporcs.BulkMwOfPhase;
                        model.ReliefPressure = UnitConvert.Convert(UOMEnum.Pressure, model.ReliefPressureUnit,reliefPressure);
                        model.ReliefTemperature = UnitConvert.Convert(UOMEnum.Temperature, model.ReliefTemperatureUnit,vaporcs.Temperature);
                        model.ReliefCpCv = vaporcs.BulkCPCVRatio;
                        model.ReliefZ = vaporcs.VaporZFmKVal;
                    }
                    else
                    {
                        model.ReliefLoad = 0;
                        model.ReliefPressure = reliefPressure;
                        MessageBox.Show("No blockedIn", "Message Box");
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

        private void MethodCritical3()
        {
             double tAvg = 0.5 * (normalColdInlet.Temperature + normalColdOutlet.Temperature);
            double Q = model.NormalDuty;
            double pressure = psv.Pressure;
            double reliefPressure = pressure * psv.ReliefPressureFactor;
            CustomStream cs = normalColdInlet;
            double reliefMW = cs.BulkMwOfPhase;
            double latent = 116;
            GlobalDefaultBLL globalbll = new GlobalDefaultBLL(SessionPF);
            latent = globalbll.GetConditionsSettings().LatentHeatSettings;

            model.LatentPoint = latent;
            if (criticalTemperature == 0)  //相当于超临界临界方法5
            {
                model.ReliefLoad = Q / latent ;
                model.ReliefTemperature = UnitConvert.Convert(UOMEnum.Temperature, model.ReliefTemperatureUnit, normalHotInlet.Temperature);
            }
            else
            {
                model.ReliefLoad = Q / latent * (model.NormalHotTemperature - criticalTemperature) / (model.NormalHotTemperature - tAvg);
                model.ReliefTemperature = UnitConvert.Convert(UOMEnum.Temperature, model.ReliefTemperatureUnit, criticalTemperature);
            }
            model.ReliefLoad = Q / latent;
            if (model.ReliefLoad < 0)
                model.ReliefLoad = 0;
            model.ReliefLoad = UnitConvert.Convert(UOMEnum.MassRate, model.ReliefLoadUnit, model.ReliefLoad);
            model.ReliefPressure = UnitConvert.Convert(UOMEnum.Pressure, model.ReliefPressureUnit, reliefPressure);
            
            model.ReliefMW = reliefMW;

            //cpcv:1.4  z:0.7
            model.ReliefCpCv = cs.BulkCPCVRatio;
            model.ReliefZ = cs.VaporZFmKVal;
        }

        /// <summary>
        /// Coldoutlet 做闪蒸
        /// </summary>
        private void MethodGasExp()
        {            
            CustomStream stream = normalColdOutlet;
            double Q = model.NormalDuty;
            SplashScreenManager.SentMsgToScreen("Calculation is in progress, please wait…");
            string tempdir = DirProtectedSystem + @"\temp\";
            string dirLatent = tempdir + "BlockedInlet_GasExp"+ScenarioID.ToString();
            if (Directory.Exists(dirLatent))
                Directory.Delete(dirLatent, true);
            Directory.CreateDirectory(dirLatent);
            string gd = Guid.NewGuid().ToString();
            string vapor = "S_" + gd.Substring(0, 5).ToUpper();
            string liquid = "S_" + gd.Substring(gd.Length - 5, 5).ToUpper();
            int ImportResult = 0;
            int RunResult = 0;
            PROIIFileOperator.DecompressProIIFile(FileFullPath, tempdir);
            string[] sourceFiles = Directory.GetFiles(tempdir, "*.inp");
            string sourceFile = sourceFiles[0];
            string[] lines = System.IO.File.ReadAllLines(sourceFile);
            HeatMethod = ProIIMethod.GetHeatMethod(lines,heathx.HXName );
            SplashScreenManager.SentMsgToScreen("Calculation is in progress, please wait…");
            string content = PROIIFileOperator.getUsableContent(stream.StreamName, tempdir);
            SplashScreenManager.SentMsgToScreen("Calculation is in progress, please wait…");
            IFlashCalculate fcalc = ProIIFactory.CreateFlashCalculate(SourceFileInfo.FileVersion);

            double temp = UnitConvert.Convert(model.NormalHotTemperatureUnit, "C", model.NormalHotTemperature);
            string tray1_f = fcalc.Calculate(content, 1, reliefPressure.ToString(), 2, temp.ToString(), HeatMethod,stream, vapor, liquid, dirLatent, ref ImportResult, ref RunResult);
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
                    CustomStream liquidcs = ProIIToDefault.ConvertProIIStreamToCustomStream(proIILiquid);
                    CustomStream vaporcs = ProIIToDefault.ConvertProIIStreamToCustomStream(proIIVapor);
                    double latent = vaporcs.SpEnthalpy - liquidcs.SpEnthalpy;
                    //double tcoldbprelief =  double.Parse(flash.TempCalc);//转换单位

                    double tcoldbprelief = UnitConvert.Convert("K", "C", double.Parse(flash.TempCalc));
                    model.LatentPoint = latent;
                    if (latent > 0)
                    {
                        model.ReliefLoad = vaporcs.WeightFlow;
                        if (model.ReliefLoad < 0 )
                            model.ReliefLoad = 0;

                        model.ReliefLoad = UnitConvert.Convert(UOMEnum.MassRate, model.ReliefLoadUnit, model.ReliefLoad);
                        model.ReliefMW = vaporcs.BulkMwOfPhase;
                        model.ReliefPressure = UnitConvert.Convert(UOMEnum.Pressure, model.ReliefPressureUnit,reliefPressure);
                        model.ReliefTemperature = UnitConvert.Convert(UOMEnum.Temperature, model.ReliefTemperatureUnit,vaporcs.Temperature);
                        model.ReliefCpCv = vaporcs.BulkCPCVRatio;
                        model.ReliefZ = vaporcs.VaporZFmKVal;
                    }
                    else
                    {
                        model.ReliefLoad = 0;
                        MessageBox.Show("Flash Calculation failed", "Message Box");
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

        private void MethodBubbleFail()
        {
            //cold_out
            double frac = normalColdOutlet.VaporFraction;
            if (frac > 0)
            {
                MethodCritical3();  //condition7
            }
            else
            {                
                PhaseEnvelopShowView view=new PhaseEnvelopShowView();
                PhaseEnvelopShowVM vm = new PhaseEnvelopShowVM();
                view.DataContext = vm;
                if (view.ShowDialog()==true)
                {
                    if (vm.IsSuperCritical)
                    {
                        MethodCritical3();
                    }
                    else
                    {
                        model.ReliefLoad = 0; //condition11                        
                        model.ReliefLoad = UnitConvert.Convert(UOMEnum.MassRate, model.ReliefLoadUnit, model.ReliefLoad);

                        model.ReliefPressure = UnitConvert.Convert(UOMEnum.Pressure, model.ReliefPressureUnit,reliefPressure);
                        model.ReliefTemperature = UnitConvert.Convert(UOMEnum.Temperature, model.ReliefTemperatureUnit,normalColdInlet.Temperature);
                        model.ReliefCpCv = normalColdInlet.BulkCPCVRatio;
                        model.ReliefZ = normalColdInlet.VaporZFmKVal;
                        model.ReliefMW = normalColdInlet.BulkMwOfPhase;
                    }
                    
                }




                //自定义部分了。
            }
        }

        private int CalcBlockedInCriticalPressure(List<string> lstFeeds)
        {
            int result = 0;
            if (lstFeeds.Count == 0)
            {
                result = 2;
            }
            else
            {
                List<CustomStream> csFeeds = new List<CustomStream>();
                CustomStreamDAL csdal = new CustomStreamDAL();
                foreach (string s in lstFeeds)
                {
                    CustomStream cs = csdal.GetModel(SessionPS, s);
                    csFeeds.Add(cs);
                }

                string tempdir = DirProtectedSystem + @"\temp\";
                string dirPhase = tempdir + "Fire" + ScenarioID.ToString() + "_Phase";
                if (Directory.Exists(dirPhase))
                    Directory.Delete(dirPhase, true);
                Directory.CreateDirectory(dirPhase);
                CustomStream stream = new CustomStream();
                stream = csFeeds[0];
                string[] streamComps = stream.TotalComposition.Split(',');
                int len = streamComps.Length;
                double[] streamCompValues = new double[len];
                double sumTotalMolarRate = 0;
                foreach (CustomStream cs in csFeeds)
                {
                    sumTotalMolarRate = sumTotalMolarRate + cs.TotalMolarRate;
                }
                foreach (CustomStream cs in csFeeds)
                {
                    string[] comps = cs.TotalComposition.Split(',');
                    for (int i = 0; i < len; i++)
                    {
                        streamCompValues[i] = streamCompValues[i] + double.Parse(comps[i]) * cs.TotalMolarRate / sumTotalMolarRate;
                    }
                }
                StringBuilder sumComposition = new StringBuilder();
                foreach (double comp in streamCompValues)
                {
                    sumComposition.Append(",").Append(comp.ToString());
                }
               
                stream.TotalComposition = sumComposition.ToString().Substring(1);
                double internPressure = UnitConvert.Convert("MPAG", "KPA", stream.Pressure);

                string phasecontent = PROIIFileOperator.getUsablePhaseContent(stream.StreamName, tempdir);
                double ReliefPressure = 1;
                result = CalcCriticalPressure(phasecontent, ReliefPressure, stream, dirPhase);

            }
            if (result == 2)
            {
                MessageBox.Show("Critical point NOT determined, please check result carefully.", "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return result;
        }
       
        private int CalcCriticalPressure(string content, double ReliefPressure, CustomStream stream, string dirPhase)
        {
            int ImportResult = 0;
            int RunResult = 0;
            int result = 0;
            IPHASECalculate PhaseCalc = ProIIFactory.CreatePHASECalculate(SourceFileInfo.FileVersion);
            string PH = "PH" + Guid.NewGuid().ToString().Substring(0, 4);
            string criticalPress = string.Empty;
            string criticalTemp = string.Empty;
            string cricondenbarPress = string.Empty;
            string cricondenbarTemp = string.Empty;

            string phasef = PhaseCalc.Calculate(content, 1, ReliefPressure.ToString(), 4, "", stream, PH, dirPhase, ref ImportResult, ref RunResult);
            if (ImportResult == 1 || ImportResult == 2)
            {
                if (RunResult == 1 || RunResult == 2)
                {
                    IProIIReader reader = ProIIFactory.CreateReader(SourceFileInfo.FileVersion);
                    reader.InitProIIReader(phasef);
                    criticalPress = reader.GetCriticalPressure(PH);
                    criticalTemp = reader.GetCriticalTemperature(PH);
                    cricondenbarPress = reader.GetCricondenbarPress(PH);
                    cricondenbarTemp = reader.GetCricondenbarTemp(PH);
                    reader.ReleaseProIIReader();

                    if (string.IsNullOrEmpty(criticalPress) || double.Parse(criticalPress) <= 0)
                    {
                        result = 2;
                    }
                    else
                    {
                        criticalPressure = UnitConvert.Convert("KPa", UOMEnum.Pressure, double.Parse(criticalPress));
                        criticalTemperature =  UnitConvert.Convert("K", "C", double.Parse(criticalTemp)); 
                        cricondenbarPressure = UnitConvert.Convert("KPa", UOMEnum.Pressure, double.Parse(cricondenbarPress));
                        cricondenbarTemperature = UnitConvert.Convert("K", "C", double.Parse(cricondenbarTemp)); 

                        result = 1;
                    }
                    return result;
                }
                else
                {
                    //MessageBox.Show("The simulation unsolved!", "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return 3;
                }
            }
            else
            {
                //MessageBox.Show("There is some errors in keyword file.", "Message Box", MessageBoxButton.OK, MessageBoxImage.Error);
                return 4;
            }

        }

        private void Save(object obj)
        {
            //if (!model.CheckData()) return; 
            if (obj != null)
            {
                System.Windows.Window wd = obj as System.Windows.Window;
                if (wd != null)
                {
                    WriteConvertModel();
                    hxBLL.SaveHXBlockedOutlet(model.dbmodel);
                    wd.DialogResult = true;
                }
            }
        }
    }
}
