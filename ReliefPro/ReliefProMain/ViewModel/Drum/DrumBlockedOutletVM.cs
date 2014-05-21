using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReliefProBLL.Common;
using ReliefProDAL;
using ReliefProModel;
using ReliefProModel.Drum;
using ReliefProBLL;

using System.Windows.Input;
using ReliefProMain.Commands;
using ProII;
using ReliefProMain.Model;
using UOMLib;
using System.Diagnostics;
using NHibernate;

namespace ReliefProMain.ViewModel.Drum
{
    public class DrumBlockedOutletVM : ViewModelBase
    {
        private DrumBll drum;
        public DrumBlockedOutletModel model { get; set; }
        public ICommand CalcCMD { get; set; }

        private ISession SessionPS;
        private ISession SessionPF;
        public DrumBlockedOutletVM(int ScenarioID, ISession SessionPS, ISession SessionPF)
        {
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;
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
            //model.dbmodel.MaxStreamRate = uc.Convert(model.StreamRateUnit, UOMLib.UOMEnum.Pressure.ToString(), model.MaxPressure);
            //model.dbmodel.NormalFlashDuty = uc.Convert(model.FlashingDutyUnit, UOMLib.UOMEnum.Pressure.ToString(), model.MaxPressure);
            //model.dbmodel.FDReliefCondition = uc.Convert(model.ReliefConditionUnit, UOMLib.UOMEnum.Pressure.ToString(), model.MaxPressure);
        }
        private void CalcResult(object obj)
        {
            double reliefLoad = 0, reliefMW = 0, reliefT = 0;
            if (drum.PfeedUpstream(SessionPS) > drum.PSet(SessionPS))
            {
                if (model.DrumType == "Flashing Drum")
                {
                    //string version = "9.1";
                    // IFlashCalculateW flashCalc = ProIIFactory.CreateFlashCalculateW(version);
                    // string filePath = flashCalc.Calculate();
                }
                else
                { }
            }
            else
            {
                reliefLoad = 0;
            }
            WriteConvertModel();
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
