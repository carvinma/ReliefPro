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
        private string PrzFile;
        private string PrzVersion;
        public HXBlockedOutletModel model { get; set; }
        private HXBLL hxBLL;
        public HXBlockedOutletVM(int ScenarioID, string przFile, string version, ISession SessionPS, ISession SessionPF, string dirPlant, string dirProtectedSystem)
        {
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;
            DirPlant = dirPlant;
            DirProtectedSystem = dirProtectedSystem;
            DirPlant = dirPlant;
            DirProtectedSystem = dirProtectedSystem;
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
            model.dbmodel.NormalDuty = UnitConvert.Convert(model.NormalDutyUnit, UOMLib.UOMEnum.EnthalpyDuty.ToString(), model.NormalDuty.Value);
            model.dbmodel.NormalHotTemperature = UnitConvert.Convert(model.NormalHotTemperatureUnit, UOMLib.UOMEnum.EnthalpyDuty.ToString(), model.NormalHotTemperature.Value);
            model.dbmodel.NormalColdInletTemperature = UnitConvert.Convert(model.NormalColdInletTemperatureUnit, UOMLib.UOMEnum.EnthalpyDuty.ToString(), model.NormalColdInletTemperature.Value);
            model.dbmodel.NormalColdOutletTemperature = UnitConvert.Convert(model.NormalColdOutletTemperatureUnit, UOMLib.UOMEnum.EnthalpyDuty.ToString(), model.NormalColdOutletTemperature.Value);
            model.dbmodel.LatentPoint = UnitConvert.Convert(model.LatentPointUnit, UOMLib.UOMEnum.SpecificEnthalpy.ToString(), model.LatentPoint.Value);
            model.dbmodel.ReliefMW = model.ReliefMW;
            model.dbmodel.ReliefLoad = UnitConvert.Convert(model.ReliefLoadUnit, UOMLib.UOMEnum.MassRate.ToString(), model.ReliefLoad.Value);
            model.dbmodel.ReliefTemperature = UnitConvert.Convert(model.ReliefTemperatureUnit, UOMLib.UOMEnum.Temperature.ToString(), model.ReliefTemperature.Value);
            model.dbmodel.ReliefPressure = UnitConvert.Convert(model.ReliefPressureUnit, UOMLib.UOMEnum.Pressure.ToString(), model.ReliefPressure.Value);
        }
        private void CalcResult(object obj)
        {
            double Q = 0;

            CustomStream normalHotInlet = new CustomStream();
            CustomStream normalColdInlet = new CustomStream();
            CustomStream normalColdOutlet = new CustomStream();
            double tAvg = 0.5 * (double.Parse(normalColdInlet.Temperature) + double.Parse(normalColdOutlet.Temperature));

            PSVDAL psvDAL = new PSVDAL();
            PSV psv = psvDAL.GetModel(SessionPS);
            double pressure = double.Parse(psv.Pressure);

            double reliefFirePressure = pressure * double.Parse(psv.ReliefPressureFactor);

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
            PROIIFileOperator.DecompressProIIFile(PrzFile, tempdir);
            string content = PROIIFileOperator.getUsableContent(stream.StreamName, tempdir);
            IFlashCalculate fcalc = ProIIFactory.CreateFlashCalculate(PrzVersion);
            string tray1_f = fcalc.Calculate(content, 1, "0", 3, "0", stream, vapor, liquid, dirLatent, ref ImportResult, ref RunResult);
            if (ImportResult == 1 || ImportResult == 2)
            {
                if (RunResult == 1 || RunResult == 2)
                {
                    IProIIReader reader = ProIIFactory.CreateReader(PrzVersion);
                    reader.InitProIIReader(tray1_f);
                    ProIIStreamData proIIVapor = reader.GetSteamInfo(vapor);
                    ProIIStreamData proIILiquid = reader.GetSteamInfo(liquid);
                    ProIIEqData flash=reader.GetEqInfo("Flash","F_1");
                    reader.ReleaseProIIReader();
                    CustomStream liquidcs = ProIIToDefault.ConvertProIIStreamToCustomStream(proIILiquid);
                    CustomStream vaporcs = ProIIToDefault.ConvertProIIStreamToCustomStream(proIIVapor);
                    double latent = double.Parse(vaporcs.SpEnthalpy) - double.Parse(liquidcs.SpEnthalpy);
                    double tcoldbprelief = double.Parse(flash.TempCalc);
                    double tnormalHotInlet=double.Parse(normalHotInlet.Temperature);

                    model.ReliefLoad = Q / latent * tnormalHotInlet - tcoldbprelief / (tnormalHotInlet-tAvg);
                    model.ReliefMW = double.Parse(vaporcs.BulkMwOfPhase);
                    model.ReliefPressure = reliefFirePressure;
                    model.ReliefTemperature = double.Parse(vaporcs.Temperature);

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
