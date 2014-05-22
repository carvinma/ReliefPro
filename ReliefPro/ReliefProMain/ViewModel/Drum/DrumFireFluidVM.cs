using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using NHibernate;
using ReliefProBLL;
using ReliefProMain.Commands;
using ReliefProMain.Model;
using UOMLib;

namespace ReliefProMain.ViewModel.Drum
{
    public class DrumFireFluidVM
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

            UOMLib.UOMEnum uomEnum = new UOMLib.UOMEnum(SessionPF);
#warning   inputVesselUnit
            model.VesselUnit = uomEnum.UserArea;
            model.PressureUnit = uomEnum.UserPressure;
            model.TemperatureUnit = uomEnum.UserTemperature;
            model.PSVPressureUnit = uomEnum.UserPressure;
            OKCMD = new DelegateCommand<object>(Save);
        }

        private void WriteConvertModel()
        {
            UnitConvert uc = new UnitConvert();
            //model.dbmodel.GasVaporMW = uc.Convert(model.WettedAreaUnit, UOMLib.UOMEnum.Area.ToString(), model.GasVaporMW);
            model.dbmodel.GasVaporMW = model.VaporMW;
            model.dbmodel.ExposedVesse = uc.Convert(model.VesselUnit, UOMLib.UOMEnum.Area.ToString(), model.Vessel);
            model.dbmodel.NormaTemperature = uc.Convert(model.TemperatureUnit, UOMLib.UOMEnum.Temperature.ToString(), model.Temperature);
            model.dbmodel.NormalPressure = uc.Convert(model.PressureUnit, UOMLib.UOMEnum.Pressure.ToString(), model.Pressure);
            model.dbmodel.PSVPressure = uc.Convert(model.PSVPressureUnit, UOMLib.UOMEnum.Pressure.ToString(), model.PSVPressure);
            model.dbmodel.TW = model.TW;
            //model.dbmodel.TW = uc.Convert(model.t, UOMLib.UOMEnum.Temperature.ToString(), model.ReliefTemperature);
        }
        private void Save(object obj)
        {
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
