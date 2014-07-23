using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReliefProModel.HXs;

namespace ReliefProMain.Model.HXs
{
    public class AirCooledHXFireModel : ModelBase
    {
        private string wettedBundleUnit;
        public string WettedBundleUnit
        {
            get { return wettedBundleUnit; }
            set
            {
                wettedBundleUnit = value;
                NotifyPropertyChanged("WettedBundleUnit");
            }
        }

        private string reliefloadUnit;
        public string ReliefLoadUnit
        {
            get { return reliefloadUnit; }
            set
            {
                reliefloadUnit = value;
                this.NotifyPropertyChanged("ReliefLoadUnit");
            }
        }

        private string reliefTemperatureUnit;
        public string ReliefTemperatureUnit
        {
            get { return reliefTemperatureUnit; }
            set
            {
                reliefTemperatureUnit = value;
                this.NotifyPropertyChanged("ReliefTemperatureUnit");
            }
        }

        private string reliefPressureUnit;
        public string ReliefPressureUnit
        {
            get { return reliefPressureUnit; }
            set
            {
                reliefPressureUnit = value;
                this.NotifyPropertyChanged("ReliefPressureUnit");
            }
        }

        public AirCooledHXFire dbmodel { get; set; }
        public AirCooledHXFireModel(AirCooledHXFire model)
        {
            dbmodel = model;
            this.wettedBundle = model.WettedBundle;
            this.pipingContingency = model.PipingContingency;
            this.reliefMW = model.ReliefMW;
            this.reliefload = model.ReliefLoad;
            this.reliefTemperature = model.ReliefTemperature;
            this.reliefPressure = model.ReliefPressure;
        }

        private double? wettedBundle;
        public double? WettedBundle
        {
            get { return wettedBundle; }
            set
            {
                wettedBundle = value;
                NotifyPropertyChanged("WettedBundle");
            }
        }
        private double? pipingContingency;
        public double? PipingContingency
        {
            get { return pipingContingency; }
            set
            {
                pipingContingency = value;
                this.NotifyPropertyChanged("PipingContingency");
            }
        }

       private double? reliefMW;
        public double? ReliefMW
        {
            get { return reliefMW; }
            set
            {
                reliefMW = value;
                this.NotifyPropertyChanged("ReliefMW");
            }
        }

        private double? reliefload;
        public double? Reliefload
        {
            get { return reliefload; }
            set
            {
                reliefload = value;
                this.NotifyPropertyChanged("Reliefload");
            }
        }

        private double? reliefTemperature;
        public double? ReliefTemperature
        {
            get { return reliefTemperature; }
            set
            {
                reliefTemperature = value;
                this.NotifyPropertyChanged("ReliefTemp");
            }
        }

        private double? reliefPressure;
        public double? ReliefPressure
        {
            get { return reliefPressure; }
            set
            {
                reliefPressure = value;
                this.NotifyPropertyChanged("ReliefPressure");
            }
        }

        private string ratedCapacity_Color;
        public string RatedCapacity_Color
        {
            get { return ratedCapacity_Color; }
            set
            {
                ratedCapacity_Color = value;
                this.NotifyPropertyChanged("RatedCapacity");
            }
        }

        private string reliefMW_Color;
        public string _Color
        {
            get { return reliefMW_Color; }
            set
            {
                reliefMW_Color = value;
                this.NotifyPropertyChanged("ReliefMW");
            }
        }

        private string reliefload_Color;
        public string Reliefload_Color
        {
            get { return reliefload_Color; }
            set
            {
                reliefload_Color = value;
                this.NotifyPropertyChanged("Reliefload");
            }
        }

        private string reliefTemperature_Color;
        public string ReliefTemperature_Color
        {
            get { return reliefTemperature_Color; }
            set
            {
                reliefTemperature_Color = value;
                this.NotifyPropertyChanged("ReliefTemp");
            }
        }

        private string reliefPressure_Color;
        public string ReliefPressure_Color
        {
            get { return reliefPressure_Color; }
            set
            {
                reliefPressure_Color = value;
                this.NotifyPropertyChanged("ReliefPressure");
            }
        }
    }
}
