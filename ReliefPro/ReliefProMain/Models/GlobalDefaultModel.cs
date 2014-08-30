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
        private ObservableCollection<GlobalFlareSystem> _lstFlareSystem;
        public ObservableCollection<GlobalFlareSystem> lstFlareSystem
        {
            get { return _lstFlareSystem; }
            set
            {
                _lstFlareSystem = value;
                NotifyPropertyChanged("lstFlareSystem");
            }
        }
        public GlobalDefaultModel()
        {
            lstFlareSystem = new ObservableCollection<GlobalFlareSystem>();
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

    public class GlobalFlareSystem : ModelBase
    {
        public GlobalFlareSystem(FlareSystem m)
        {
            dbmodel = m;
        }
        public FlareSystem dbmodel;
        public int ID
        {
            get { return dbmodel.ID; }
            set
            {
                dbmodel.ID = value;
                NotifyPropertyChanged("ID");
            }
        }
        public Guid RowGuid
        {
            get { return dbmodel.RowGuid; }
            set
            {
                dbmodel.RowGuid = value;
                NotifyPropertyChanged("RowGuid");
            }
        }
        public String FlareName
        {
            get { return dbmodel.FlareName; }
            set
            {
                dbmodel.FlareName = value;
                NotifyPropertyChanged("FlareName");
            }
        }
        public double DesignBackPressure
        {
            get { return dbmodel.DesignBackPressure; }
            set
            {
                dbmodel.DesignBackPressure = value;
                NotifyPropertyChanged("DesignBackPressure");
            }
        }
        public bool isDel
        {
            get { return dbmodel.isDel; }
            set
            {
                dbmodel.isDel = value;
                NotifyPropertyChanged("isDel");
            }
        }
    }
}
