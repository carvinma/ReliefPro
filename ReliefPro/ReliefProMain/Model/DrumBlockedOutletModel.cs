using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReliefProModel.Drums;

namespace ReliefProMain.Model
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
    }
}
