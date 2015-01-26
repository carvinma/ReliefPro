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
        public virtual double DutyFactor { get; set; }
        public virtual int ScenarioStreamID { get; set; }
        public virtual int ScenarioID { get; set; }
        public virtual bool IsFB { get; set; }
        public virtual bool IsFired { get; set; }
        public virtual string HeatSourceType { get; set; }
        public virtual string HeatSourceID_Color { get; set; }
        public virtual string DutyFactor_Color { get; set; }
        public virtual string ScenarioStreamID_Color { get; set; }
        public virtual string ScenarioID_Color { get; set; }
        public virtual string IsFB_Color { get; set; }
    }
}
