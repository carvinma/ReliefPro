using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReliefProModel.HX
{
    public class HXBlockedOutlet : IScenarioModel
    {
        public virtual int ID { get; set; }
        public virtual int ScenarioID { get; set; }

        public virtual string ColdStream { get; set; }
        public virtual double NormalDuty { get; set; }
        public virtual double NormalHotTemperature { get; set; }
        public virtual double NormalColdInletTemperature { get; set; }
        public virtual double NormalColdOutletTemperature { get; set; }

        public virtual double LatentPoint { get; set; }
        public virtual double ReliefLoad { get; set; }
        public virtual double ReliefMW { get; set; }
        public virtual double ReliefTemperature { get; set; }
        public virtual double ReliefPressure { get; set; }
    }
}
