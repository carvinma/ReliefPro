using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using NHibernate;
using ReliefProLL;
using ReliefProMain.Model;
using ReliefProModel.GlobalDefault;
using UOMLib;

namespace ReliefProMain.ViewModel
{
    public class GlobalDefaultVM : ViewModelBase
    {
        public ICommand OKCMD { get; set; }
        public ICommand DelCMD { get; set; }
        public ICommand AddCMD { get; set; }
        public GlobalDefaultModel model { get; set; }
        private GlobalDefaultBLL globalDefaultBLL;
        public GlobalDefaultVM()
        {
            OKCMD = new DelegateCommand<object>(Save);
            DelCMD = new DelegateCommand<object>(DelRow);
            AddCMD = new DelegateCommand<object>(AddRow);
            ISession SessionPS = TempleSession.Session;
            ISession SessionPF = SessionPS;
            model = new GlobalDefaultModel();
            globalDefaultBLL = new GlobalDefaultBLL(SessionPS, SessionPF);
            model.lstFlareSystem = new ObservableCollection<FlareSystem>(globalDefaultBLL.GetFlareSystem());
            foreach (var flareSystem in model.lstFlareSystem)
            {
                flareSystem.RowGuid = Guid.NewGuid();
            }
            model.conditSetModel = globalDefaultBLL.GetConditionsSettings();
            if (model.conditSetModel != null)
                model.conditSetModel = globalDefaultBLL.ReadConvertModel(model.conditSetModel);
            else
                model.conditSetModel = new ConditionsSettings();
            //UOMLib.UOMEnum uomEnum = new UOMEnum(SessionPF);
            //model.LatentHeatSettingsUnit = uomEnum.UserSpecificEnthalpy;
            //model.DrumSurgeTimeSettingsUnit = uomEnum.UserTime;
            model.LatentHeatSettingsUnit = UOMEnum.SpecificEnthalpy;
            model.DrumSurgeTimeSettingsUnit = UOMEnum.Time;
        }
        private void WriteConvertModel()
        {
            if (model.conditSetModel.LatentHeatSettings != null)
                model.conditSetModel.LatentHeatSettings = UnitConvert.Convert(model.LatentHeatSettingsUnit, UOMLib.UOMEnum.SpecificEnthalpy.ToString(), model.conditSetModel.LatentHeatSettings.Value);
            if (model.conditSetModel.DrumSurgeTimeSettings != null)
                model.conditSetModel.DrumSurgeTimeSettings = UnitConvert.Convert(model.DrumSurgeTimeSettingsUnit, UOMLib.UOMEnum.Time.ToString(), model.conditSetModel.DrumSurgeTimeSettings.Value);
        }
        private void DelRow(object obj)
        {
            if (obj == null)
                return;
            var rowGuid = (Guid)obj;
            var findFlareSystem = model.lstFlareSystem.FirstOrDefault(p => p.RowGuid == rowGuid);
            if (findFlareSystem.RowGuid != null)
                model.lstFlareSystem.Remove(findFlareSystem);
        }
        private void AddRow(object obj)
        {
            var flareSystem = new FlareSystem();
            flareSystem.isDel = true;
            flareSystem.RowGuid = Guid.NewGuid();
            model.lstFlareSystem.Add(flareSystem);
        }
        private void Save(object obj)
        {
            if (obj != null)
            {
                System.Windows.Window wd = obj as System.Windows.Window;
                if (wd != null)
                {
                    WriteConvertModel();
                    globalDefaultBLL.Save(model.lstFlareSystem.ToList(), model.conditSetModel);
                    wd.DialogResult = true;
                }
            }
        }
    }
}
