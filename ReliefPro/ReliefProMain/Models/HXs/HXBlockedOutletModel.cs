using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReliefProModel.HXs;

namespace ReliefProMain.Models.HXs
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
            this.reliefCpCv = model.ReliefCpCv;
            this.reliefZ = model.ReliefZ;
        }

        private string coldStream;
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
        [ReliefProMain.Util.Required(ErrorMessage = "NotEmpty")]
        [ReliefProMain.Util.RegularExpression(ModelBase.GreaterThanZero, ErrorMessage = "GreaterThanZero")]
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
        [ReliefProMain.Util.Required(ErrorMessage = "NotEmpty")]
        [ReliefProMain.Util.RegularExpression(ModelBase.GreaterThanZero, ErrorMessage = "GreaterThanZero")]
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
        [ReliefProMain.Util.Required(ErrorMessage = "NotEmpty")]
        [ReliefProMain.Util.RegularExpression(ModelBase.GreaterThanZero, ErrorMessage = "GreaterThanZero")]
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
        [ReliefProMain.Util.Required(ErrorMessage = "NotEmpty")]
        [ReliefProMain.Util.RegularExpression(ModelBase.GreaterThanZero, ErrorMessage = "GreaterThanZero")]
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






        private string coldStream_Color;
        public string ColdStream_Color
        {
            get { return coldStream_Color; }
            set
            {
                coldStream_Color = value;
                NotifyPropertyChanged("ColdStream_Color");
            }
        }

        private double normalDuty_Color;
        public double NormalDuty_Color
        {
            get { return normalDuty_Color; }
            set
            {
                normalDuty_Color = value;
                NotifyPropertyChanged("NormalDuty_Color");
            }
        }

        private double normalHotTemperature_Color;
        public double NormalHotTemperature_Color
        {
            get { return normalHotTemperature_Color; }
            set
            {
                normalHotTemperature_Color = value;
                NotifyPropertyChanged("NormalHotTemperature_Color");
            }
        }

        private double normalColdInletTemperature_Color;
        public double NormalColdInletTemperature_Color
        {
            get { return normalColdInletTemperature_Color; }
            set
            {
                normalColdInletTemperature_Color = value;
                NotifyPropertyChanged("NormalColdInletTemperature_Color");
            }
        }

        private double normalColdOutletTemperature_Color;
        public double NormalColdOutletTemperature_Color
        {
            get { return normalColdOutletTemperature_Color; }
            set
            {
                normalColdOutletTemperature_Color = value;
                NotifyPropertyChanged("NormalColdOutletTemperature_Color");
            }
        }

        private double latentPoint_Color;
        public double LatentPoint_Color
        {
            get { return latentPoint_Color; }
            set
            {
                latentPoint_Color = value;
                NotifyPropertyChanged("LatentPoint_Color");
            }
        }

        private double reliefMW_Color;
        public double ReliefMW_Color
        {
            get { return reliefMW_Color; }
            set
            {
                reliefMW_Color = value;
                this.NotifyPropertyChanged("ReliefMW_Color");
            }
        }

        private double reliefload_Color;
        public double ReliefLoad_Color
        {
            get { return reliefload_Color; }
            set
            {
                reliefload_Color = value;
                this.NotifyPropertyChanged("ReliefLoad_Color");
            }
        }

        private double reliefTemperature_Color;
        public double ReliefTemperature_Color
        {
            get { return reliefTemperature_Color; }
            set
            {
                reliefTemperature_Color = value;
                this.NotifyPropertyChanged("ReliefTemperature_Color");
            }
        }

        private double reliefPressure_Color;
        public double ReliefPressure_Color
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
