using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReliefProModel.HXs
{
    public class TubeRupture : IScenarioModel
    {
        public virtual int ID { get; set; }
        public virtual int ScenarioID { get; set; }

        
        public virtual double OD { get; set; }
        
        public virtual double ReliefLoad { get; set; }
        public virtual double ReliefMW { get; set; }
        public virtual double ReliefTemperature { get; set; }
        public virtual double ReliefPressure { get; set; }

        
        public virtual string OD_Color { get; set; }
        public virtual string ReliefLoad_Color { get; set; }
        public virtual string ReliefMW_Color { get; set; }
        public virtual string ReliefTemperature_Color { get; set; }
        public virtual string ReliefPressure_Color { get; set; }
    }
}
