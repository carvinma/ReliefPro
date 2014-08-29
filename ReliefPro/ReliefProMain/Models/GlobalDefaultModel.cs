using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using ReliefProModel.GlobalDefault;

namespace ReliefProMain.Models
{
    public class GlobalDefaultModel : ModelBase
    {
        private ObservableCollection<FlareSystem> _lstFlareSystem;
        public ObservableCollection<FlareSystem> lstFlareSystem
        {
            get { return _lstFlareSystem; }
            set
            {
                _lstFlareSystem = value;
                NotifyPropertyChanged("lstFlareSystem");
            }
        }
        public ConditionsSettings conditSetModel { get; set; }
        public bool AirCondition
        {
            get
            {
                return conditSetModel.AirCondition;
            }
            set
            {
                conditSetModel.AirCondition = value;
                NotifyPropertyChanged("AirCondition");
            }
        }
        public bool CoolingWaterCondition
        {
            get
            {
                return conditSetModel.CoolingWaterCondition;
            }
            set
            {
                conditSetModel.CoolingWaterCondition = value;
                NotifyPropertyChanged("CoolingWaterCondition");
            }
        }
        public bool SteamCondition
        {
            get
            {
                return conditSetModel.SteamCondition;
            }
            set
            {
                conditSetModel.SteamCondition = value;
                NotifyPropertyChanged("SteamCondition");
            }
        }
        public double LatentHeatSettings
        {
            get
            {
                return conditSetModel.LatentHeatSettings;
            }
            set
            {
                conditSetModel.LatentHeatSettings = value;
                NotifyPropertyChanged("LatentHeatSettings");
            }
        }
        public double DrumSurgeTimeSettings
        {
            get
            {
                return conditSetModel.DrumSurgeTimeSettings;
            }
            set
            {
                conditSetModel.DrumSurgeTimeSettings = value;
                NotifyPropertyChanged("DrumSurgeTimeSettings");
            }
        }

        private string latentHeatSettingsUnit;
        public string LatentHeatSettingsUnit
        {
            get
            {
                return latentHeatSettingsUnit;
            }
            set
            {
                latentHeatSettingsUnit = value;
                NotifyPropertyChanged("LatentHeatSettingsUnit");
            }
        }
        private string drumSurgeTimeSettingsUnit;
        public string DrumSurgeTimeSettingsUnit
        {
            get
            {
                return drumSurgeTimeSettingsUnit;
            }
            set
            {
                drumSurgeTimeSettingsUnit = value;
                NotifyPropertyChanged("DrumSurgeTimeSettingsUnit");
            }
        }
    }
}
