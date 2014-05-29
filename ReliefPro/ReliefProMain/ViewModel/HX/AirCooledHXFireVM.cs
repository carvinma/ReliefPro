using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using NHibernate;
using ReliefProLL;
using ReliefProMain.Model.HX;
using UOMLib;

namespace ReliefProMain.ViewModel.HX
{
    public class AirCooledHXFireVM : ViewModelBase
    {
        public ICommand CalcCMD { get; set; }
        public ICommand OKCMD { get; set; }
        private ISession SessionPS;
        private ISession SessionPF;
        public AirCooledHXFireModel model { get; set; }
        private HXBLL hxBLL;
        public AirCooledHXFireVM(int ScenarioID, ISession SessionPS, ISession SessionPF)
        {
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;
            OKCMD = new DelegateCommand<object>(Save);
            CalcCMD = new DelegateCommand<object>(CalcResult);

            hxBLL = new HXBLL(SessionPS, SessionPF);
            var airModel = hxBLL.GetAirCooledHXFireModel(ScenarioID);
            airModel = hxBLL.ReadConvertAirCooledHXFireModel(airModel);

            model = new AirCooledHXFireModel(airModel);
            model.dbmodel.ScenarioID = ScenarioID;


            UOMLib.UOMEnum uomEnum = new UOMLib.UOMEnum(SessionPF);
            model.WettedBundleUnit = uomEnum.UserArea;
            model.ReliefLoadUnit = uomEnum.UserMassRate;
            model.ReliefTemperatureUnit = uomEnum.UserTemperature;
            model.ReliefPressureUnit = uomEnum.UserPressure;
        }
        private void WriteConvertModel()
        {
            UnitConvert uc = new UnitConvert();
            model.dbmodel.WettedBundle = uc.Convert(model.WettedBundleUnit, UOMLib.UOMEnum.Area.ToString(), model.WettedBundle);
            model.dbmodel.PipingContingency = model.PipingContingency;
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
                    hxBLL.SaveAirCooledHXFire(model.dbmodel);
                    wd.DialogResult = true;
                }
            }
        }
    }
}
