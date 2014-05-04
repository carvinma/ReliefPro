using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliefProModel
{
    public class TowerScenarioStream
    {
        public virtual int ID { get; set; }
        public virtual int ScenarioID { get; set; }
        public virtual string StreamName { get; set; }
        public virtual bool FlowStop { get; set; }
        public virtual string FlowCalcFactor { get; set; }
        public virtual string SourceType { get; set; }
        public virtual bool IsProduct { get; set; }
    }
}
