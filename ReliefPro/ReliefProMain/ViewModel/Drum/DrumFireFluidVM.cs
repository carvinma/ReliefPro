using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using NHibernate;
using ReliefProLL;
using ReliefProMain.Commands;
using ReliefProMain.Model;

namespace ReliefProMain.ViewModel.Drum
{
    public class DrumFireFluidVM
    {
        public ICommand OKCMD { get; set; }
        public DrumFireFluidModel model { get; set; }
        private ISession SessionPS;
        private ISession SessionPF;
        public DrumFireFluidVM(int ScenarioID, ISession SessionPS, ISession SessionPF)
        {
            this.SessionPS = SessionPS;
            this.SessionPF = SessionPF;

            DrumFireFluidBLL fluidBll = new DrumFireFluidBLL(SessionPS, SessionPF);


            var fireModel = fluidBll.GetFireFluidModel();
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

        private void Save(object obj)
        {
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
