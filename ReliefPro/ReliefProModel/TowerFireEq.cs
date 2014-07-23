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
        public virtual double? FFactor { get; set; }
        public virtual double? WettedArea { get; set; }
        public virtual string HeatInput { get; set; }
        public virtual double? ReliefLoad { get; set; }
        public virtual int FireID { get; set; }

        public virtual string EqName_Color { get; set; }
        public virtual string Type_Color { get; set; }
        public virtual string Elevation_Color { get; set; }
        public virtual string FireZone_Color { get; set; }
        public virtual string FFactor_Color { get; set; }
        public virtual string WettedArea_Color { get; set; }
        public virtual string HeatInput_Color { get; set; }
        public virtual string ReliefLoad_Color { get; set; }
        public virtual string FireID_Color { get; set; }
    }
}
