using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliefProModel
{
   public class TowerFireOther
    {
        public virtual int ID { get; set; }
        public virtual double? WettedArea { get; set; }
        public virtual double? PipingContingency { get; set; }
        public virtual int EqID { get; set; }

        public virtual string WettedArea_Color { get; set; }
        public virtual string PipingContingency_Color { get; set; }
        public virtual string EqID_Color { get; set; }
    }
}
