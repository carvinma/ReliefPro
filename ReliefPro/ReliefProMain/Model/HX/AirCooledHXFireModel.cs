using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReliefProModel.HX;

namespace ReliefProMain.Model.HX
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

        private double wettedBundle;
        public double WettedBundle
        {
            get { return wettedBundle; }
            set
            {
                wettedBundle = value;
                NotifyPropertyChanged("WettedBundle");
            }
        }
        private double pipingContingency;
        public double PipingContingency
        {
            get { return pipingContingency; }
            set
            {
                pipingContingency = value;
                this.NotifyPropertyChanged("PipingContingency");
            }
        }

        private double reliefMW;
        public double ReliefMW
        {
            get { return reliefMW; }
            set
            {
                reliefMW = value;
                this.NotifyPropertyChanged("ReliefMW");
            }
        }

        private double reliefload;
        public double ReliefLoad
        {
            get { return reliefload; }
            set
            {
                reliefload = value;
                this.NotifyPropertyChanged("ReliefLoad");
            }
        }

        private double reliefTemperature;
        public double ReliefTemperature
        {
            get { return reliefTemperature; }
            set
            {
                reliefTemperature = value;
                this.NotifyPropertyChanged("ReliefTemperature");
            }
        }

        private double reliefPressure;
        public double ReliefPressure
        {
            get { return reliefPressure; }
            set
            {
                reliefPressure = value;
                this.NotifyPropertyChanged("ReliefPressure");
            }
        }
    }
}
