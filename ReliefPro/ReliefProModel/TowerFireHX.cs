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
        public virtual string OD { get; set; }
        public virtual string Length { get; set; }
        public virtual string PipingContingency { get; set; }
        public virtual int EqID { get; set; }
        public virtual string Elevation { get; set; }
    }
}
