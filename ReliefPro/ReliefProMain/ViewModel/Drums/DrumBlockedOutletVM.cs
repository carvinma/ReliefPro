﻿/*
 * drum 的出口堵塞
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using ReliefProBLL.Common;
using ReliefProDAL;
using ReliefProModel;
using ReliefProModel.Drums;
using ReliefProBLL;
using ReliefProCommon.CommonLib;
using System.Windows.Input;
using ReliefProMain.Commands;
using ProII;
using ReliefProMain.Models;
using UOMLib;
using System.Diagnostics;
using NHibernate;

namespace ReliefProMain.ViewModel.Drums
{
    public class DrumBlockedOutletVM : ViewModelBase
    {
        private DrumBLL drum;
        public DrumBlockedOutletModel model { get; set; }
        public ICommand CalcCMD { get; set; }
        public ICommand OKCMD { get; set; }

        private ISession SessionPS;
        private ISession SessionPF;
        private string DirPlant { set; get; }
        private string DirProtectedSystem { set; get; }
        string dirMix;
        string mixPrzFile;
        private List<CustomStream> mixFeeds;
        CustomStream mixCSProduct;
        SourceFile SourceFileInfo { set; get; }
        double reliefLoad = 0, reliefMW = 0, reliefT = 0, reliefPressure = 0, reliefCpCv = 0, reliefZ = 0;
        public Tuple<double, double, double, double> CalcTuple { get; set; }
        public int ScenarioID;
        private int EqType;
        public int IsHasBlockedOutlet;

        public string HeatMethod;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ScenarioID"></param>
        /// <param name="sourceFileInfo"></param>
        /// <param name="SessionPS"></param>
        /// <param name="SessionPF"></param>
        /// <param name="dirPlant"></param>
        /// <param name="dirProtectedSystem"></param>
        /// <param name="EqType">0:drum 1:hx</param>
        public DrumBlockedOutletVM(int ScenarioID, SourceFile sourceFileInfo, ISession SessionPS, ISession SessionPF, string dirPlant, string dirProtectedSystem,int EqType)
        {
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;
            SourceFileInfo = sourceFileInfo;
            DirPlant = dirPlant;
            DirProtectedSystem = dirProtectedSystem;
            this.EqType = EqType;
            drum = new DrumBLL();
            this.ScenarioID = ScenarioID;
            mixFeeds = new List<CustomStream>();
            mixCSProduct = new CustomStream();
            dirMix = DirProtectedSystem + @"\mix" + ScenarioID.ToString();
            mixPrzFile = dirMix + @"\a.prz";
            var outletModel = drum.GetBlockedOutletModel(SessionPS, ScenarioID,EqType);
            double setPress = drum.PSet(SessionPS);
            if (drum.Feeds.Count > 1 && string.IsNullOrEmpty(outletModel.MixProductName))
            {
                
                string sbcontent = string.Empty;
                List<string> strFeeds = new List<string>();
                SourceDAL sourcedal = new SourceDAL();
                double minPressure = 100000;
                foreach (CustomStream cs in drum.Feeds)
                {
                    Source sr = sourcedal.GetModel(cs.StreamName, SessionPS);
                    if (sr.MaxPossiblePressure >= setPress)
                    {
                        strFeeds.Add(cs.StreamName);
                        //cs.Pressure = sr.MaxPossiblePressure;  //改为使用本身的压力了。
                        mixFeeds.Add(cs);
                        if (sr.MaxPossiblePressure < minPressure)
                            minPressure = sr.MaxPossiblePressure;
                    }
                }
                if (strFeeds.Count==0)
                {
                    IsHasBlockedOutlet = 1; //表示没有出口堵塞
                    return;
                }


                outletModel.MaxPressure = minPressure;
                string dir = DirPlant + @"\" + SourceFileInfo.FileNameNoExt;
                string[] sourceFiles = Directory.GetFiles(dir, "*.inp");
                string sourceFile = sourceFiles[0];
                string[] lines = System.IO.File.ReadAllLines(sourceFile);
                sbcontent = PROIIFileOperator.getUsableContent(strFeeds, lines);
                IMixCalculate mixcalc = ProIIFactory.CreateMixCalculate(SourceFileInfo.FileVersion);
                outletModel.MixProductName = Guid.NewGuid().ToString().Substring(0, 6);
                
                if (Directory.Exists(dirMix))
                {
                    Directory.Delete(dirMix, true);
                }
                Directory.CreateDirectory(dirMix);
                int mixImportResult = 1;
                int mixRunResult = 1;
                mixPrzFile = mixcalc.Calculate(sbcontent, mixFeeds, outletModel.MixProductName, dirMix, ref mixImportResult, ref mixRunResult);

                if (mixImportResult == 1 || mixImportResult == 2)
                {
                    if (mixRunResult == 1 || mixRunResult == 2)
                    {
                        IProIIReader reader = ProIIFactory.CreateReader(SourceFileInfo.FileVersion);
                        reader.InitProIIReader(mixPrzFile);
                        ProIIStreamData proIIvapor = reader.GetSteamInfo(outletModel.MixProductName);
                        reader.ReleaseProIIReader();
                        mixCSProduct = ProIIToDefault.ConvertProIIStreamToCustomStream(proIIvapor);
                        
                        //outletModel.MaxPressure = mixCSProduct.Pressure;  //2015.1.6 修改其读取方法，不再从mix里读取。而是通过和定压比较取最小的压力。
                        outletModel.MaxStreamRate = mixCSProduct.WeightFlow;
                    }
                    else
                    {
                        MessageBox.Show("Inp file is error", "Message Box");
                        //return;
                    }
                }
                else
                {
                    //MessageBox.Show("Prz file is error", "Message Box");
                    return;
                }
            }
            if (outletModel.MaxPressure == 100000 || outletModel.MaxPressure <= setPress)
            {
                //MessageBox.Show("No blocked outlet", "Message Box");
                IsHasBlockedOutlet = 1;
                return;
            }

            outletModel = drum.ReadConvertModel(outletModel, SessionPF);//转换为用户单位
            model = new DrumBlockedOutletModel(outletModel);
            model.dbmodel.DrumID = drum.GetDrumID(SessionPS);
            model.dbmodel.ScenarioID = ScenarioID;
            CalcCMD = new DelegateCommand<object>(CalcResult);
            OKCMD = new DelegateCommand<object>(Save);
            UOMLib.UOMEnum uomEnum = UOMSingle.UomEnums.FirstOrDefault(p => p.SessionPlant == this.SessionPF);
            model.PressureUnit = uomEnum.UserPressure;
            model.StreamRateUnit = uomEnum.UserMassRate;
            model.FlashingDutyUnit = uomEnum.UserEnthalpyDuty;
            model.ReliefConditionUnit = uomEnum.UserEnthalpyDuty;

            model.ReliefloadUnit = uomEnum.UserMassRate;
            model.ReliefTempUnit = uomEnum.UserTemperature;
            model.ReliefPressureUnit = uomEnum.UserPressure;

            ScenarioDAL db = new ScenarioDAL();
            var sModel = db.GetModel(ScenarioID, SessionPS);

            model.ReliefLoad = UnitConvert.Convert(UOMEnum.MassRate, model.ReliefloadUnit, sModel.ReliefLoad);
            model.ReliefPressure = UnitConvert.Convert(UOMEnum.Pressure, model.ReliefPressureUnit, sModel.ReliefPressure);
            model.ReliefTemperature = UnitConvert.Convert(UOMEnum.Temperature, model.ReliefTempUnit, sModel.ReliefTemperature);
            model.ReliefMW = sModel.ReliefMW;
            model.ReliefCpCv = sModel.ReliefCpCv;
            model.ReliefZ = sModel.ReliefZ;
        }
        private void WriteConvertModel()
        {
            model.dbmodel.MaxPressure = UnitConvert.Convert(model.PressureUnit, UOMLib.UOMEnum.Pressure.ToString(), model.MaxPressure);
            model.dbmodel.MaxStreamRate = UnitConvert.Convert(model.StreamRateUnit, UOMLib.UOMEnum.MassRate.ToString(), model.MaxStreamRate);
            model.dbmodel.NormalFlashDuty = UnitConvert.Convert(model.FlashingDutyUnit, UOMLib.UOMEnum.EnthalpyDuty.ToString(), model.NormalFlashDuty);
            model.dbmodel.FDReliefCondition = UnitConvert.Convert(model.ReliefConditionUnit, UOMLib.UOMEnum.EnthalpyDuty.ToString(), model.FDReliefCondition);
            model.dbmodel.ReboilerPinch = model.ReboilerPinch;
            model.dbmodel.Feed = model.Feed;

            model.ReliefLoad = UnitConvert.Convert(model.ReliefloadUnit, UOMLib.UOMEnum.MassRate.ToString(), model.ReliefLoad);
            model.ReliefTemperature = UnitConvert.Convert(model.ReliefTempUnit, UOMLib.UOMEnum.Temperature.ToString(), model.ReliefTemperature);
            model.ReliefPressure = UnitConvert.Convert(model.ReliefPressureUnit, UOMLib.UOMEnum.Pressure.ToString(), model.ReliefPressure);

        }
        private void CalcResult(object obj)
        {
            int errorType = 0;
            try
            {
                SplashScreenManager.Show();
                //if (!model.CheckData()) return;
                SplashScreenManager.SentMsgToScreen("Calculation is in progress, please wait…");
                reliefPressure = drum.ScenarioReliefPressure(SessionPS);
                string vapor = "V_" + Guid.NewGuid().ToString().Substring(0, 6);
                string liquid = "L_" + Guid.NewGuid().ToString().Substring(0, 6);
                string tempdir = DirProtectedSystem + @"\BlockedOutlet"+ScenarioID.ToString();
                if (Directory.Exists(tempdir))
                {
                    Directory.Delete(tempdir,true);
                }
                Directory.CreateDirectory(tempdir);
                string duty = "0";
                double feedupPress = model.MaxPressure;
                double setPress = drum.PSet(SessionPS);
                if (feedupPress > setPress)
                {
                    SplashScreenManager.SentMsgToScreen("Calculation is in progress, please wait…");
                    PSVDAL psvdal = new PSVDAL();

                    string dir = DirPlant + @"\" + SourceFileInfo.FileNameNoExt;
                    SplashScreenManager.SentMsgToScreen("Calculation is in progress, please wait…");
                    string[] files = Directory.GetFiles(dir, "*.inp");
                    string sourceFile = files[0];
                    string[] lines = System.IO.File.ReadAllLines(sourceFile);
                    HeatMethod = ProIIMethod.GetHeatMethod(lines, drum.EqName);
                    string content = string.Empty;
                    if (drum.Feeds.Count == 1)
                    {
                        content = PROIIFileOperator.getUsableContent(drum.Feeds[0].StreamName, lines);
                    }
                    else
                    {
                        bool b = PROIIFileOperator.DecompressProIIFile(mixPrzFile, dirMix);
                        string[] mixfiles = Directory.GetFiles(dirMix, "*.inp");
                        string mixsourceFile = mixfiles[0];
                        string[] mixlines = System.IO.File.ReadAllLines(mixsourceFile);
                        content = PROIIFileOperator.getUsableContent(model.MixProductName, mixlines);
                    }
                    if (model.DrumType == "Flashing Drum")
                    {
                        duty = (model.FDReliefCondition / Math.Pow(10, 6)).ToString();
                    }
                    if (EqType == 1)
                    {
                        duty = (model.FDReliefCondition / Math.Pow(10, 6)).ToString(); 
                    }
                    
                    SplashScreenManager.SentMsgToScreen("Calculation is in progress, please wait…");
                    IFlashCalculate flashcalc = ProIIFactory.CreateFlashCalculate(SourceFileInfo.FileVersion);
                    int ImportResult = 0;
                    int RunResult = 0;
                    string f = string.Empty;
                    if (drum.Feeds.Count == 1)
                    {      
                        //drum.Feeds[0].Pressure=UnitConvert.Convert(model.PressureUnit,"Mpag",model.MaxPressure);
                        double weightFlow=UnitConvert.Convert(model.StreamRateUnit,"Kg/hr",model.MaxStreamRate);
                        drum.Feeds[0].TotalMolarRate=weightFlow/3600/drum.Feeds[0].BulkMwOfPhase;
                        f = flashcalc.Calculate(content, 1, reliefPressure.ToString(), 5, duty, HeatMethod,drum.Feeds[0], vapor, liquid, tempdir, ref ImportResult, ref RunResult);
                    }
                    else
                    {
                        IProIIReader reader = ProIIFactory.CreateReader(SourceFileInfo.FileVersion);
                        reader.InitProIIReader(mixPrzFile);
                        ProIIStreamData proIIvapor = reader.GetSteamInfo(model.MixProductName);
                        reader.ReleaseProIIReader();
                        mixCSProduct = ProIIToDefault.ConvertProIIStreamToCustomStream(proIIvapor);
                        //mixCSProduct.Pressure = UnitConvert.Convert(model.PressureUnit, "Mpag", model.MaxPressure);
                        double weightFlow = UnitConvert.Convert(model.StreamRateUnit, "Kg/hr", model.MaxStreamRate);
                        mixCSProduct.TotalMolarRate = weightFlow / 3600 / mixCSProduct.BulkMwOfPhase;
                        f = flashcalc.Calculate(content, 1, reliefPressure.ToString(), 5, duty, HeatMethod, mixCSProduct, vapor, liquid, tempdir, ref ImportResult, ref RunResult);
                    }

                    if (ImportResult == 1 || ImportResult == 2)
                    {
                        if (RunResult == 1 || RunResult == 2)
                        {
                            IProIIReader reader = ProIIFactory.CreateReader(SourceFileInfo.FileVersion);
                            reader.InitProIIReader(f);
                            ProIIStreamData proIIvapor = reader.GetSteamInfo(vapor);
                            reader.ReleaseProIIReader();
                            CustomStream cs = ProIIToDefault.ConvertProIIStreamToCustomStream(proIIvapor);

                            reliefMW = cs.BulkMwOfPhase;
                            reliefT = cs.Temperature;
                            reliefLoad = cs.WeightFlow;
                            model.ReliefLoad = UnitConvert.Convert(UOMEnum.MassRate,model.ReliefloadUnit, reliefLoad);
                            model.ReliefPressure = UnitConvert.Convert(UOMEnum.Pressure,model.ReliefPressureUnit,reliefPressure);
                            model.ReliefTemperature =  UnitConvert.Convert(UOMEnum.Temperature,model.ReliefTempUnit,reliefT);
                            model.ReliefMW = reliefMW;
                            model.ReliefCpCv = cs.BulkCPCVRatio;
                            model.ReliefZ = cs.VaporZFmKVal;
                            if (model.ReliefLoad < 0)
                                model.ReliefLoad = 0;
                        }
                        else
                        {
                            //MessageBox.Show("Prz file is error", "Message Box");
                            errorType = 1;
                        }
                    }
                    else
                    {
                        //MessageBox.Show("inp file is error", "Message Box");
                        errorType = 2;
                    }

                }
                else
                {
                    reliefLoad = 0;
                    model.ReliefLoad = 0;
                    model.ReliefPressure = reliefPressure;
                    model.ReliefTemperature = 0;
                    model.ReliefMW = 0;
                    model.ReliefCpCv = 0;
                    model.ReliefZ = 0;
                    model.ReliefPressure = UnitConvert.Convert(UOMEnum.Pressure, model.ReliefPressureUnit, reliefPressure);
                    MessageBox.Show("Source Pressure is less than set pressure,no relief occurs.","Message Box",MessageBoxButton.OK,MessageBoxImage.Warning);
                }
                SplashScreenManager.SentMsgToScreen("Calculation finished");
            }
            catch
            { }
            finally
            {
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
            

        }

        private void Save(object obj)
        {
            //if (!model.CheckData()) return;
            if (model.MaxPressure == 0)
            {
                MessageBox.Show("Max source pressure must be greater than zero", "Message Box",MessageBoxButton.OK,MessageBoxImage.Warning);
                return;
            }
            if (model.MaxStreamRate == 0)
            {
                MessageBox.Show("Max source stream rate must be greater than zero", "Message Box", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            WriteConvertModel();
            reliefLoad = model.ReliefLoad;
            reliefMW = model.ReliefMW;
            reliefT = model.ReliefTemperature;
            reliefCpCv = model.ReliefCpCv;
            reliefZ = model.ReliefZ;
            reliefPressure = model.ReliefPressure;
            CalcTuple = new Tuple<double, double, double, double>(reliefLoad, reliefMW, reliefT, reliefPressure);            
            drum.SaveDrumBlockedOutlet(model.dbmodel, SessionPS, reliefLoad, reliefMW, reliefT, reliefCpCv, reliefZ);
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
