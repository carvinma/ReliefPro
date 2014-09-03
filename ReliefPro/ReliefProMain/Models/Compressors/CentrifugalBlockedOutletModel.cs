using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReliefProModel.Compressors;


namespace ReliefProMain.Models.Compressors
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
            this.inletLoad = model.InletLoad;
            this.outletPressure = model.OutletPressure;
            this.surgeLoad = model.SurgeLoad;
            this.scale = model.Scale;
            this.reliefMW = model.ReliefMW;
            this.reliefload = model.Reliefload;
            this.reliefTemperature = model.ReliefTemperature;
            this.reliefPressure = model.ReliefPressure;
            this.reliefCpCv = model.ReliefCpCv;
            this.reliefZ = model.ReliefZ;

            this.inletLoad_Color = model.InletLoad_Color;
            this.outletPressure_Color = model.OutletPressure_Color;
            this.surgeLoad_Color = model.SurgeLoad_Color;
            this.scale_Color = model.Scale_Color;
            this.reliefMW_Color = model.ReliefMW_Color;
            this.reliefload_Color = model.Reliefload_Color;
            this.reliefTemperature_Color = model.ReliefTemperature_Color;
            this.reliefPressure_Color = model.ReliefPressure_Color;
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


        private string scale_Color;
        public string Scale_Color
        {
            get { return scale_Color; }
            set
            {
                scale_Color = value;
                this.NotifyPropertyChanged("Scale_Color");
            }
        }

        private string inletLoad_Color;
        public string InletLoad_Color
        {
            get { return inletLoad_Color; }
            set
            {
                inletLoad_Color = value;
                this.NotifyPropertyChanged("InletLoad_Color");
            }
        }

        private string outletPressure_Color;
        public string OutletPressure_Color
        {
            get { return outletPressure_Color; }
            set
            {
                outletPressure_Color = value;
                this.NotifyPropertyChanged("OutletPressure_Color");
            }
        }

        private string surgeLoad_Color;
        public string SurgeLoad_Color
        {
            get { return surgeLoad_Color; }
            set
            {
                surgeLoad_Color = value;
                this.NotifyPropertyChanged("SurgeLoad_Color");
            }
        }

        private string reliefMW_Color;
        public string ReliefMW_Color
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
