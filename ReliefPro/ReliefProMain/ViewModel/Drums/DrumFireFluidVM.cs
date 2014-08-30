using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using NHibernate;
using ReliefProBLL;
using ReliefProMain.Commands;
using ReliefProMain.Models;
using UOMLib;

namespace ReliefProMain.ViewModel.Drums
{
    public class DrumFireFluidVM : ViewModelBase
    {
        public ICommand OKCMD { get; set; }
        public DrumFireFluidModel model { get; set; }
        private ISession SessionPS;
        private ISession SessionPF;
        public DrumFireFluidVM(int DrumFireCalcID, ISession SessionPS, ISession SessionPF)
        {
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;

            DrumFireFluidBLL fluidBll = new DrumFireFluidBLL(SessionPS, SessionPF);

            var fireModel = fluidBll.GetFireFluidModel(DrumFireCalcID);
            fireModel = fluidBll.ReadConvertModel(fireModel);
            model = new DrumFireFluidModel(fireModel);
            //  model = new DrumBlockedOutletModel(fireModel);
            ////  model.dbmodel.DrumFireCalcID = drum.GetDrumID(dbPSFile);
            //  //model.dbmodel.ScenarioID = ScenarioID;

            UOMLib.UOMEnum uomEnum = UOMSingle.UomEnums.FirstOrDefault(p => p.SessionDBPath == SessionPF.Connection.ConnectionString);
            model.VesselUnit = uomEnum.UserArea;
            model.PressureUnit = uomEnum.UserPressure;
            model.TemperatureUnit = uomEnum.UserTemperature;
            model.PSVPressureUnit = uomEnum.UserPressure;
            model.TWUnit = uomEnum.UserTemperature;
            OKCMD = new DelegateCommand<object>(Save);
        }

        private void WriteConvertModel()
        {
            //model.dbmodel.GasVaporMW = uc.Convert(model.WettedAreaUnit, UOMLib.UOMEnum.Area.ToString(), model.GasVaporMW);
            model.dbmodel.GasVaporMW = model.VaporMW;
            model.dbmodel.ExposedVesse = UnitConvert.Convert(model.VesselUnit, UOMLib.UOMEnum.Area.ToString(), model.Vessel);
            model.dbmodel.NormaTemperature = UnitConvert.Convert(model.TemperatureUnit, UOMLib.UOMEnum.Temperature.ToString(), model.Temperature);
            model.dbmodel.NormalPressure = UnitConvert.Convert(model.PressureUnit, UOMLib.UOMEnum.Pressure.ToString(), model.Pressure);
            model.dbmodel.PSVPressure = UnitConvert.Convert(model.PSVPressureUnit, UOMLib.UOMEnum.Pressure.ToString(), model.PSVPressure);
            model.dbmodel.TW = model.TW;
            //model.dbmodel.TW = uc.Convert(model.t, UOMLib.UOMEnum.Temperature.ToString(), model.ReliefTemperature);
        }
        private void Save(object obj)
        {
            if (!CheckData()) return;
            if (obj != null)
            {
                System.Windows.Window wd = obj as System.Windows.Window;
                if (wd != null)
                {
                    WriteConvertModel();
                    wd.DialogResult = true;
                }
            }
        }
    }
}
