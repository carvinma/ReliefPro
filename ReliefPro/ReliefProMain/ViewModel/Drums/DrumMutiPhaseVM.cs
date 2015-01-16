using NHibernate;
using ReliefProMain.Commands;
using ReliefProMain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UOMLib;

namespace ReliefProMain.ViewModel.Drums
{
    class DrumMutiPhaseVM:ViewModelBase
    {
        public ICommand OKCMD { get; set; }
        public DrumFireModel model { get; set; }

        public DrumMutiPhaseVM(DrumFireModel m, ISession SessionPlant, ISession SessionProtectedSystem)
        {
            if (m.FluidType == 5)
            {
                m.LatentHeat = 46;
                m.LatentHeat = UnitConvert.Convert(UOMEnum.SpecificEnthalpy, m.LatentHeatUnit, m.LatentHeat);
            }
            model = m;
            
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
