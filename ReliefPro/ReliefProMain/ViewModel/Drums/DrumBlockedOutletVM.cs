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
        SourceFile SourceFileInfo { set; get; }
        double reliefLoad = 0, reliefMW = 0, reliefT = 0, reliefPressure = 0, reliefCpCv = 0, reliefZ = 0;
        public Tuple<double, double, double, double> CalcTuple { get; set; }
        public DrumBlockedOutletVM(int ScenarioID, SourceFile sourceFileInfo, ISession SessionPS, ISession SessionPF, string dirPlant, string dirProtectedSystem)
        {
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;
            SourceFileInfo = sourceFileInfo;
            DirPlant = dirPlant;
            DirProtectedSystem = dirProtectedSystem;
            drum = new DrumBLL();


            var outletModel = drum.GetBlockedOutletModel(SessionPS);
            outletModel = drum.ReadConvertModel(outletModel, SessionPF);

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

            model.ReliefLoad = sModel.ReliefLoad;
            model.ReliefPressure = sModel.ReliefPressure;
            model.ReliefTemperature = sModel.ReliefTemperature;
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
            try
            {
                SplashScreenManager.Show();
                if (!model.CheckData()) return;
                SplashScreenManager.SentMsgToScreen("Calculation is in progress, please wait…");
                reliefPressure = drum.ScenarioReliefPressure(SessionPS);
                string vapor = "V_" + Guid.NewGuid().ToString().Substring(0, 6);
                string liquid = "L_" + Guid.NewGuid().ToString().Substring(0, 6);
                string tempdir = DirProtectedSystem + @"\BlockedOutlet";
                if (!Directory.Exists(tempdir))
                {
                    Directory.CreateDirectory(tempdir);
                }
                string duty = "0";
                double feedupPress = model.MaxPressure;
                double setPress = drum.PSet(SessionPS);
                if (feedupPress > setPress)
                {
                    SplashScreenManager.SentMsgToScreen("Calculation is in progress, please wait…");
                    PSVDAL psvdal = new PSVDAL();
                    PSV psv = psvdal.GetModel(SessionPS);
                    if (reliefPressure > psv.CriticalPressure)
                    {
                        SplashScreenManager.SentMsgToScreen("Calculation is in progress, please wait…");
                        CustomStream cs = drum.Feeds[0];
                        model.ReliefLoad = 116;
                        model.ReliefPressure = reliefPressure;
                        model.ReliefTemperature = psv.CriticalTemperature;
                        model.ReliefMW = reliefMW;
                        model.ReliefCpCv = cs.BulkCPCVRatio;
                        model.ReliefZ = cs.VaporZFmKVal;
                        if (model.ReliefLoad < 0)
                            model.ReliefLoad = 0;
                    }
                    else
                    {
                        string dir = DirPlant + @"\" + SourceFileInfo.FileNameNoExt;
                        SplashScreenManager.SentMsgToScreen("Calculation is in progress, please wait…");
                        string content = PROIIFileOperator.getUsableContent(drum.Feeds[0].StreamName, dir);
                        if (model.DrumType == "Flashing Drum")
                        {
                            duty = (model.NormalFlashDuty / Math.Pow(10, 6)).ToString();
                        }
                        SplashScreenManager.SentMsgToScreen("Calculation is in progress, please wait…");
                        IFlashCalculate flashcalc = ProIIFactory.CreateFlashCalculate(SourceFileInfo.FileVersion);
                        int ImportResult = 0;
                        int RunResult = 0;
                        string f = flashcalc.Calculate(content, 1, reliefPressure.ToString(), 5, duty, drum.Feeds[0], vapor, liquid, tempdir, ref ImportResult, ref RunResult);
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
                                model.ReliefLoad = reliefLoad;
                                model.ReliefPressure = reliefPressure;
                                model.ReliefTemperature = reliefT;
                                model.ReliefMW = reliefMW;
                                model.ReliefCpCv = cs.BulkCPCVRatio;
                                model.ReliefZ = cs.VaporZFmKVal;
                                if (model.ReliefLoad < 0)
                                    model.ReliefLoad = 0;
                            }
                            else
                            {
                                MessageBox.Show("Prz file is error", "Message Box");
                                return;
                            }
                        }
                        else
                        {
                            MessageBox.Show("inp file is error", "Message Box");
                            return;
                        }
                    }
                }
                else
                {
                    reliefLoad = 0;
                    model.ReliefLoad = reliefLoad;
                    model.ReliefPressure = reliefPressure;
                    model.ReliefTemperature = reliefT;
                    model.ReliefMW = reliefMW;
                    model.ReliefCpCv = 0;
                    model.ReliefZ = 0;
                }
                SplashScreenManager.SentMsgToScreen("Calculation finished");
            }
            catch
            { }
            finally
            {
                SplashScreenManager.Close();
            }
            

        }

        private void Save(object obj)
        {
            if (!model.CheckData()) return;
            WriteConvertModel();
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
