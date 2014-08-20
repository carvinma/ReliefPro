using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReliefProModel.ReactorLoops;

namespace ReliefProMain.Models.ReactorLoops
{
    public class ReactorLoopCommonModel : ModelBase
    {

        private string effluentTemperatureUnit;
        public string EffluentTemperatureUnit
        {
            get { return effluentTemperatureUnit; }
            set
            {
                effluentTemperatureUnit = value;
                this.NotifyPropertyChanged("EffluentTemperatureUnit");
            }
        }
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

        public ReactorLoopCommon dbmodel { get; set; }
        public ReactorLoopCommonModel(ReactorLoopCommon model)
        {
            dbmodel = model;
            this.reactorType = dbmodel.ReactorType;
            this.effluentTemperature = dbmodel.EffluentTemperature;
            this.maxGasRate = model.MaxGasRate;
            this.totalPurgeRate = model.TotalPurgeRate;
            this.reliefMW = model.ReliefMW;
            this.reliefload = model.ReliefLoad;
            this.reliefTemperature = model.ReliefTemperature;
            this.reliefCpCv = model.ReliefCpCv;
            this.reliefZ = model.ReliefZ;
        }

        private int reactorType;
        public int ReactorType
        {
            get { return reactorType; }
            set
            {
                reactorType = value;
                this.NotifyPropertyChanged("ReactorType");
            }
        }
        private double? effluentTemperature;
        public double? EffluentTemperature
        {
            get { return effluentTemperature; }
            set
            {
                effluentTemperature = value;
                this.NotifyPropertyChanged("EffluentTemperature");
            }
        }

        private double? maxGasRate;
        public double? MaxGasRate
        {
            get { return maxGasRate; }
            set
            {
                maxGasRate = value;
                this.NotifyPropertyChanged("MaxGasRate");
            }
        }

        private double? totalPurgeRate;
        public double? TotalPurgeRate
        {
            get { return totalPurgeRate; }
            set
            {
                totalPurgeRate = value;
                this.NotifyPropertyChanged("TotalPurgeRate");
            }
        }

        private double? reliefload;
        public double? ReliefLoad
        {
            get { return reliefload; }
            set
            {
                reliefload = value;
                this.NotifyPropertyChanged("ReliefLoad");
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

        private double? reliefCpCv;
        public double? ReliefCpCv
        {
            get { return reliefCpCv; }
            set
            {
                reliefCpCv = value;
                this.NotifyPropertyChanged("ReliefCpCv");
            }
        }

        private double? reliefZ;
        public double? ReliefZ
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
