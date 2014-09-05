using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReliefProModel.Drums;

namespace ReliefProMain.Models
{
    public class DrumBlockedOutletModel : ModelBase
    {
        private string pressureUnit = "MPag";
        public string PressureUnit
        {
            get { return pressureUnit; }
            set
            {
                pressureUnit = value;
                this.NotifyPropertyChanged("PressureUnit");
            }
        }
        private string streamRateUnit = "kg/hr";
        public string StreamRateUnit
        {
            get { return streamRateUnit; }
            set
            {
                streamRateUnit = value;
                this.NotifyPropertyChanged("StreamRateUnit");
            }
        }
        private string flashingDutyUnit = "MW";
        public string FlashingDutyUnit
        {
            get { return flashingDutyUnit; }
            set
            {
                flashingDutyUnit = value;
                this.NotifyPropertyChanged("FlashingDutyUnit");
            }
        }
        private string reliefConditionUnit = "MW";
        public string ReliefConditionUnit
        {
            get { return reliefConditionUnit; }
            set
            {
                reliefConditionUnit = value;
                this.NotifyPropertyChanged("ReliefConditionUnit");
            }
        }

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

        public DrumBlockedOutletModel(DrumBlockedOutlet outletModel)
        {
            dbmodel = outletModel;
            this.maxPressure = dbmodel.MaxPressure;
            this.maxStreamRate = dbmodel.MaxStreamRate;
            this.drumType = dbmodel.DrumType;
            this.normalFlashDuty = dbmodel.NormalFlashDuty;
            this.fdReliefCondition = dbmodel.FDReliefCondition;
            this.feed = dbmodel.Feed;
            this.reboilerPinch = dbmodel.ReboilerPinch;
        }
        public DrumBlockedOutlet dbmodel { get; set; }

        private double maxPressure;
        
        [ReliefProMain.Util.RegularExpression(ModelBase.IsNum, ErrorMessage = "GreaterThanZero")]
        public double MaxPressure
        {
            get { return maxPressure; }
            set
            {
                maxPressure = value;
                dbmodel.MaxPressure = value;
                this.NotifyPropertyChanged("MaxPressure");
            }
        }

        private double maxStreamRate;
        
        [ReliefProMain.Util.RegularExpression(ModelBase.IsNum, ErrorMessage = "GreaterThanZero")]
        public double MaxStreamRate
        {
            get { return maxStreamRate; }
            set
            {
                maxStreamRate = value;
                dbmodel.MaxStreamRate = value;
                this.NotifyPropertyChanged("MaxStreamRate");
            }
        }
        private string drumType;
        public string DrumType
        {
            get { return drumType; }
            set
            {
                drumType = value;
                dbmodel.DrumType = value;
                this.NotifyPropertyChanged("DrumType");
            }
        }
        private double normalFlashDuty;
        
        [ReliefProMain.Util.RegularExpression(ModelBase.IsNum, ErrorMessage = "GreaterThanZero")]
        public double NormalFlashDuty
        {
            get { return normalFlashDuty; }
            set
            {
                normalFlashDuty = value;
                dbmodel.NormalFlashDuty = value;
                this.NotifyPropertyChanged("NormalFlashDuty");
            }
        }
        private double fdReliefCondition;
        
        [ReliefProMain.Util.RegularExpression(ModelBase.IsNum, ErrorMessage = "GreaterThanZero")]
        public double FDReliefCondition
        {
            get { return fdReliefCondition; }
            set
            {
                fdReliefCondition = value;
                dbmodel.FDReliefCondition = value;
                this.NotifyPropertyChanged("FDReliefCondition");
            }
        }
        private bool feed;
        public bool Feed
        {
            get { return feed; }
            set
            {
                feed = value;
                dbmodel.Feed = value;
                this.NotifyPropertyChanged("Feed");
            }
        }
        public bool reboilerPinch;
        public bool ReboilerPinch
        {
            get { return reboilerPinch; }
            set
            {
                reboilerPinch = value;
                dbmodel.ReboilerPinch = value;
                this.NotifyPropertyChanged("ReboilerPinch");
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


        private string maxPressure_Color;
        public string MaxPressure_Color
        {
            get { return maxPressure_Color; }
            set
            {
                maxPressure_Color = value;
                dbmodel.MaxPressure_Color = maxPressure_Color;
                this.NotifyPropertyChanged("MaxPressure_Color");
            }
        }

        private string maxStreamRate_Color;
        public string MaxStreamRate_Color
        {
            get { return maxStreamRate_Color; }
            set
            {
                maxStreamRate_Color = value;
                dbmodel.MaxStreamRate_Color = value;
                this.NotifyPropertyChanged("MaxStreamRate");
            }
        }
        private string drumType_Color;
        public string DrumType_Color
        {
            get { return drumType_Color; }
            set
            {
                drumType_Color = value;
                dbmodel.DrumType_Color = value;
                this.NotifyPropertyChanged("DrumType_Color");
            }
        }
        private string normalFlashDuty_Color;
        public string NormalFlashDuty_Color
        {
            get { return normalFlashDuty_Color; }
            set
            {
                normalFlashDuty_Color = value;
                dbmodel.NormalFlashDuty_Color = value;
                this.NotifyPropertyChanged("NormalFlashDuty_Color");
            }
        }
        private string fdReliefCondition_Color;
        public string FDReliefCondition_Color
        {
            get { return fdReliefCondition_Color; }
            set
            {
                fdReliefCondition_Color = value;
                dbmodel.FDReliefCondition_Color = value;
                this.NotifyPropertyChanged("FDReliefCondition_Color");
            }
        }
        private string feed_Color;
        public string Feed_Color
        {
            get { return feed_Color; }
            set
            {
                feed_Color = value;
                dbmodel.Feed_Color = value;
                this.NotifyPropertyChanged("Feed_Color");
            }
        }
        public string reboilerPinch_Color;
        public string ReboilerPinch_Color
        {
            get { return reboilerPinch_Color; }
            set
            {
                reboilerPinch_Color = value;
                dbmodel.ReboilerPinch_Color = value;
                this.NotifyPropertyChanged("ReboilerPinch_Color");
            }
        }
    }
}
