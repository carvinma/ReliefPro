using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReliefProModel.Drum;

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
    }
}
