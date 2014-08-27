using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReliefProModel.HXs;


namespace ReliefProMain.Models.HXs
{
    public class TubeRuptureModel : ModelBase
    {

        private string _ODUnit;
        public string ODUnit
        {
            get { return _ODUnit; }
            set
            {
                _ODUnit = value;
                this.NotifyPropertyChanged("ODUnit");
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

        public TubeRupture dbmodel;
        public TubeRuptureModel(TubeRupture model)
        {
            dbmodel = model;

            this.OD = model.OD;
            this.ReliefPressure = model.ReliefPressure;
            this.ReliefLoad = model.ReliefLoad;
            this.ReliefMW = this.ReliefMW;
            this.ReliefTemperature = this.ReliefTemperature;
        }

       
        private double _OD;
        public double OD
        {
            get { return _OD; }
            set
            {
                _OD = value;
                this.NotifyPropertyChanged("OD");
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
