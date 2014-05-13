using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliefProModel
{
    public  class InletValveOpen
    {
        public virtual int ID { get; set; }
        public virtual string VesselName { get; set; }
        public virtual string UpStameName { get; set; }
        public virtual string OperatingPhase { get; set; }
        public virtual string MaxOperatingPressure { get; set; }
        public virtual string DownStreamName { get; set; }
        public virtual string TotlCV { get; set; }
        public virtual string ReliefLoad { get; set; }
        public virtual string ReliefTemperature { get; set; }
        public virtual string ReliefPressure { get; set; }
        public virtual string ReliefMW { get; set; }
    }
}
