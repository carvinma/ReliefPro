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
using ReliefProMain.Model.HXs;
using UOMLib;
using ReliefProModel;
using ReliefProCommon.CommonLib;
using ProII;
using ReliefProDAL;

namespace ReliefProMain.ViewModel.HXs
{
    public class HXBlockedOutletVM : ViewModelBase
    {
        public ICommand CalcCMD { get; set; }
        public ICommand OKCMD { get; set; }
        private ISession SessionPS;
        private ISession SessionPF;
        private string DirPlant;
        private string DirProtectedSystem;
        public SourceFile SourceFileInfo { get; set; }
        public string FileFullPath { get; set; }
        public HXBlockedOutletModel model { get; set; }
        private HXBLL hxBLL;
        public HXBlockedOutletVM(int ScenarioID, SourceFile sourceFileInfo, ISession SessionPS, ISession SessionPF, string dirPlant, string dirProtectedSystem)
        {
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;
            DirPlant = dirPlant;
            DirProtectedSystem = dirProtectedSystem;
            DirPlant = dirPlant;
            DirProtectedSystem = dirProtectedSystem;
            SourceFileInfo = sourceFileInfo;
            FileFullPath = DirPlant + @"\" + sourceFileInfo.FileNameNoExt + @"\" + sourceFileInfo.FileName;
            OKCMD = new DelegateCommand<object>(Save);
            CalcCMD = new DelegateCommand<object>(CalcResult);

            hxBLL = new HXBLL(SessionPS, SessionPF);
            var blockModel = hxBLL.GetHXBlockedOutletModel(ScenarioID);
            blockModel = hxBLL.ReadConvertHXBlockedOutletModel(blockModel);

            model = new HXBlockedOutletModel(blockModel);
            model.dbmodel.ScenarioID = ScenarioID;


            UOMLib.UOMEnum uomEnum = new UOMEnum(SessionPF);
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
            model.dbmodel.NormalHotTemperature = UnitConvert.Convert(model.NormalHotTemperatureUnit, UOMLib.UOMEnum.EnthalpyDuty.ToString(), model.NormalHotTemperature);
            model.dbmodel.NormalColdInletTemperature = UnitConvert.Convert(model.NormalColdInletTemperatureUnit, UOMLib.UOMEnum.EnthalpyDuty.ToString(), model.NormalColdInletTemperature);
            model.dbmodel.NormalColdOutletTemperature = UnitConvert.Convert(model.NormalColdOutletTemperatureUnit, UOMLib.UOMEnum.EnthalpyDuty.ToString(), model.NormalColdOutletTemperature);
            model.dbmodel.LatentPoint = UnitConvert.Convert(model.LatentPointUnit, UOMLib.UOMEnum.SpecificEnthalpy.ToString(), model.LatentPoint);
            model.dbmodel.ReliefMW = model.ReliefMW;
            model.dbmodel.ReliefLoad = UnitConvert.Convert(model.ReliefLoadUnit, UOMLib.UOMEnum.MassRate.ToString(), model.ReliefLoad.Value);
            model.dbmodel.ReliefTemperature = UnitConvert.Convert(model.ReliefTemperatureUnit, UOMLib.UOMEnum.Temperature.ToString(), model.ReliefTemperature);
            model.dbmodel.ReliefPressure = UnitConvert.Convert(model.ReliefPressureUnit, UOMLib.UOMEnum.Pressure.ToString(), model.ReliefPressure);
        }
        private void CalcResult(object obj)
        {
            if (!model.CheckData()) return; 
            double Q = 0;

            CustomStream normalHotInlet = new CustomStream();
            CustomStream normalColdInlet = new CustomStream();
            CustomStream normalColdOutlet = new CustomStream();
            double tAvg = 0.5 * (normalColdInlet.Temperature.Value + normalColdOutlet.Temperature.Value);

            PSVDAL psvDAL = new PSVDAL();
            PSV psv = psvDAL.GetModel(SessionPS);
            double pressure = psv.Pressure.Value;

            double reliefFirePressure = pressure * psv.ReliefPressureFactor.Value;

            CustomStream stream = normalColdInlet;
            string tempdir = DirProtectedSystem + @"\temp\";
            string dirLatent = tempdir + "BlockedOutlet";
            if (!Directory.Exists(dirLatent))
                Directory.CreateDirectory(dirLatent);
            string gd = Guid.NewGuid().ToString();
            string vapor = "S_" + gd.Substring(0, 5).ToUpper();
            string liquid = "S_" + gd.Substring(gd.Length - 5, 5).ToUpper();
            int ImportResult = 0;
            int RunResult = 0;
            PROIIFileOperator.DecompressProIIFile(FileFullPath, tempdir);
            string content = PROIIFileOperator.getUsableContent(stream.StreamName, tempdir);
            IFlashCalculate fcalc = ProIIFactory.CreateFlashCalculate(SourceFileInfo.FileVersion);
            string tray1_f = fcalc.Calculate(content, 1, "0", 3, "0", stream, vapor, liquid, dirLatent, ref ImportResult, ref RunResult);
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
                    double latent = vaporcs.SpEnthalpy.Value - liquidcs.SpEnthalpy.Value;
                    double tcoldbprelief = double.Parse(flash.TempCalc);
                    double tnormalHotInlet = normalHotInlet.Temperature.Value;

                    model.ReliefLoad = Q / latent * tnormalHotInlet - tcoldbprelief / (tnormalHotInlet - tAvg);
                    model.ReliefMW = vaporcs.BulkMwOfPhase.Value;
                    model.ReliefPressure = reliefFirePressure;
                    model.ReliefTemperature = vaporcs.Temperature.Value;

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
        private void Save(object obj)
        {
            if (!model.CheckData()) return; 
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
