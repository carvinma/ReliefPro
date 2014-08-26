using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReliefProModel.GlobalDefault
{
    public class ConditionsSettings
    {
        public virtual int ID { get; set; }
        public virtual bool AirCondition { get; set; }
        public virtual bool CoolingWaterCondition { get; set; }
        public virtual bool SteamCondition { get; set; }
        public virtual double LatentHeatSettings { get; set; }
        public virtual double DrumSurgeTimeSettings { get; set; }

       
    }
}
