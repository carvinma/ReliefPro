using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReliefProModel.Compressors;

namespace ReliefProMain.Model.Compressors
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
            this.reliefTemp = model.ReliefTemperature;
            this.reliefPressure = model.ReliefPressure;
        }

        private double ratedCapacity;
        public double RatedCapacity
        {
            get { return ratedCapacity; }
            set
            {
                ratedCapacity = value;
                this.NotifyPropertyChanged("RatedCapacity");
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
        public double Reliefload
        {
            get { return reliefload; }
            set
            {
                reliefload = value;
                this.NotifyPropertyChanged("Reliefload");
            }
        }

        private double reliefTemp;
        public double ReliefTemp
        {
            get { return reliefTemp; }
            set
            {
                reliefTemp = value;
                this.NotifyPropertyChanged("ReliefTemp");
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
