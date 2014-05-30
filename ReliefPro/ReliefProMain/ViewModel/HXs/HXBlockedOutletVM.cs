using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using NHibernate;
using ReliefProLL;
using ReliefProMain.Model.HXs;
using UOMLib;

namespace ReliefProMain.ViewModel.HXs
{
    public class HXBlockedOutletVM : ViewModelBase
    {
        public ICommand CalcCMD { get; set; }
        public ICommand OKCMD { get; set; }
        private ISession SessionPS;
        private ISession SessionPF;
        public HXBlockedOutletModel model { get; set; }
        private HXBLL hxBLL;
        public HXBlockedOutletVM(int ScenarioID, ISession SessionPS, ISession SessionPF)
        {
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;
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
            UnitConvert uc = new UnitConvert();
            model.dbmodel.ColdStream = model.ColdStream;
            model.dbmodel.NormalDuty = uc.Convert(model.NormalDutyUnit, UOMLib.UOMEnum.EnthalpyDuty.ToString(), model.NormalDuty);
            model.dbmodel.NormalHotTemperature = uc.Convert(model.NormalHotTemperatureUnit, UOMLib.UOMEnum.EnthalpyDuty.ToString(), model.NormalHotTemperature);
            model.dbmodel.NormalColdInletTemperature = uc.Convert(model.NormalColdInletTemperatureUnit, UOMLib.UOMEnum.EnthalpyDuty.ToString(), model.NormalColdInletTemperature);
            model.dbmodel.NormalColdOutletTemperature = uc.Convert(model.NormalColdOutletTemperatureUnit, UOMLib.UOMEnum.EnthalpyDuty.ToString(), model.NormalColdOutletTemperature);
            model.dbmodel.LatentPoint = uc.Convert(model.LatentPointUnit, UOMLib.UOMEnum.SpecificEnthalpy.ToString(), model.LatentPoint);
            model.dbmodel.ReliefMW = model.ReliefMW;
            model.dbmodel.ReliefLoad = uc.Convert(model.ReliefLoadUnit, UOMLib.UOMEnum.MassRate.ToString(), model.ReliefLoad);
            model.dbmodel.ReliefTemperature = uc.Convert(model.ReliefTemperatureUnit, UOMLib.UOMEnum.Temperature.ToString(), model.ReliefTemperature);
            model.dbmodel.ReliefPressure = uc.Convert(model.ReliefPressureUnit, UOMLib.UOMEnum.Pressure.ToString(), model.ReliefPressure);
        }
        private void CalcResult(object obj)
        {
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
