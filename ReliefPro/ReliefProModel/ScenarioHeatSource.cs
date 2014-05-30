using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliefProModel
{
    public class ScenarioHeatSource
    {
        public virtual int ID { get; set; }
        public virtual int HeatSourceID { get; set; }
        public virtual string DutyFactor { get; set; }
        public virtual int ScenarioStreamID { get; set; }
        public virtual int ScenarioID { get; set; }
    }
}
