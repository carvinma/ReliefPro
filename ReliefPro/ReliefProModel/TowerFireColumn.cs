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
        public virtual int NumberOfSegment { get; set; }
        public virtual double? Elevation { get; set; }
        public virtual double? BNLL { get; set; }
        public virtual double? LiquidHoldup { get; set; }
        public virtual double? PipingContingency { get; set; }
        public virtual int EqID { get; set; }
       
    }
}
