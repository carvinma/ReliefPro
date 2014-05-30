using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReliefProModel.HXs;

namespace ReliefProMain.Model.HXs
{
    public class HXBlockedOutletModel : ModelBase
    {
        private string normalDutyUnit;
        public string NormalDutyUnit
        {
            get { return normalDutyUnit; }
            set
            {
                normalDutyUnit = value;
                NotifyPropertyChanged("NormalDutyUnit");
            }
        }

        private string normalHotTemperatureUnit;
        public string NormalHotTemperatureUnit
        {
            get { return normalHotTemperatureUnit; }
            set
            {
                normalHotTemperatureUnit = value;
                NotifyPropertyChanged("NormalHotTemperatureUnit");
            }
        }

        private string normalColdInletTemperatureUnit;
        public string NormalColdInletTemperatureUnit
        {
            get { return normalColdInletTemperatureUnit; }
            set
            {
                normalColdInletTemperatureUnit = value;
                NotifyPropertyChanged("NormalColdInletTemperatureUnit");
            }
        }

        private string normalColdOutletTemperatureUnit;
        public string NormalColdOutletTemperatureUnit
        {
            get { return normalColdOutletTemperatureUnit; }
            set
            {
                normalColdOutletTemperatureUnit = value;
                NotifyPropertyChanged("NormalColdOutletTemperatureUnit");
            }
        }

        private string latentPointUnit;
        public string LatentPointUnit
        {
            get { return latentPointUnit; }
            set
            {
                latentPointUnit = value;
                NotifyPropertyChanged("LatentPointUnit");
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

        public HXBlockedOutlet dbmodel { get; set; }
        public HXBlockedOutletModel(HXBlockedOutlet model)
        {
            dbmodel = model;
            this.coldStream = model.ColdStream;
            this.normalDuty = model.NormalDuty;
            this.normalHotTemperature = model.NormalHotTemperature;
            this.normalColdInletTemperature = model.NormalColdInletTemperature;
            this.normalColdOutletTemperature = model.NormalColdOutletTemperature;
            this.latentPoint = model.LatentPoint;
            this.reliefMW = model.ReliefMW;
            this.reliefload = model.ReliefLoad;
            this.reliefTemperature = model.ReliefTemperature;
            this.reliefPressure = model.ReliefPressure;
        }

        private string coldStream = "S123";
        public string ColdStream
        {
            get { return coldStream; }
            set
            {
                coldStream = value;
                NotifyPropertyChanged("ColdStream");
            }
        }

        private double normalDuty;
        public double NormalDuty
        {
            get { return normalDuty; }
            set
            {
                normalDuty = value;
                NotifyPropertyChanged("NormalDuty");
            }
        }

        private double normalHotTemperature;
        public double NormalHotTemperature
        {
            get { return normalHotTemperature; }
            set
            {
                normalHotTemperature = value;
                NotifyPropertyChanged("NormalHotTemperature");
            }
        }

        private double normalColdInletTemperature;
        public double NormalColdInletTemperature
        {
            get { return normalColdInletTemperature; }
            set
            {
                normalColdInletTemperature = value;
                NotifyPropertyChanged("NormalColdInletTemperature");
            }
        }

        private double normalColdOutletTemperature;
        public double NormalColdOutletTemperature
        {
            get { return normalColdOutletTemperature; }
            set
            {
                normalColdOutletTemperature = value;
                NotifyPropertyChanged("NormalColdOutletTemperature");
            }
        }

        private double latentPoint;
        public double LatentPoint
        {
            get { return latentPoint; }
            set
            {
                latentPoint = value;
                NotifyPropertyChanged("LatentPoint");
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
