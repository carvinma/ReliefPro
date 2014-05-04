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
        public virtual string WettedArea { get; set; }
        public virtual string PipingContingency { get; set; }
        public virtual int EqID { get; set; }
    }
}
