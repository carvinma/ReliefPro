using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReliefProModel.Compressors
{
    public class PistonBlockedOutlet
    {
        public virtual int ID { get; set; }
        public virtual int ScenarioID { get; set; }
        public virtual double RatedCapacity { get; set; }       
        public virtual double Reliefload { get; set; }
        public virtual double ReliefMW { get; set; }
        public virtual double ReliefTemperature { get; set; }
        public virtual double ReliefPressure { get; set; }

        public virtual string ScenarioID_Color { get; set; }
        public virtual string RatedCapacity_Color { get; set; }
        public virtual string Reliefload_Color { get; set; }
        public virtual string ReliefMW_Color { get; set; }
        public virtual string ReliefTemperature_Color { get; set; }
        public virtual string ReliefPressure_Color { get; set; }
    }
}
