using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReliefProModel.CompressorBlocked
{
    public class Piston
    {
        public virtual int ID { get; set; }
        public virtual int ScenarioID { get; set; }
        public virtual double RatedCapacity { get; set; }
        public virtual double Reliefload { get; set; }
        public virtual double ReliefMW { get; set; }
        public virtual double ReliefTemp { get; set; }
        public virtual double ReliefPressure { get; set; }
    }
}
