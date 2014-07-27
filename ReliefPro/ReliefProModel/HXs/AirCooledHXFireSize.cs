using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReliefProModel.HXs
{
    public class AirCooledHXFireSize 
    {
        public virtual int ID { get; set; }
        public virtual int ScenarioID { get; set; }
        public virtual double? WettedBundle { get; set; }
        public virtual double? PipingContingency { get; set; }
        public virtual string ScenarioID_Color { get; set; }
        public virtual string WettedBundle_Color { get; set; }
        public virtual string PipingContingency_Color { get; set; }
       
    }
}
