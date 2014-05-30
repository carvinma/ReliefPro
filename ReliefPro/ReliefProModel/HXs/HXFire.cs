using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReliefProModel.HXs
{
    public class HXFire
    {
        public virtual int ID { get; set; }
        public virtual int ScenarioID { get; set; }

        public virtual string ExposedToFire { get; set; }
        public virtual string Type { get; set; }
        public virtual double OD { get; set; }
        public virtual double Length { get; set; }
        public virtual double Elevation { get; set; }
        public virtual double PipingContingency { get; set; }

    }
}
