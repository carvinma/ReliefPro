using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReliefProModel.ReactorLoops;

namespace ReliefProMain.Models.ReactorLoops
{
    public class GeneralFailureCommonModel : ModelBase
    {
        public GeneralFailureCommon dbmodel { get; set; }
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

        public List<GeneralFailureHXModel> lstNetworkHX { get; set; }
        public List<GeneralFailureHXModel> lstUtilityHX { get; set; }

        public bool RecycleCompressorFailure
        {
            get { return dbmodel.RecycleCompressorFailure; }
            set
            {
                dbmodel.RecycleCompressorFailure = value;
                this.NotifyPropertyChanged("RecycleCompressorFailure");
            }
        }
        public bool CalcInjectionWaterStream
        {
            get { return dbmodel.CalcInjectionWaterStream; }
            set
            {
                dbmodel.CalcInjectionWaterStream = value;
                this.NotifyPropertyChanged("CalcInjectionWaterStream");
            }
        }
        public double InjectionWaterStream
        {
            get { return dbmodel.InjectionWaterStream; }
            set
            {
                dbmodel.InjectionWaterStream = value;
                this.NotifyPropertyChanged("InjectionWaterStream");
            }
        }

        public bool CalcHXNetworkColdStream
        {
            get { return dbmodel.CalcHXNetworkColdStream; }
            set
            {
                dbmodel.CalcHXNetworkColdStream = value;
                this.NotifyPropertyChanged("CalcHXNetworkColdStream");
            }
        }
        public double HXNetworkColdStream
        {
            get { return dbmodel.HXNetworkColdStream; }
            set
            {
                dbmodel.HXNetworkColdStream = value;
                this.NotifyPropertyChanged("HXNetworkColdStream");
            }
        }

        public double ReliefLoad
        {
            get { return dbmodel.ReliefLoad; }
            set
            {
                dbmodel.ReliefLoad = value;
                this.NotifyPropertyChanged("ReliefLoad");
            }
        }
        public double ReliefMW
        {
            get { return dbmodel.ReliefMW; }
            set
            {
                dbmodel.ReliefMW = value;
                this.NotifyPropertyChanged("ReliefMW");
            }
        }
        public double ReliefTemperature
        {
            get { return dbmodel.ReliefTemperature; }
            set
            {
                dbmodel.ReliefTemperature = value;
                this.NotifyPropertyChanged("ReliefTemperature");
            }
        }
        public double ReliefPressure
        {
            get { return dbmodel.ReliefPressure; }
            set
            {
                dbmodel.ReliefPressure = value;
                this.NotifyPropertyChanged("ReliefPressure");
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

    public class GeneralFailureHXModel : ModelBase
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
