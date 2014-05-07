using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliefProModel
{
    public class TowerFireColumn
    {
        public virtual int ID { get; set; }
        public virtual string NumberOfSegment { get; set; }
        public virtual string Elevation { get; set; }
        public virtual string BNLL { get; set; }
        public virtual string LiquidHoldup { get; set; }        
        public virtual string PipingContingency { get; set; }
        public virtual int EqID { get; set; }
       
    }
}
