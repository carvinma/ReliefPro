using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReliefProModel.Compressors;

namespace ReliefProMain.Model.Compressors
{
    public class CentrifugalBlockedOutletModel : ModelBase
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

        public CentrifugalBlockedOutlet dbmodel { get; set; }
        public CentrifugalBlockedOutletModel(CentrifugalBlockedOutlet model)
        {
            dbmodel = model;
            this.scale = model.Scale;
            this.reliefMW = model.ReliefMW;
            this.reliefload = model.Reliefload;
            this.reliefTemp = model.ReliefTemperature;
            this.reliefPressure = model.ReliefPressure;
        }

        private double scale;
        public double Scale
        {
            get { return scale; }
            set
            {
                scale = value;
                this.NotifyPropertyChanged("Scale");
            }
        }

        private double inletLoad;
        public double InletLoad
        {
            get { return inletLoad; }
            set
            {
                inletLoad = value;
                this.NotifyPropertyChanged("InletLoad");
            }
        }

        private double outletPressure;
        public double OutletPressure
        {
            get { return outletPressure; }
            set
            {
                outletPressure = value;
                this.NotifyPropertyChanged("OutletPressure");
            }
        }

        private double surgeLoad;
        public double SurgeLoad
        {
            get { return surgeLoad; }
            set
            {
                surgeLoad = value;
                this.NotifyPropertyChanged("SurgeLoad");
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
