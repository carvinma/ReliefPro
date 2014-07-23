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
        public virtual double ExposedToFire { get; set; }
        public virtual string Type { get; set; }
        public virtual double? OD { get; set; }
        public virtual double? Length { get; set; }
        public virtual double? PipingContingency { get; set; }
        public virtual int EqID { get; set; }
        public virtual double? Elevation { get; set; }
    }
}
