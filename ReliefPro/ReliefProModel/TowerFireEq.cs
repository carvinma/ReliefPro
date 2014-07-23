using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliefProModel
{
    public class TowerFireEq
    {
        public virtual int ID { get; set; }
        public virtual string EqName { get; set; }
        public virtual string Type { get; set; }
        public virtual double? Elevation { get; set; }
        public virtual bool FireZone { get; set; }
        public virtual double?? FFactor { get; set; }
        public virtual double? WettedArea { get; set; }
        public virtual string HeatInput { get; set; }
        public virtual double? ReliefLoad { get; set; }
        public virtual int FireID { get; set; }
    }
}
