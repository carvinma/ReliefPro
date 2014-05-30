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
using ReliefProMain.Model;
using UOMLib;
using System.Diagnostics;
using NHibernate;

namespace ReliefProMain.ViewModel.Drums
{
    public class DrumBlockedOutletVM : ViewModelBase
    {
        private DrumBll drum;
        public DrumBlockedOutletModel model { get; set; }
        public ICommand CalcCMD { get; set; }

        private ISession SessionPS;
        private ISession SessionPF;
        private string DirPlant { set; get; }
        private string DirProtectedSystem { set; get; }
        private string PrzFile;
        private string PrzVersion;

        public Tuple<double, double, double, double> CalcTuple { get; set; }
        public DrumBlockedOutletVM(int ScenarioID, string przFile, string version, ISession SessionPS, ISession SessionPF, string dirPlant, string dirProtectedSystem)
        {
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;
            this.PrzFile = przFile;
            this.PrzVersion = version;
            DirPlant = dirPlant;
            DirProtectedSystem = dirProtectedSystem;
            drum = new DrumBll();


            var outletModel = drum.GetBlockedOutletModel(SessionPS);
            outletModel = drum.ReadConvertModel(outletModel, SessionPF);

            model = new DrumBlockedOutletModel(outletModel);
            model.dbmodel.DrumID = drum.GetDrumID(SessionPS);
            model.dbmodel.ScenarioID = ScenarioID;
            CalcCMD = new DelegateCommand<object>(CalcResult);

            UOMLib.UOMEnum uomEnum = new UOMEnum(SessionPF);
            model.PressureUnit = uomEnum.UserPressure;
            model.StreamRateUnit = uomEnum.UserMassRate;
            model.FlashingDutyUnit = uomEnum.UserEnthalpyDuty;
            model.ReliefConditionUnit = uomEnum.UserEnthalpyDuty;
        }
        private void WriteConvertModel()
        {
            UnitConvert uc = new UnitConvert();
            model.dbmodel.MaxPressure = uc.Convert(model.PressureUnit, UOMLib.UOMEnum.Pressure.ToString(), model.MaxPressure);
            model.dbmodel.MaxStreamRate = uc.Convert(model.StreamRateUnit, UOMLib.UOMEnum.MassRate.ToString(), model.MaxStreamRate);
            model.dbmodel.NormalFlashDuty = uc.Convert(model.FlashingDutyUnit, UOMLib.UOMEnum.EnthalpyDuty.ToString(), model.NormalFlashDuty);
            model.dbmodel.FDReliefCondition = uc.Convert(model.ReliefConditionUnit, UOMLib.UOMEnum.EnthalpyDuty.ToString(), model.FDReliefCondition);
            model.dbmodel.ReboilerPinch = model.ReboilerPinch;
            model.dbmodel.Feed = model.Feed;
        }
        private void CalcResult(object obj)
        {
            double reliefLoad = 0, reliefMW = 0, reliefT = 0;
            double reliefPressure = drum.ScenarioReliefPressure(SessionPS);
            string vapor = "V_" + Guid.NewGuid().ToString().Substring(0, 6);
            string liquid = "L_" + Guid.NewGuid().ToString().Substring(0, 6);
            string tempdir = DirProtectedSystem + @"\BlockedOutlet";
            if (!Directory.Exists(tempdir))
            {
                Directory.CreateDirectory(tempdir);
            }
            string duty = "0";
            if (drum.PfeedUpstream(SessionPS) > drum.PSet(SessionPS))
            {
                string content = PROIIFileOperator.getUsableContent(drum.Feeds[0].StreamName, DirPlant);
                if (model.DrumType == "Flashing Drum")
                {
                    duty = (model.NormalFlashDuty/Math.Pow(10,6)).ToString();
                }
                IFlashCalculate flashcalc = ProIIFactory.CreateFlashCalculate(PrzVersion);
                int ImportResult = 0;
                int RunResult = 0;
                string f = flashcalc.Calculate(content, 1, reliefPressure.ToString(), 5, duty, drum.Feeds[0], vapor, liquid, tempdir,ref ImportResult,ref RunResult);
                if (ImportResult == 1 || ImportResult == 2)
                {
                    if (RunResult == 1 || RunResult == 2)
                    {
                        IProIIReader reader = ProIIFactory.CreateReader(PrzVersion);
                        reader.InitProIIReader(f);
                        ProIIStreamData proIIvapor = reader.GetSteamInfo(vapor);
                        reader.ReleaseProIIReader();
                        CustomStream cs = ProIIToDefault.ConvertProIIStreamToCustomStream(proIIvapor);

                        reliefMW = double.Parse(cs.BulkMwOfPhase);
                        reliefT = double.Parse(cs.Temperature);
                        reliefLoad = double.Parse(cs.WeightFlow);
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
            else
            {
                reliefLoad = 0;
            }
            WriteConvertModel();
            CalcTuple = new Tuple<double, double, double, double>(reliefLoad, reliefMW, reliefT, reliefPressure);
            drum.SaveDrumBlockedOutlet(model.dbmodel, SessionPS, reliefLoad, reliefMW, reliefT);
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
