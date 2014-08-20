using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReliefProModel.Compressors;

namespace ReliefProMain.Models.Compressors
{
    public class PistonBlockedOutletModel : ModelBase
    {
        private string reliefloadUnit;
        public string ReliefloadUnit
        {
            get { return reliefloadUnit; }
            set
            {
                reliefloadUnit = value;
                this.NotifyPropertyChanged("ReliefloadUnit");
            }
        }

        private string reliefTempUnit;
        public string ReliefTempUnit
        {
            get { return reliefTempUnit; }
            set
            {
                reliefTempUnit = value;
                this.NotifyPropertyChanged("ReliefTempUnit");
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

        public PistonBlockedOutlet dbmodel { get; set; }
        public PistonBlockedOutletModel(PistonBlockedOutlet model)
        {
            dbmodel = model;
            this.ratedCapacity = model.RatedCapacity;
            this.reliefMW = model.ReliefMW;
            this.reliefload = model.Reliefload;
            this.reliefTemperature = model.ReliefTemperature;
            this.reliefPressure = model.ReliefPressure;

            this.ratedCapacity_Color = model.RatedCapacity_Color;
            this.reliefMW_Color = model.ReliefMW_Color;
            this.reliefload_Color = model.Reliefload_Color;
            this.reliefTemperature_Color = model.ReliefTemperature_Color;
            this.reliefPressure_Color = model.ReliefPressure_Color;
        }

        private double? ratedCapacity;
        public double? RatedCapacity
        {
            get { return ratedCapacity; }
            set
            {
                ratedCapacity = value;
                this.NotifyPropertyChanged("RatedCapacity");
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
                this.NotifyPropertyChanged("ReliefTemperature");
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
                this.NotifyPropertyChanged("RatedCapacity_Color");
            }
        }

        private string reliefMW_Color;
        public string _Color
        {
            get { return reliefMW_Color; }
            set
            {
                reliefMW_Color = value;
                this.NotifyPropertyChanged("ReliefMW_Color");
            }
        }

        private string reliefload_Color;
        public string Reliefload_Color
        {
            get { return reliefload_Color; }
            set
            {
                reliefload_Color = value;
                this.NotifyPropertyChanged("Reliefload_Color");
            }
        }

        private string reliefTemperature_Color;
        public string ReliefTemperature_Color
        {
            get { return reliefTemperature_Color; }
            set
            {
                reliefTemperature_Color = value;
                this.NotifyPropertyChanged("ReliefTemperature_Color");
            }
        }

        private string reliefPressure_Color;
        public string ReliefPressure_Color
        {
            get { return reliefPressure_Color; }
            set
            {
                reliefPressure_Color = value;
                this.NotifyPropertyChanged("ReliefPressure_Color");
            }
        }
    }
}
