using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReliefProModel.ReactorLoops;

namespace ReliefProMain.Models.ReactorLoops
{
    public class GeneralFailureCommonModel : ModelBase
    {
        public GeneralFailureCommon dbModel { get; set; }
        private string reliefLoadUnit;
        public string ReliefLoadUnit
        {
            get { return reliefLoadUnit; }
            set
            {
                reliefLoadUnit = value;
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

        public List<UtilityHXModel> lstUtilityHX { get; set; }

        public bool RecycleCompressorFailure
        {
            get { return dbModel.RecycleCompressorFailure; }
            set
            {
                dbModel.RecycleCompressorFailure = value;
                this.NotifyPropertyChanged("RecycleCompressorFailure");
            }
        }
        public bool CalcInjectionWaterStream
        {
            get { return dbModel.CalcInjectionWaterStream; }
            set
            {
                dbModel.CalcInjectionWaterStream = value;
                this.NotifyPropertyChanged("CalcInjectionWaterStream");
            }
        }
        public double InjectionWaterStream
        {
            get { return dbModel.InjectionWaterStream; }
            set
            {
                dbModel.InjectionWaterStream = value;
                this.NotifyPropertyChanged("InjectionWaterStream");
            }
        }

        public bool CalcHXNetworkColdStream
        {
            get { return dbModel.CalcHXNetworkColdStream; }
            set
            {
                dbModel.CalcHXNetworkColdStream = value;
                this.NotifyPropertyChanged("CalcHXNetworkColdStream");
            }
        }
        public double HXNetworkColdStream
        {
            get { return dbModel.HXNetworkColdStream; }
            set
            {
                dbModel.HXNetworkColdStream = value;
                this.NotifyPropertyChanged("HXNetworkColdStream");
            }
        }

        public double ReliefLoad
        {
            get { return dbModel.ReliefLoad; }
            set
            {
                dbModel.ReliefLoad = value;
                this.NotifyPropertyChanged("ReliefLoad");
            }
        }
        public double ReliefMW
        {
            get { return dbModel.ReliefMW; }
            set
            {
                dbModel.ReliefMW = value;
                this.NotifyPropertyChanged("ReliefMW");
            }
        }
        public double ReliefTemperature
        {
            get { return dbModel.ReliefTemperature; }
            set
            {
                dbModel.ReliefTemperature = value;
                this.NotifyPropertyChanged("ReliefTemperature");
            }
        }
        public double ReliefPressure
        {
            get { return dbModel.ReliefPressure; }
            set
            {
                dbModel.ReliefPressure = value;
                this.NotifyPropertyChanged("ReliefPressure");
            }
        }
    }
    public class UtilityHXModel : ModelBase
    {
        private string hXName;
        public string HXName
        {
            get { return hXName; }
            set
            {
                hXName = value;
                this.NotifyPropertyChanged("HXName");
            }
        }
        private bool stop;
        public bool Stop
        {
            get { return stop; }
            set
            {
                stop = value;
                this.NotifyPropertyChanged("Stop");
            }
        }
        private double dutyFactor;
        public double DutyFactor
        {
            get { return dutyFactor; }
            set
            {
                dutyFactor = value;
                this.NotifyPropertyChanged("DutyFactor");
            }
        }
    }
}
