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
        private string effluentTemperature2Unit;
        public string EffluentTemperature2Unit
        {
            get { return effluentTemperature2Unit; }
            set
            {
                effluentTemperature2Unit = value;
                this.NotifyPropertyChanged("EffluentTemperature2Unit");
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

        private string reliefPressureUnit;
        public string ReliefPressureUnit
        {
            get
            {
                return this.reliefPressureUnit;
            }
            set
            {
                this.reliefPressureUnit = value;

                OnPropertyChanged("ReliefPressureUnit");
            }
        }

        public ReactorLoopCommon dbmodel { get; set; }
        public ReactorLoopCommonModel(ReactorLoopCommon model)
        {
            dbmodel = model;
            this.reactorType = dbmodel.ReactorType;
            this.effluentTemperature = dbmodel.EffluentTemperature;
            this.effluentTemperature2 = dbmodel.EffluentTemperature2;
            
            this.maxGasRate = model.MaxGasRate;
            this.totalPurgeRate = model.TotalPurgeRate;
            this.reliefMW = model.ReliefMW;
            this.reliefload = model.ReliefLoad;
            this.reliefTemperature = model.ReliefTemperature;
            this.reliefCpCv = model.ReliefCpCv;
            this.reliefZ = model.ReliefZ;

            this.effluentTemperature_Color = dbmodel.EffluentTemperature_Color;
            this.effluentTemperature2_Color = dbmodel.EffluentTemperature2_Color;
            this.totalPurgeRate_Color = dbmodel.TotalPurgeRate_Color;
            this.maxGasRate_Color = dbmodel.MaxGasRate_Color;
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
        private double effluentTemperature;
        public double EffluentTemperature
        {
            get { return effluentTemperature; }
            set
            {
                effluentTemperature = value;
                this.NotifyPropertyChanged("EffluentTemperature");
            }
        }
        private double effluentTemperature2;
        public double EffluentTemperature2
        {
            get { return effluentTemperature2; }
            set
            {
                effluentTemperature2 = value;
                this.NotifyPropertyChanged("EffluentTemperature2");
            }
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

        private string effluentTemperature_Color;
        public string EffluentTemperature_Color
        {
            get { return effluentTemperature_Color; }
            set
            {
                effluentTemperature_Color = value;
                this.NotifyPropertyChanged("EffluentTemperature_Color");
            }
        }
        private string effluentTemperature2_Color;
        public string EffluentTemperature2_Color
        {
            get { return effluentTemperature2_Color; }
            set
            {
                effluentTemperature2_Color = value;
                this.NotifyPropertyChanged("EffluentTemperature2_Color");
            }
        }
        private string maxGasRate_Color;
        public string MaxGasRate_Color
        {
            get { return effluentTemperature_Color; }
            set
            {
                maxGasRate_Color = value;
                this.NotifyPropertyChanged("MaxGasRate_Color");
            }
        }
        private string totalPurgeRate_Color;
        public string TotalPurgeRate_Color
        {
            get { return totalPurgeRate_Color; }
            set
            {
                totalPurgeRate_Color = value;
                this.NotifyPropertyChanged("TotalPurgeRate_Color");
            }
        }

    }
}
