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
        CustomStream normalHotInlet = null;
        CustomStream normalColdInlet = new CustomStream();
        CustomStream normalColdOutlet = new CustomStream();

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

            //判断冷测进出，
            CustomStreamBLL csbll = new CustomStreamBLL(SessionPF, SessionPS);
            ObservableCollection<CustomStream> feeds = csbll.GetStreams(SessionPS, false);

            normalColdInlet = feeds[0];
            if (feeds.Count > 1)
            {
                normalHotInlet = feeds[1];
                if (normalColdInlet.Temperature > feeds[1].Temperature)
                {
                    normalColdInlet = feeds[1];
                    normalHotInlet = feeds[0];
                }
            }
            ObservableCollection<CustomStream> products = csbll.GetStreams(SessionPS, true);
            normalColdOutlet = products[0];
            if (products.Count > 1)
            {
                if (normalColdOutlet.Temperature > products[1].Temperature)
                {
                    normalColdOutlet = products[1];
                }
            }


            HeatExchangerDAL heatdal = new HeatExchangerDAL();
            HeatExchanger heat = heatdal.GetModel(SessionPS);
            model.NormalDuty = heat.Duty;
            model.NormalColdInletTemperature = normalColdInlet.Temperature;
            model.NormalColdOutletTemperature = normalColdOutlet.Temperature;
            if (normalHotInlet != null)
            {
                model.NormalHotTemperature = normalHotInlet.Temperature;
            }
            model.ColdStream = normalColdInlet.StreamName;


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
            double Q = model.NormalDuty;


            double tAvg = 0.5 * (normalColdInlet.Temperature + normalColdOutlet.Temperature);

            PSVDAL psvDAL = new PSVDAL();
            PSV psv = psvDAL.GetModel(SessionPS);
            double pressure = psv.Pressure;

            double reliefPressure = pressure * psv.ReliefPressureFactor;

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
            string tray1_f = fcalc.Calculate(content, 1, reliefPressure.ToString(), 3, "0", stream, vapor, liquid, dirLatent, ref ImportResult, ref RunResult);
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
                    model.ReliefLoad = Q / latent * (model.NormalHotTemperature - tcoldbprelief) / (model.NormalHotTemperature - tAvg);
                    if (model.ReliefLoad < 0 || tcoldbprelief > model.NormalHotTemperature)
                        model.ReliefLoad = 0;
                    model.ReliefMW = vaporcs.BulkMwOfPhase;
                    model.ReliefPressure = reliefPressure;
                    model.ReliefTemperature = vaporcs.Temperature;

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
