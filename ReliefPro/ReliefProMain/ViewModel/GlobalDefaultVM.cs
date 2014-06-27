using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using NHibernate;
using ReliefProLL;
using ReliefProMain.Model;
using UOMLib;

namespace ReliefProMain.ViewModel
{
    public class GlobalDefaultVM : ViewModelBase
    {
        public ICommand OKCMD { get; set; }
        public GlobalDefaultModel model { get; set; }
        private GlobalDefaultBLL globalDefaultBLL;
        public GlobalDefaultVM(ISession SessionPS, ISession SessionPF)
        {
            OKCMD = new DelegateCommand<object>(Save);
            globalDefaultBLL = new GlobalDefaultBLL(SessionPS, SessionPF);
            model.lstFlareSystem = globalDefaultBLL.GetFlareSystem().ToList();
            model.conditSetModel = globalDefaultBLL.GetConditionsSettings();
            model.conditSetModel = globalDefaultBLL.ReadConvertModel(model.conditSetModel);
        }
        private void WriteConvertModel()
        {
            if (model.conditSetModel.LatentHeatSettings != null)
                model.conditSetModel.LatentHeatSettings = UnitConvert.Convert(model.LatentHeatSettingsUnit, UOMLib.UOMEnum.SpecificEnthalpy.ToString(), model.conditSetModel.LatentHeatSettings.Value);
            if (model.conditSetModel.DrumSurgeTimeSettings != null)
                model.conditSetModel.DrumSurgeTimeSettings = UnitConvert.Convert(model.DrumSurgeTimeSettingsUnit, UOMLib.UOMEnum.Time.ToString(), model.conditSetModel.DrumSurgeTimeSettings.Value);
        }
        private void Save(object obj)
        {
            if (obj != null)
            {
                System.Windows.Window wd = obj as System.Windows.Window;
                if (wd != null)
                {
                    WriteConvertModel();
                    globalDefaultBLL.Save(model.lstFlareSystem, model.conditSetModel);
                    wd.DialogResult = true;
                }
            }
        }
    }
}
