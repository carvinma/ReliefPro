using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReliefProModel.HXs
{
    public class HXFireSize
    {
        public virtual int ID { get; set; }
        public virtual int ScenarioID { get; set; }

        public virtual string ExposedToFire { get; set; }
        public virtual string Type { get; set; }
        public virtual double OD { get; set; }
        public virtual double Length { get; set; }
        public virtual double Elevation { get; set; }
        public virtual double PipingContingency { get; set; }

        public virtual string ScenarioID_Color { get; set; }
        public virtual string ExposedToFire_Color { get; set; }
        public virtual string Type_Color { get; set; }
        public virtual string OD_Color { get; set; }
        public virtual string Length_Color { get; set; }
        public virtual string Elevation_Color { get; set; }
        public virtual string PipingContingency_Color { get; set; }

    }
}
