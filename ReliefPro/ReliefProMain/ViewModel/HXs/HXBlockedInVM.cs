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
        CustomStream normalColdOutlet = new CustomStream();
        PSVDAL psvDAL ;
        PSV psv;
        double reliefPressure;
        public HXBlockedInVM(int ScenarioID, SourceFile sourceFileInfo, ISession SessionPS, ISession SessionPF, string dirPlant, string dirProtectedSystem)
        {
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;
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
            
           
            HeatExchangerDAL heatexdal=new HeatExchangerDAL();
            HeatExchanger heathx=heatexdal.GetModel(SessionPS);
            if (heathx.ColdInlet.Contains(","))
            {
                //需要做mixer
                int ImportResult = 0;
                int RunResult = 0;
                string tempdir = DirProtectedSystem + @"\temp\";
                string dirMix = tempdir + "BlockedInlet_Mix";
                if (!Directory.Exists(dirMix))
                {
                    Directory.CreateDirectory(dirMix);
                }
                string[] coldfeeds = heathx.ColdInlet.Split(',');
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
                    }
                }
            }
            else
            {
                normalColdInlet = csdal.GetModel(SessionPS,heathx.ColdInlet);
                
            }
            normalColdOutlet= csdal.GetModel(SessionPS,heathx.ColdOutlet);
            normalHotInlet = csdal.GetModel(SessionPS, heathx.HotInlet);

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
                SplashScreenManager.Show();
                SplashScreenManager.SentMsgToScreen("Calculation is in progress, please wait…");
 
                double pressure = psv.Pressure;
                reliefPressure = pressure * psv.ReliefPressureFactor;

                if (normalColdInlet.VaporFraction == 1)
                {
                    // gas expansion
                    MethodGasExp();
                }
                else
                {
                    double critalcalPress =UnitConvert.Convert("MPAG","Kpa", psv.CriticalPressure);
                    if (critalcalPress == 0)
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
                        if (reliefPressure < psv.CriticalPressure)
                        {
                            //flash bubble
                           bool b= MethodBubble1();
                           if (!b)
                           {
                               if (normalColdInlet.VaporFraction == 0)
                               {
                                   MethodBubbleFail();
                               }
                               else
                               {
                                   MethodDuty2();
                               }
                           }
                        }
                        else
                        {
                            if (reliefPressure < psv.CricondenbarPress && psv.CriticalTemperature > psv.CricondenbarTemp)
                            {
                               bool b= MethodBubble1();
                               if (!b)
                               {
                                   MethodBubbleFail();
                               }
                            }
                            else
                            {
                                MethodCritical3();
                            }
                        }
                    }
                }


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
            string dirLatent = tempdir + "BlockedIn";
            if (!Directory.Exists(dirLatent))
                Directory.CreateDirectory(dirLatent);
            string gd = Guid.NewGuid().ToString();
            string vapor = "S_" + gd.Substring(0, 5).ToUpper();
            string liquid = "S_" + gd.Substring(gd.Length - 5, 5).ToUpper();
            int ImportResult = 0;
            int RunResult = 0;
            PROIIFileOperator.DecompressProIIFile(FileFullPath, tempdir);
            SplashScreenManager.SentMsgToScreen("Calculation is in progress, please wait…");
            List<string> coldList = new List<string>();
            foreach (CustomStream cs in normalColdInletList)
            {
                coldList.Add(cs.StreamName);
            }
            string content = PROIIFileOperator.getUsableContent(coldList, tempdir);
            SplashScreenManager.SentMsgToScreen("Calculation is in progress, please wait…");
            IFlashCalculate fcalc = ProIIFactory.CreateFlashCalculate(SourceFileInfo.FileVersion);
            string tray1_f = fcalc.Calculate(content, 1, reliefPressure.ToString(), 3, "0", normalColdInletList, vapor, liquid, dirLatent, ref ImportResult, ref RunResult);
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
                    MessageBox.Show("Prz file is error", "Message Box");
                    return false;
                }
            }
            else
            {
                MessageBox.Show("inp file is error", "Message Box");
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
            string dirLatent = tempdir + "BlockedIn";
            if (!Directory.Exists(dirLatent))
                Directory.CreateDirectory(dirLatent);
            string gd = Guid.NewGuid().ToString();
            string vapor = "S_" + gd.Substring(0, 5).ToUpper();
            string liquid = "S_" + gd.Substring(gd.Length - 5, 5).ToUpper();
            int ImportResult = 0;
            int RunResult = 0;
            PROIIFileOperator.DecompressProIIFile(FileFullPath, tempdir);
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
            string tray1_f = fcalc.Calculate(content, 1, reliefPressure.ToString(), 5, normalduty.ToString(), normalColdInletList, vapor, liquid, dirLatent, ref ImportResult, ref RunResult);
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
                        model.ReliefMW = vaporcs.BulkMwOfPhase;
                        model.ReliefPressure = reliefPressure;
                        model.ReliefTemperature = vaporcs.Temperature;
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

        private void MethodCritical3()
        {
             double tAvg = 0.5 * (normalColdInlet.Temperature + normalColdOutlet.Temperature);
            double Q = model.NormalDuty;
            double pressure = psv.Pressure;
            double reliefPressure = pressure * psv.ReliefPressureFactor;
            CustomStream cs = normalColdInlet;
            double reliefMW = cs.BulkMwOfPhase;
            double latent = 116;
            model.ReliefLoad = Q/latent+(model.NormalHotTemperature-psv.CriticalTemperature)/(model.NormalHotTemperature-tAvg);
            model.ReliefPressure = reliefPressure;
            model.ReliefTemperature = psv.CriticalTemperature;
            model.ReliefMW = reliefMW;
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
            string dirLatent = tempdir + "BlockedInlet_GasExp";
            if (!Directory.Exists(dirLatent))
                Directory.CreateDirectory(dirLatent);
            string gd = Guid.NewGuid().ToString();
            string vapor = "S_" + gd.Substring(0, 5).ToUpper();
            string liquid = "S_" + gd.Substring(gd.Length - 5, 5).ToUpper();
            int ImportResult = 0;
            int RunResult = 0;
            PROIIFileOperator.DecompressProIIFile(FileFullPath, tempdir);
            SplashScreenManager.SentMsgToScreen("Calculation is in progress, please wait…");
            string content = PROIIFileOperator.getUsableContent(stream.StreamName, tempdir);
            SplashScreenManager.SentMsgToScreen("Calculation is in progress, please wait…");
            IFlashCalculate fcalc = ProIIFactory.CreateFlashCalculate(SourceFileInfo.FileVersion);

            double temp = UnitConvert.Convert(model.NormalHotTemperatureUnit, "C", model.NormalHotTemperature);
            string tray1_f = fcalc.Calculate(content, 1, reliefPressure.ToString(), 2, temp.ToString(), stream, vapor, liquid, dirLatent, ref ImportResult, ref RunResult);
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
                        model.ReliefMW = vaporcs.BulkMwOfPhase;
                        model.ReliefPressure = reliefPressure;
                        model.ReliefTemperature = vaporcs.Temperature;
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
            CustomStreamBLL csbll=new CustomStreamBLL(SessionPF,SessionPS);
            IList<CustomStream> list = csbll.GetStreams(SessionPS, true);
            CustomStream coldout=list[0];
            if(list.Count==2)
            {
                if (coldout.Temperature > list[1].Temperature)
                {
                    coldout = list[1];
                }
            }

            double frac = coldout.VaporFraction;
            if (frac > 0)
            {
                MethodCritical3();
            }
            else
            {
                model.ReliefLoad = 0;
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
