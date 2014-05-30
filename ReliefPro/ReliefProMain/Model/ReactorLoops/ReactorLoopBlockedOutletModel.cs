using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReliefProModel.ReactorLoops;

namespace ReliefProMain.Model.ReactorLoops
{
    public class ReactorLoopBlockedOutletModel : ModelBase
    {
        private string maxGasRateUnit;
        public string MaxGasRateUnit
        {
            get { return maxGasRateUnit; }
            set
            {
                maxGasRateUnit = value;
                this.NotifyPropertyChanged("MaxGasRateUnit");
            }
        }

        private string totalPurgeRateUnit;
        public string TotalPurgeRateUnit
        {
            get { return totalPurgeRateUnit; }
            set
            {
                totalPurgeRateUnit = value;
                this.NotifyPropertyChanged("TotalPurgeRateUnit");
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

        public ReactorLoopBlockedOutlet dbmodel { get; set; }
        public ReactorLoopBlockedOutletModel(ReactorLoopBlockedOutlet model)
        {
            dbmodel = model;
            this.maxGasRate = model.MaxGasRate;
            this.totalPurgeRate = model.TotalPurgeRate;
            this.reliefMW = model.ReliefMW;
            this.reliefload = model.ReliefLoad;
            this.reliefTemperature = model.ReliefTemperature;
            this.reliefCpCv = model.ReliefCpCv;
            this.reliefZ = model.ReliefZ;
        }

        private double maxGasRate;
        public double MaxGasRate
        {
            get { return maxGasRate; }
            set
            {
                maxGasRate = value;
                this.NotifyPropertyChanged("MaxGasRate");
            }
        }

        private double totalPurgeRate;
        public double TotalPurgeRate
        {
            get { return totalPurgeRate; }
            set
            {
                totalPurgeRate = value;
                this.NotifyPropertyChanged("TotalPurgeRate");
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

        private double reliefCpCv;
        public double ReliefCpCv
        {
            get { return reliefCpCv; }
            set
            {
                reliefCpCv = value;
                this.NotifyPropertyChanged("ReliefCpCv");
            }
        }

        private double reliefZ;
        public double ReliefZ
        {
            get { return reliefZ; }
            set
            {
                reliefZ = value;
                this.NotifyPropertyChanged("ReliefZ");
            }
        }
    }
}
