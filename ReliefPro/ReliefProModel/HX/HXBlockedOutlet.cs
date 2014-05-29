using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReliefProModel.HX
{
    public class HXBlockedOutlet
    {
        public virtual int ID { get; set; }
        public virtual int ScenarioID { get; set; }

        public virtual double ColdStream { get; set; }
        public virtual double NormalDuty { get; set; }
        public virtual double NormalHotTemp { get; set; }
        public virtual double NormalColdInletTemp { get; set; }
        public virtual double NormalColdOutletTemp { get; set; }

        public virtual double Reliefload { get; set; }
        public virtual double ReliefMW { get; set; }
        public virtual double ReliefTemperature { get; set; }
        public virtual double ReliefPressure { get; set; }
    }
}
