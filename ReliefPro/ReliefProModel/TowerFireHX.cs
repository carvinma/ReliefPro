using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliefProModel
{
    public class TowerFireHX
    {
        public virtual int ID { get; set; }
        public virtual string ExposedToFire { get; set; }

        public virtual string Type { get; set; }
        public virtual double? OD { get; set; }
        public virtual double? Length { get; set; }
        public virtual double? PipingContingency { get; set; }
        public virtual int EqID { get; set; }
        public virtual double? Elevation { get; set; }

        public virtual string ExposedToFire_Color { get; set; }
        public virtual string Type_Color { get; set; }
        public virtual string OD_Color { get; set; }
        public virtual string Length_Color { get; set; }
        public virtual string PipingContingency_Color { get; set; }
        public virtual string EqID_Color { get; set; }
        public virtual string Elevation_Color { get; set; }
    }
}
